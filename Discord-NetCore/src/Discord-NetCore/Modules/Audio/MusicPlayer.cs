using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules.Audio
{
    /// <summary>
    /// Guild specific music player
    /// </summary>
    public class MusicPlayer
    {
        /// <summary>
        /// Volume of the stream
        /// </summary>
        public float Volume { get; set; } = 0.3f;
        /// <summary>
        /// IAudioCient should be specific to a guild
        /// </summary>
        public IAudioClient AudioClient { get; private set; }
        /// <summary>
        /// Currently connected channel if any
        /// </summary>
        public IVoiceChannel ConnectedChannel { get; private set; }
        /// <summary>
        /// Allows audio stream to stop
        /// </summary>
        public CancellationTokenSource AudioCancelSource { get; private set; }
        /// <summary>
        /// Allows cancelling of audio data
        /// </summary>
        private CancellationToken CancelToken { get; set; }
        /// <summary>
        /// Thread safe Song queue
        /// </summary>
        private ConcurrentQueue<Song> _songQueue { get; set; } = new ConcurrentQueue<Song>();

        /// <summary>
        /// Thread for running an audio stream
        /// </summary>
        private Thread StreamThread { get; set; }
        /// <summary>
        /// Whether the audio stream is paused
        /// </summary>
        public bool Paused { get; private set; } = false;
        /// <summary>
        /// True if a stream is not playing (IE the stream is free)
        /// </summary>
        public bool AudioFree { get; private set; }
        /// <summary>
        /// Whether the stream will play automatically when the first song is queued
        /// </summary>
        public bool AutoPlay { get; set; } = true;
        /// <summary>
        /// Whether the current song will skip
        /// </summary>
        public bool SlatedForSkip { get; private set; } = false;
        /// <summary>
        /// Allows sending messages to the channel
        /// </summary>
        private CommandContext _context { get; set; }
        /// <summary>
        /// Stream process
        /// </summary>
        private Process process { get; set; }
        public MusicPlayer(CommandContext context)
        {
            _context = context;
            AudioFree = true;
        }
        /// <summary>
        /// Attempts to move the bot to an audio channel
        /// </summary>
        /// <param name="chan"></param>
        /// <returns></returns>
        public async Task MoveToVoiceChannel(IVoiceChannel chan)
        {
            if (AudioClient == null || AudioClient.ConnectionState == ConnectionState.Disconnected)
            {
                AudioClient = await chan.ConnectAsync();
                ConnectedChannel = chan;
            }
            // If the bot is connected to a voice channel and the user is in a different voice channel
            else if (AudioClient.ConnectionState == ConnectionState.Connected && !(chan.Id == ConnectedChannel.Id))
            {
                await AudioClient.DisconnectAsync();
                AudioClient = await chan.ConnectAsync();
                ConnectedChannel = chan;
            }
        }

        /// <summary>
        /// Plays a song from a file
        /// </summary>
        /// <param name="song">Song file</param>
        /// <param name="volume">Volume to play at</param>
        /// <returns></returns>
        public async Task PlaySong(string song, float volume)
        {
            using (var stream = AudioClient.CreatePCMStream(2880, bitrate: ConnectedChannel.Bitrate))
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments =
                    $"-i \"{song}\" " +
                    "-f s16le -ar 48000 -ac 2 pipe:1 -loglevel quiet -af \"volume=0.3\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                });

                await process.StandardOutput.BaseStream.CopyToAsync(stream);
                await stream.FlushAsync();

                process.WaitForExit();
            }
        }
        public void TruffleShuffle()
        {
            var songList = new List<Song>();
            songList = _songQueue.ToList();
            // delete the list somehow
            _songQueue = new ConcurrentQueue<Song>();
            for (var i = 0; i < songList.Count; i++)
            {
                Song temp;
                var rand = DateTime.Now.ToFileTimeUtc() % songList.Count;
                temp = songList[i];
                songList[i] = songList[(int)rand];
                songList[(int)rand] = temp;
            }
            foreach (var s in songList)
                _songQueue.Enqueue(s);
        }
        /// <summary>
        /// Adjusts PCM audio volume
        /// </summary>
        /// <param name="audioSamples"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        private static unsafe byte[] AdjustVolume(byte[] audioSamples, float volume)
        {
            Contract.Requires(audioSamples != null);
            Contract.Requires(audioSamples.Length % 2 == 0);
            Contract.Requires(volume >= 0f && volume <= 1f);
            Contract.Assert(BitConverter.IsLittleEndian);

            if (Math.Abs(volume - 1f) < 0.0001f) return audioSamples;

            // 16-bit precision for the multiplication
            int volumeFixed = (int)Math.Round(volume * 65536d);

            int count = audioSamples.Length / 2;

            fixed (byte* srcBytes = audioSamples)
            {
                short* src = (short*)srcBytes;

                for (int i = count; i != 0; i--, src++)
                    *src = (short)(((*src) * volumeFixed) >> 16);
            }

            return audioSamples;
        }
        /// <summary>
        /// Toggles whether Paused is true or false
        /// </summary>
        public void TogglePause()
        {
            Paused = !Paused;
        }
        /// <summary>
        /// Stop the audio stream
        /// </summary>
        public void StopAudio()
        {
            try
            {
                AudioCancelSource.Cancel();
                AudioCancelSource.Dispose();

                if (!AudioFree)
                    AudioFree = true;
            } catch (Exception)
            {
                throw;
            }
        }
        public async Task RepeatAudio()
        {
            var RecordingCancelSource = new CancellationTokenSource();
            var RecordingCancelToken = RecordingCancelSource.Token;
            await Task.Factory.StartNew(async () =>
            {
                using (var stream = AudioClient.CreatePCMStream(2880, bitrate: ConnectedChannel.Bitrate))
                {
                    while (true)
                    {
                        var buffer = new byte[1024];
                        await stream.ReadAsync(buffer, 0, 1024);
                        await stream.WriteAsync(buffer, 0, 1024);
                        
                    }
                }
            });
        }
        /// <summary>
        /// Runs the music queue
        /// </summary>
        /// <param name="context">CommandContext for sending a message when a new song is playing</param>
        /// <returns></returns>
        public async Task RunQueue()
        {
            if (!AudioFree)
                throw new AudioStreamInUseException("Something is currently playing!");
            AudioCancelSource = new CancellationTokenSource();
            CancelToken = AudioCancelSource.Token;

            StreamThread = new Thread(async () =>
            {
                AudioFree = false;
                while (_songQueue.Any())
                {

                    Song song;
                    _songQueue.TryDequeue(out song);
                    await _context.Channel.SendMessageAsync($"Now playing: `{song.Title}`");
                    await StreamYoutube(song.Url, CancelToken);
                    // /\ /\ WARNING /\ /\ hack detected ahead!!
                    while (process == null) // Wait for process to start
                        Thread.Sleep(100);
                    while (process != null || !process.HasExited) // wait for process to stop
                        Thread.Sleep(100);
                }
                StreamThread.Join(); // Does this even do anything
                AudioFree = true;
                Console.WriteLine($"{DateTime.Now}: Process finished.");
            });
            StreamThread.Start();
        }
        /// <summary>
        /// Gets the current queue in string format
        /// </summary>
        /// <returns></returns>
        public string GetQueue()
        {
            var str = "";
            var i = 1;
            foreach (var song in _songQueue)
            {
                str += $"{i} : {song.Title}\n";
                i++;
            }
            return str;

        }
        public void SkipSong()
        {
            SlatedForSkip = true;

        }
        /// <summary>
        /// Created a new Song object based on a url and adds it to the queue
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task AddToQueue(string url)
        {
            try
            {
                var song = new Song(url);
                await song.GetVideoInfo();
                _songQueue.Enqueue(song);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Stream youtube audio data to discord voice channel. Works by youtube-dl piping video data to ffmpeg, 
        /// which then extracts the audio and outputs it, which is then read by a stream
        /// </summary>
        /// <param name="url">Url of the video</param>
        /// <returns></returns>
        private async Task StreamYoutube(string url, CancellationToken cancelToken)
        {
            using (var stream = AudioClient.CreatePCMStream(2880, bitrate: ConnectedChannel.Bitrate))
            {
                process = Process.Start(new ProcessStartInfo
                {
                    // add way to execute bash if on linux
                    FileName = "cmd",
                    Arguments = $"/C youtube-dl -q -o - {url} | ffmpeg -i - -f s16le -ar 48000 -ac 2 -loglevel quiet pipe:1 ",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,

                });

                process.Exited += (o, e) =>
                {
                    process.Dispose();
                };

                try
                {
                    int blockSize = 1024;
                    var buffer = new byte[blockSize];
                    int byteCount = 1;
                    do
                    {
                        // Don't send any data or read from the stream if the stream is supposed to be paused
                        if (Paused)
                            continue;
                        byteCount = await process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                        if (byteCount == 0 || SlatedForSkip)
                            break;

                    if (cancelToken.IsCancellationRequested)
                        break;
                        
                        buffer = AdjustVolume(buffer, Volume);
                        await stream.WriteAsync(buffer, 0, blockSize);
                    } while (byteCount > 0);
                    await stream.FlushAsync();
                    process.WaitForExit();
                    
                    SlatedForSkip = true;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream writing cancelled.");
                }
                SlatedForSkip = true;
            }
        }
    }
}
