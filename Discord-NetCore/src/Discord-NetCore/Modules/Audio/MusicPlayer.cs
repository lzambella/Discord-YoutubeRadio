using Discord;
using Discord.Audio;
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
    public class MusicPlayer
    {

        public float Volume { get; set; }
        public IAudioClient AudioClient { get; private set; }
        public IVoiceChannel ConnectedChannel { get; private set; }

        public CancellationTokenSource AudioCancelSource { get; private set; }
        private CancellationToken CancelToken { get; set; }

        private CancellationTokenSource RecordingCancelSource { get; set; }
        private CancellationToken RecordingCancelToken { get; set; }

        private ConcurrentQueue<Action> actionQueue { get; } = new ConcurrentQueue<Action>();

        private Boolean AudioFree { get; set; } = true;
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
        /// <param name="chan">IVoiceChannel the user is in if any</param>
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

                //Doing it this way causes the stream to lag
                /*
                int blockSize = 420;
                var buffer = new byte[blockSize];
                int byteCount;
                do
                {
                    byteCount = await Process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                    if (byteCount == 0)
                        break;
                    buffer = AdjustVolume(buffer, Volume);
                    await stream.WriteAsync(buffer, 0, blockSize);
                    
                    await stream.FlushAsync();
                } while (byteCount > 0);
                */

                process.WaitForExit();
            }
        }
        
        // stolen from.... somewhere
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

        public void StopAudio()
        {
            try
            {
                AudioCancelSource.Cancel();
            } catch (Exception)
            {
                throw;
            }
        }
        /*
        public async Task RepeatAudio()
        {
            RecordingCancelSource = new CancellationTokenSource();
            RecordingCancelToken = RecordingCancelSource.Token;
            await Task.Factory.StartNew(async () =>
            {
                using (var stream = I)
            }, RecordingCancelToken);
        }
        */
        /// <summary>
        /// Stream youtube audio data to discord voice channel
        /// </summary>
        /// <param name="url">Url of the video</param>
        /// <returns></returns>
        public async Task StreamYoutube(string url)
        {
            AudioCancelSource = new CancellationTokenSource();
            CancelToken = AudioCancelSource.Token;

            using (var stream = AudioClient.CreatePCMStream(2880, bitrate: ConnectedChannel.Bitrate))
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    // add way to execute bash if on linux
                    FileName = "cmd",
                    Arguments = $"/K youtube-dl -q -o - {url} | ffmpeg -i - -f s16le -ar 48000 -ac 2 -af \"volume=0.3\" pipe:1 ",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                });

                await process.StandardOutput.BaseStream.CopyToAsync(stream, 81920, CancelToken);
                await stream.FlushAsync();
                process.WaitForExit();
            }
        }


    }
}
