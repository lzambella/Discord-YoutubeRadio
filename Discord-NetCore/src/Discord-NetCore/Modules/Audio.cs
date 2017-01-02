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
        Process Process { get; set; }

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
            var audioPlayer = GetMusicPlayerForGuild();
            var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
            if (voiceChannel == null || audioPlayer == null)           
                await ReplyAsync("You are not currently in a voice channel.");            
            else if (audioPlayer.ConnectedChannel == null)           
                await ReplyAsync("I am currently not in a voice channel.");           
            else await audioPlayer.StreamYoutube(url);
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
                await ReplyAsync("Error! Is anything even playing?");
                Console.WriteLine(e);
            }
        }

        [Command("parrot", RunMode = RunMode.Async)]
        public async Task Parrot()
        {
            try
            {
                await ReplyAsync("Testing audio input!");

            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
