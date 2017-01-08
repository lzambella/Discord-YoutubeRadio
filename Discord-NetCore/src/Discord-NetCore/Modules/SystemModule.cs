using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Discord_NetCore.Modules
{
    [Name("System")]
    public class SystemModule : ModuleBase
    {
        [Command("memory"), Summary("View avaliable memory")]
        public async Task GetInfo()
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "/usr/bin/free";
            process.StartInfo.Arguments = "-h";
            process.Start();
            var stdout = await process.StandardOutput.ReadToEndAsync();
            string output = $"```{stdout}```";
            await ReplyAsync(output);
        }

        [Command("uptime"), Summary("Print how long the bot has been online")]
        public async Task uptime()
        {
            var time = DateTime.Now - Process.GetCurrentProcess().StartTime;
            await ReplyAsync($"I have been online for {time.Days}:{time.Hours}:{time.Minutes}:{time.Seconds}! [DD:HH:MM:SS]");
        }

        [Command("sysinfo"), Summary("Print system information")]
        public async Task hw()
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "/usr/bin/lshw";
            process.StartInfo.Arguments = "-short";
            process.Start();
            var stdout = await process.StandardOutput.ReadToEndAsync();
            string output = $"```{stdout}```";
            await ReplyAsync(output);
        }

        [Command("tail"), Summary("Print last 20 lines of the log file (admin only)")]
        public async Task tail()
        {
            if (Context.User.Id == Program.OwnerId)
            {
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = "/usr/bin/tail";
                process.StartInfo.Arguments = "-n 20 /home/ubuntu/nohup.out";
                process.Start();
                var stdout = await process.StandardOutput.ReadToEndAsync();
                string output = $"```{stdout}```";
                await ReplyAsync(output);
            }
        }
        [Command("help"), Summary("Prints help message")]
        public async Task Help([Summary("(Optional) Command Name")]string c = null)
        {
            var str = "";
            var modules = Program.commands.Modules;
            var commands = Program.commands.Commands;
            if (c != null)
            {
                var command = commands.Single(com => com.Name.Equals(c));
                if (command == null)
                {
                    await ReplyAsync("Unknown command");
                    return;
                }
                var commandName = command.Name;
                var commandSummary = command.Summary;
                var commandParameters = command.Parameters;
                var commandAlias = string.Join(",", command.Aliases);
                var paramString = "";
                foreach (var parameter in commandParameters)
                    paramString += $"<{parameter.Summary}> ";
                await ReplyAsync($"Name: `{commandName}`\n" +
                                 $"Description: `{commandSummary}`\n" +
                                 $"Usage: `!{commandName} {paramString}`\n" +
                                 $"Aliases: `{commandAlias}`");
            }
            else
            {
                foreach (var module in modules)
                {
                    str += $"{module.Name}: ";
                    foreach (var command in module.Commands)
                        str += $"{command.Name} ";
                    str += '\n';
                }
                await ReplyAsync("```\n" +
                                 "Commands\n" +
                                 $"{str}\n" +
                                 "Type !help <command> for more info.```");

            }
        }
    }
}
