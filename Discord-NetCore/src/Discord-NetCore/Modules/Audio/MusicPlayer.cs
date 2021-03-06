﻿using Discord;
using Discord.Audio;
using Discord.Audio.Streams;
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
using Google.Cloud.Speech.V1;

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
        public ConcurrentQueue<Song> _songQueue { get; private set; } = new ConcurrentQueue<Song>();
    
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
        public bool AutoPlay { get; set; } = false;
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

        private ProcessStartInfo _processInfo { get; set; }

        private Discord.Audio.AudioInStream InputStream { get; set; }
        /// <summary>
        /// Set up a new music player
        /// </summary>
        /// <param name="context">Allows the object to send messages to the server</param>
        public MusicPlayer(ICommandContext context)
        {
            _context = context;
            AudioFree = true;
        }

        public Song CurrentSong { get; private set; }

        /// <summary>
        /// Attempts to move the bot to an audio channel
        /// </summary>
        /// <param name="chan"></param>
        /// <returns></returns>
        public async Task MoveToVoiceChannel(IVoiceChannel chan)
        {
            AudioClient = await chan.ConnectAsync();
            ConnectedChannel = chan;

           
            //This seems redundant
            /*
            if (AudioClient == null || AudioClient.ConnectionState == ConnectionState.Disconnected)
            {
                AudioClient = await chan.ConnectAsync();
                ConnectedChannel = chan;
                AudioClient.StreamCreated += AudioClient_StreamCreated;
            }
            // If the bot is connected to a voice channel and the user is in a different voice channel
            else if (AudioClient.ConnectionState == ConnectionState.Connected && !(chan.Id == ConnectedChannel.Id))
            {
                AudioClient = await chan.ConnectAsync();
                ConnectedChannel = chan;
            }
            */
        }

        /// <summary>
        /// Creates a new stream when a user joins the voice channel
        /// repeats all their audio
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        private async Task StreamCreated(ulong arg1, AudioInStream arg2)
        {
            try
            {
                using (var stream = AudioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    if (Program.DEBUG)
                    {
                        Console.WriteLine(arg1); // User ID
                        //await arg2.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                AudioCancelSource.Cancel();
                _songQueue.Clear();
                CurrentSong = null;
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// test audio input
        /// </summary>
        /// <returns></returns>
        public async Task RepeatAudio()
        {
            return;
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
            await RunQueueThread();
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
                CurrentSong = song;

                //await _context.Channel.SendMessageAsync($"Now playing: `{CurrentSong.Title}`");

                var builder = new EmbedBuilder()
                    .WithTitle("Gachi's Gucci Jukebox!")
                    .WithColor(new Color(0xA4F233))
                    .WithThumbnailUrl("http://i0.kym-cdn.com/photos/images/original/000/666/924/849.jpg")
                    .AddField("Now Playing:", $"{CurrentSong.Title}");
                var embed = builder.Build();
                await _context.Channel.SendMessageAsync(
                    "",
                    embed: embed);
                   
                await StreamAudio(CurrentSong.DirectLink, CancelToken);
                Console.WriteLine("Playing the next song...");
            }
            _songQueue.Clear();
            CurrentSong = null;
            AudioFree = true;
            Console.WriteLine($"{DateTime.Now}: Queue finished.");
            
        }
        /// <summary>
        /// Gets the current queue in string format
        /// </summary>
        /// <returns></returns>
        public List<Song> GetQueue()
        {
            return _songQueue.ToList();
        }
        /// <summary>
        /// Skip to the next song in the queue
        /// Will default to votes unless the person who requested the current song is skipping
        /// </summary>
        public async Task SkipSong(ICommandContext context)
        {
            if (WillSkip)
            {
                await context.Channel.SendMessageAsync("The song is set to be skipped!");
                return;
            }
            var song = CurrentSong;
            
            if (context.User.Id == song.RequestedBy || context.User.Id == Program.OwnerId)

            {
                await context.Channel.SendMessageAsync($"{context.User} is skipping their own song.");
                WillSkip = true;
            }
            else if (song.UsersVoted.Any(user => user == context.User.Id))
            {
                await context.Channel.SendMessageAsync("You have already voted.");
            }
            else
            {
                var voiceUsers = await ConnectedChannel.GetUsersAsync().FlattenAsync();
                var requiredVotes = voiceUsers.Count(user => !user.IsBot) / 2; // Half the users in voice 
                song.SkipVotes++;
                song.UsersVoted.Add(context.User.Id);
                await context.Channel.SendMessageAsync($"Votes: {song.SkipVotes}/{requiredVotes}");
                if (song.SkipVotes >= requiredVotes && !WillSkip)
                {
                    WillSkip = true;
                    await context.Channel.SendMessageAsync("Skipping song!");
                    song.SkipVotes = 0;
                }
            }
        }
        /// <summary>
        /// Created a new Song object based on a url and adds it to the queue
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Song> AddToQueue(string url, ICommandContext context)
        {
            try
            {
                var song = new Song(url, context);
                await song.GetVideoInfo();
                _songQueue.Enqueue(song);
                return song;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                Console.WriteLine("Something has gone wrong with adding a song.");
                return null;
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
        private async Task StreamAudio(string url, CancellationToken cancelToken)
        {
            Console.WriteLine("Youtube requested");
            using (var stream = AudioClient.CreatePCMStream(application: AudioApplication.Mixed))
            {
                try
                {

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        #if DEBUG
                            Console.WriteLine("Windows Detected");
                        #endif
                        _process = Process.Start(new ProcessStartInfo
                        {
                            // 'Direct' method using only ffmpeg and a music link
                            
                            FileName = "Binaries\\ffmpeg",
                            Arguments =
                             $"-i \"{url}\" " +
                            " -ac 2 -f s16le -ar 48000 pipe:1",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false
                            
                            // 'indirect' method using both youtube-dl and ffmpeg
                            /*
                            FileName = "cmd",
                            Arguments = $"/C youtube-dl.exe --hls-prefer-native -q -o - {url} | ffmpeg.exe -i - -f s16le -ar 48000 -ac 2 -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 10 pipe:1 -b:a 96K ",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false,
                            */
                        });
                    } else
                    {
                        #if DEBUG
                            Console.WriteLine("Linux Detected");
                        #endif
                        _process = Process.Start(new ProcessStartInfo
                        {
                            /*
                            FileName = "/bin/bash",
                            Arguments =
                             $"-c \"ffmpeg -i \'{url}\' " +
                            " -ac 2 -f s16le -ar 48000 -loglevel panic pipe:1 \" ",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false
                            */
                            FileName = "/bin/bash",
                            Arguments = $"youtube-dl.exe --hls-prefer-native -q -o - {url} | ffmpeg.exe -i - -f s16le -ar 48000 -ac 2 -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 10 pipe:1 -b:a 96K",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false,
                        });
                    }
                    Console.WriteLine("Starting process...");
                    int blockSize = 512;
                    var buffer = new byte[blockSize];
                    int byteCount = 1;
                    do
                    {
                        // Don't send any data or read from the stream if the stream is supposed to be paused
                        if (Paused) continue;

                        if (cancelToken.IsCancellationRequested || WillSkip)
                            break;

                        byteCount = await _process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                        //buffer = AdjustVolume(buffer, Volume);
                        await stream.WriteAsync(buffer, 0, blockSize);
                    } while (byteCount > 0);
                    if (!WillSkip)
                        _process.WaitForExit();
                    _process.Close();
                    await stream.FlushAsync();
                    WillSkip = false;
                    Paused = false;


                    #if DEBUG
                        Console.WriteLine("Process finished.");
                    #endif
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Cancelled by user.");
                    _process.Close();
                    await stream.FlushAsync();
                    WillSkip = false;
                }
                catch (FileNotFoundException)
                {
                    await _context.Channel.SendMessageAsync("Error, Youtube-dl and/or ffmpeg can not be found");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                }
            }
        }

        private void _process_Exited(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
