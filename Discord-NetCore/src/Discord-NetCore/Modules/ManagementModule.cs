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
     
        [Command("check"), Summary("Check if required binaries are available")]
        public async Task CheckApps()
        {
            try
            {
                Process process = Process.Start(new ProcessStartInfo {
                    FileName = "ffmpeg"
                });
                process.Start();
                await Context.Channel.SendMessageAsync("FFMPEG found!");
            } catch (System.IO.FileNotFoundException)
            {
                await Context.Channel.SendMessageAsync("FFMPEG not found!");
            }
            try
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = "youtube-dl"
                });
                process.Start();
                await Context.Channel.SendMessageAsync("youtube-dl found!");
            }
            catch (System.IO.FileNotFoundException)
            {
                await Context.Channel.SendMessageAsync("youtube-dl error!");
            }
        }
    }
}
