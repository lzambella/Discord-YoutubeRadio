using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Discord.WebSocket;
namespace Discord_NetCore.Modules 
{
    [Name("Audio")]
    public class Audio : ModuleBase
    {
        private IAudioClient client { get; set; }
        // Create a Join command, that will join the parameter or the user's current voice channel
        [Command("joinchannel")]
        public async Task JoinChannel()
        {
            try
            {
                // Get the audio channel
                var user = (IGuildUser) Context.User;
                var channel = user.VoiceChannel;
                if (channel == null) { await ReplyAsync("User must be in a voice channel"); return; }
                client = await channel.ConnectAsync().ConfigureAwait(false);
                await ReplyAsync($"Joining {Context.User.Mention} voice channel {channel.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        [Command("meme")]
        public async Task AudioTest()
        {
            try
            {
                var channel = (Context.User as IGuildUser).VoiceChannel;
                await PlaySong($"{Program.argv["DataLocation"]}\\sound\\gachi\\beep1.mp3", 0.5f, channel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task PlaySong(string song, float volume, IVoiceChannel chan)
        {
            var audioClient = await chan.ConnectAsync();
            const int blockSize = 3840;
            var buffer = new byte[blockSize];
            using (var channel = await chan.ConnectAsync())
            using (var stream = channel.CreatePCMStream(2880, bitrate: chan.Bitrate))
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments =
                    $"-i \"{song}\" " +
                    "-f s16le -ar 48000 -ac 2 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                });
                await process.StandardOutput.BaseStream.CopyToAsync(stream);
                process.WaitForExit();
            }
            /*
            var voiceStream = audioClient.CreatePCMStream(blockSize);
            while (true)
            {
                var byteCount = await process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                if (byteCount == 0)
                    break;
                buffer = AdjustVolume(buffer, volume);
                await voiceStream.WriteAsync(buffer, 0, blockSize);
            }
            */

        }

        public static unsafe byte[] AdjustVolume(byte[] audioSamples, float volume)
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
    }
}
