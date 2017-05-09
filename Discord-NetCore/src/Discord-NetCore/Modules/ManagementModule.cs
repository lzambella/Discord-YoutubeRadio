using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules
{
    [Name("Management")]
    public class ManagementModule : ModuleBase
    {
        //[Command("createteamchannel"), Summary("Creates a temporary voice channel that only a certain roll can enter")]
        public async Task CreateTeamChannel([Summary("Name")]string name, [Summary("Duration in minutes")]int duration)
        {
        }
     
        //[Command("mute"), Summary("Server mute someone")]
        public async Task Mute([Summary("Mention")]IUser user)
        {
            await ReplyAsync($"Muting {user.Mention}");
            var voiceChannel = (user as IGuildUser)?.VoiceChannel ?? null;
            if (voiceChannel == null) return;
            var voiceUser = await voiceChannel.GetUserAsync(user.Id);
            Action<GuildUserProperties> propAction = delegate (GuildUserProperties g)
            {
                g.Mute = true;
            };
            await voiceUser.ModifyAsync(propAction);
        }
        //[Command("unmute"), Summary("Unmute a punished noob")]
        public async Task Unmute([Summary("Mention")]IUser user)
        {
            var guildUser = Context.User as IGuildUser;
            
            var voiceChannel = (user as IGuildUser)?.VoiceChannel ?? null;
            if (voiceChannel == null) return;
            var voiceUser = await voiceChannel.GetUserAsync(user.Id);
            Action<GuildUserProperties> propAction = delegate (GuildUserProperties g)
            {
                g.Mute = false;
            };
            await voiceUser.ModifyAsync(propAction);
        }
        [Command("testbin")]
        public async Task CheckApps()
        {
            try
            {
                Process process = Process.Start(new ProcessStartInfo {
                    FileName = "ffmpeg"
                });
                process.Start();
                Console.WriteLine("FFMPEG found!");
            } catch (Exception e)
            {
                Console.WriteLine("FFMPEG error!");
            }
            try
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = "youtube-dl"
                });
                process.Start();
                Console.WriteLine("youtube-dl found!");
            }
            catch (Exception e)
            {
                Console.WriteLine("youtube-dl error!");
            }
        }
    }
}
