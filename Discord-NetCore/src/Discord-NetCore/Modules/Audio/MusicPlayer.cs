using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
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
        public bool WillSkip { get; private set; } = false;
        /// <summary>
        /// Allows sending messages to the channel
        /// </summary>
        private ICommandContext _context { get; set; }
        /// <summary>
        /// Stream process
        /// </summary>
        private Process _process { get; set; }
        /// <summary>
        /// Set up a new music player
        /// </summary>
        /// <param name="context">Allows the object to send messages to the server</param>
        public MusicPlayer(ICommandContext context)
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
        public async Task PlaySong(string song, CancellationToken cancelToken)
        {
            using (var stream = AudioClient.CreatePCMStream(AudioApplication.Music))
            {
                _process = Process.Start(new ProcessStartInfo
                {
                    FileName = "Binaries\\ffmpeg",
                    Arguments =
                    $"-i \"{song}\" " +
                    "-f s16le -ar 48000 -ac 2 pipe:1 -loglevel quiet",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                });
                try
                {
                    int blockSize = 1024;
                    var buffer = new byte[blockSize];
                    int byteCount = 1;
                    do
                    {
                        // Don't send any data or read from the stream if the stream is supposed to be paused
                        if (Paused) continue;

                        if (cancelToken.IsCancellationRequested || byteCount == 0 || WillSkip)
                            break;

                        byteCount = await _process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                        buffer = AdjustVolume(buffer, Volume);
                        await stream.WriteAsync(buffer, 0, blockSize);
                    } while (byteCount > 0);
                    _process.WaitForExit();
                    await stream.FlushAsync();
                    WillSkip = false;

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream writing cancelled.");
                    WillSkip = false;
                }
            }
        }
        /// <summary>
        /// Shuffle the queue
        /// </summary>
        public void TruffleShuffle()
        {
            if (_songQueue.Count == 0)
                return;
            var songList = new List<Song>();
            songList = _songQueue.ToList();
            // delete the list somehow
            _songQueue = new ConcurrentQueue<Song>();
            for (var i = 0; i < songList.Count; i++)
            {
                Song temp;
                var rand = DateTime.Now.ToFileTimeUtc() % songList.Count; // wahoo generate a new random number every time, even the same numbers
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
                if (AudioFree)
                    return;
                AudioCancelSource.Cancel();
                AudioCancelSource.Dispose();

                if (!AudioFree)
                    AudioFree = true;
            } catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// test audio input
        /// </summary>
        /// <returns></returns>
        public async Task RepeatAudio()
        {
            var RecordingCancelSource = new CancellationTokenSource();
            var RecordingCancelToken = RecordingCancelSource.Token;
            await Task.Factory.StartNew(async () =>
            {
                using (var stream = AudioClient.CreatePCMStream(AudioApplication.Music))
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
        public void RunQueue()
        {
            if (!AudioFree)
                throw new AudioStreamInUseException("Something is currently playing!");

            StreamThread = new Thread(new ThreadStart(async () =>
                await RunQueueThread()));
            StreamThread.Start();
        }
        private async Task RunQueueThread()
        {
            AudioFree = false;
            while (_songQueue.Any())
            {
                AudioCancelSource = new CancellationTokenSource();
                CancelToken = AudioCancelSource.Token;
                Song song;
                _songQueue.TryDequeue(out song);
                await _context.Channel.SendMessageAsync($"Now playing: `{song.Title}`");
                if (song.IsFile)
                    await PlaySong(song.Url, CancelToken);
                else
                    await StreamYoutube(song.Url, CancelToken);
                Console.WriteLine("Running audio...");
                // /\ /\ WARNING /\ /\ hack detected ahead!!
                while (!_process.HasExited) // wait for process to stop
                    Thread.Sleep(100);
            }
            AudioFree = true;
            _process.Dispose();
            Console.WriteLine($"{DateTime.Now}: Process finished.");
            
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
        /// <summary>
        /// Skip to the next song in the queue
        /// </summary>
        public async Task SkipSong(ICommandContext context)
        {
            if (context.User.Id == _songQueue.First().RequestedBy.User.Id)
                WillSkip = true;
            else
                await context.Channel.SendMessageAsync("You didn't originally request the song!");
        }
        /// <summary>
        /// Created a new Song object based on a url and adds it to the queue
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task AddToQueue(string url, ICommandContext context)
        {
            try
            {
                var song = new Song(url, context);
                await song.GetVideoInfo();
                _songQueue.Enqueue(song);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task AddFileToQueue(string path, ICommandContext context, bool isFile)
        {
            try
            {
                var song = new Song(path, context, isFile);
                _songQueue.Enqueue(song);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Stream youtube audio data to discord voice channel. Works by youtube-dl piping video data to ffmpeg, 
        /// which then extracts the audio and outputs it, which is then read by a stream, which is then forced into the user's ear
        /// </summary>
        /// <param name="url">Url of the video</param>
        /// <returns></returns>
        private async Task StreamYoutube(string url, CancellationToken cancelToken)
        {
            using (var stream = AudioClient.CreatePCMStream(AudioApplication.Music))
            {
                try
                {
                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        _process = Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = $"/C Binaries\\youtube-dl.exe -q -o - {url} | ffmpeg.exe -i - -f s16le -ar 48000 -ac 2 -loglevel quiet pipe:1 ",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false,
                        });
                    } else
                    {
                        _process = Process.Start(new ProcessStartInfo
                        {
                            FileName = "bash",
                            Arguments = $"/app/heroku_output/youtube-dl -q -o - {url} | /app/heroku_output/ffmpeg -i - -f s16le -ar 48000 -ac 2 -loglevel quiet pipe:1 ",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false,
                        });
                    }
                    Console.WriteLine("Starting process...");
                    int blockSize = 2048;
                    var buffer = new byte[blockSize];
                    int byteCount = 1;
                    do
                    {
                        // Don't send any data or read from the stream if the stream is supposed to be paused
                        if (Paused) continue;

                        if (cancelToken.IsCancellationRequested || byteCount == 0 || WillSkip)
                            break;

                        byteCount = await _process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                        buffer = AdjustVolume(buffer, Volume);
                        await stream.WriteAsync(buffer, 0, blockSize);
                    } while (byteCount > 0);
                    _process.WaitForExit();
                    await stream.FlushAsync();
                    WillSkip = false;

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream writing cancelled.");
                    WillSkip = false;
                }
                catch (FileNotFoundException)
                {
                    await _context.Channel.SendMessageAsync("Error, Youtube-dl and/or ffmpeg can not be found");
                }
            }
        }
        public async Task audioTest()
        {
            using (var stream = AudioClient.CreatePCMStream(AudioApplication.Mixed))
            {

                try
                {
                    int blockSize = 1024;
                    var buffer = new byte[blockSize];
                    int byteCount = 1;
                    do
                    {

                        byteCount = await stream.ReadAsync(buffer, 0, blockSize);
                        buffer = AdjustVolume(buffer, Volume);
                        await stream.WriteAsync(buffer, 0, blockSize);
                    } while (byteCount > 0);

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream writing cancelled.");
                    WillSkip = false;
                }
            }
        }
    }

}
