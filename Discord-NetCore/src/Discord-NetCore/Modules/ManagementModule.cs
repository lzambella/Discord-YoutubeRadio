using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules
{
    [Name("Management"), Group("operator")]
    public class ManagementModule : ModuleBase
    {
       
        [Command("check"), Summary("Check if required binaries are available")]
        public async Task CheckApps()
        {
            if (!IsOperator()) return;
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

        private bool IsOperator()
        {
            return Context.User.Id == Program.OwnerId;
        }
    }

}
