using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
namespace NetCoreBot.Modules
{
    [Module, Name("System")]
    public class SystemModule
    {
        [Command("memory"), Summary("View avaliable memory")]
        public async Task GetInfo(IUserMessage msg)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "/usr/bin/free";
            process.StartInfo.Arguments = "-h";
            process.Start();
            var stdout = await process.StandardOutput.ReadToEndAsync();
            string output = $"```{stdout}```";
            await msg.Channel.SendMessageAsync(output);
        }

        [Command("uptime"), Summary("Print how long the bot has been online")]
        public async Task uptime(IUserMessage msg)
        {
            var time = DateTime.Now - Process.GetCurrentProcess().StartTime;
            await msg.Channel.SendMessageAsync($"I have been online for {time.Days}:{time.Hours}:{time.Minutes}:{time.Seconds}! [DD:HH:MM:SS]");
        }

        [Command("sysinfo"), Summary("Print system information")]
        public async Task hw(IUserMessage msg)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "/usr/bin/lshw";
            process.StartInfo.Arguments = "-short";
            process.Start();
            var stdout = await process.StandardOutput.ReadToEndAsync();
            string output = $"```{stdout}```";
            await msg.Channel.SendMessageAsync(output);
        }

        [Command("tail"), Summary("Print last 20 lines of the log file (admin only)")]
        public async Task tail(IUserMessage msg)
        {
            if (msg.Author.Id == Program.OwnerId)
            {
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = "/usr/bin/tail";
                process.StartInfo.Arguments = "-n 20 /home/ubuntu/nohup.out";
                process.Start();
                var stdout = await process.StandardOutput.ReadToEndAsync();
                string output = $"```{stdout}```";
                await msg.Channel.SendMessageAsync(output);
            }
        }
        [Command("help"), Summary("Prints help message")]
        public async Task Help(IUserMessage msg,[Summary("(Optional) Command Name")]string c = null)
        {
            var str = "";
            var modules = Program.commands.Modules;
            var commands = Program.commands.Commands;
            if (c != null)
            {
                var command = commands.Single(com => com.Text.Equals(c));
                if (command == null)
                {
                    await msg.Channel.SendMessageAsync("Unknown command");
                    return;
                }
                var commandName = command.Text;
                var commandSummary = command.Summary;
                var commandParameters = command.Parameters;
                var paramString = "";
                foreach (var parameter in commandParameters)
                    paramString += $"<{parameter.Summary}> ";
                await msg.Channel.SendMessageAsync($"Name: `{commandName}`\nDescription: `{commandSummary}`\n" +
                                                   $"Usage: `!{commandName} {paramString}`");
            }
            else
            {
                foreach (var module in modules)
                {
                    str += $"{module.Name}: ";
                    foreach (var command in module.Commands)
                        str += $"`{command.Text}` ";
                    str += '\n';
                }
                await msg.Channel.SendMessageAsync("These are the commands you can use:\n" +
                                                  $"{str}\n" +
                                                   "Type `!help <command>` for more info.");

            }
        }
    }
}
