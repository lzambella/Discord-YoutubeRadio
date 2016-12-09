using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Discord_NetCore.Modules 
{
    [Name("Audio")]
    public class Audio : ModuleBase
    {
        public Audio()
        {
            Console.WriteLine("Initialized Audio Module");
        }
        private IAudioClient _vClient = Program.Audio;
        // Create a Join command, that will join the parameter or the user's current voice channel
        [Command("joinchannel")]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            try
            {

                await ReplyAsync("Joining voice channel...");
                // Get the audio channel
                channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
                if (channel == null) { await ReplyAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
                // Get the IAudioClient by calling the JoinAsync method
                Program.Audio = await channel.ConnectAsync();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        [Command("meme")]
        public async Task AudioTest(IUserMessage msg)
        {
            try
            {
                await PlaySong($"{Program.argv["DataLocation"]}\\sound\\Motivation\\smart.mp3", 1.0f);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task PlaySong(string song, float volume)
        {
            const int blockSize = 3840;
            var buffer = new byte[blockSize];
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments =
                    $"-i \"{song}\" " +
                    "-f s16le -ar 48000 -ac 2 pipe:1 -loglevel quiet",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            while (true)
            {
                var byteCount = await process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);
                if (byteCount == 0)
                    break;
                buffer = AdjustVolume(buffer, volume);
                
            }

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
