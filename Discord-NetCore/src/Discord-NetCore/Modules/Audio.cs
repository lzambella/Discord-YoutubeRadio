using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using Discord_NetCore.Modules.Audio;
using System.Collections.Generic;

namespace Discord_NetCore.Modules
{
    [Name("Audio")]
    public class AudioModule : ModuleBase
    {
        //private MusicPlayer AudioPlayer = Program.Player;

        //private Dictionary<ulong, MusicPlayer> MusicPlayers = Program.MusicPlayers;
        /// <summary>
        /// A Process
        /// </summary>
        private Process Process { get; set; }

        // Create a Join command, that will join the parameter or the user's current voice channel
        [Command("joinchannel", RunMode = RunMode.Async)]
        public async Task JoinChannel()
        {
            try
            {
                var channel = (Context.User as IGuildUser)?.VoiceChannel;
                var guildId = Context.Guild.Id;
                if (channel == null) { await ReplyAsync("You must be in a voice channel for me to join"); return; }

                if (!Program.MusicPlayers.ContainsKey(guildId))
                {
                    Program.MusicPlayers.Add(guildId, new MusicPlayer());
                    await Program.MusicPlayers[guildId].MoveToVoiceChannel(channel);
                    await ReplyAsync($"Joining {Context.User.Mention}'s voice channel: {channel.Name}");
                }
                else
                {
                    await Program.MusicPlayers[guildId].MoveToVoiceChannel(channel);
                    await ReplyAsync($"Moving to {Context.User.Mention}'s voice channel: {channel.Name}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("youtube", RunMode = RunMode.Async)]
        public async Task Youtube(string url)
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
                if (voiceChannel == null || audioPlayer == null)
                    await ReplyAsync("You are not currently in a voice channel.");
                else if (audioPlayer.ConnectedChannel == null)
                    await ReplyAsync("I am currently not in a voice channel.");
                else
                {
                    await audioPlayer.AddToQueue(url);
                    await ReplyAsync("Added the song to the queue.");
                }
                if (audioPlayer.AutoPlay)
                    await RunQueue();
            }
            catch (AudioStreamInUseException a)
            {

                await ReplyAsync(a.Message);
            }
        }
        [Command("play", RunMode = RunMode.Async), Summary("Plays the queue")]
        public async Task RunQueue()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            await audioPlayer.RunQueue(Context);
        }
        [Command("togglepause", RunMode = RunMode.Async), Summary("Pause/play the audio stream"), Alias("p")]
        public async Task PauseStream()
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                audioPlayer.TogglePause();
                if (audioPlayer.Paused)
                    await ReplyAsync("Audio stream paused!");
                else await ReplyAsync("Audio stream resumed!");
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        [Command("checkqueue", RunMode = RunMode.Async)]
        public async Task CheckQueue()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            await ReplyAsync($"```{audioPlayer.GetQueue()}```");
        }
        [Command("autoplay"), Summary("Toggle autoplay")]
        public async Task AutoPlay()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.AutoPlay = !audioPlayer.AutoPlay;
            if (!audioPlayer.AutoPlay)
                await ReplyAsync("Autoplay Disabled!");
            else await ReplyAsync("Autoplay enabled!");

        }
        [Command("stop", RunMode = RunMode.Async)]
        public async Task StopAudio()
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                if (audioPlayer == null) return;
                await ReplyAsync("Stopping all audio...");
                audioPlayer.StopAudio();
            } catch (Exception e)
            {
                //await ReplyAsync(a.Message);
            }
        }

        [Command("volume", RunMode = RunMode.Async)]
        public async Task ChangeVolume([Summary("Volume 0-100")] int vol)
        {
            if (vol > 100) vol = 100;
            if (vol < 0) vol = 0;
            float volume = vol / 100f;
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.Volume = volume;
            await ReplyAsync($"Setting the volume to {vol}!");
        }

        private MusicPlayer GetMusicPlayerForGuild()
        {
            var guildId = Context.Guild.Id;
            if (Program.MusicPlayers.ContainsKey(guildId))
                return Program.MusicPlayers[guildId];
            return null;
        }
    }
}
