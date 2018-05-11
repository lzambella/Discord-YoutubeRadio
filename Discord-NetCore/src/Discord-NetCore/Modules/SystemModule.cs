using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Discord_NetCore.Modules
{
    [Name("System")]
    public class SystemModule : ModuleBase
    {
        //[Command("memory"), Summary("View avaliable memory")]
        public async Task GetInfo()
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "/usr/bin/free",
                    Arguments = "-h"
                }
            };
            process.Start();
            var stdout = await process.StandardOutput.ReadToEndAsync();
            string output = $"```{stdout}```";
            await ReplyAsync(output);
        }

        [Command("uptime"), Summary("Print how long the bot has been online")]
        public async Task uptime()
        {
            var time = (DateTime.Now - Process.GetCurrentProcess().StartTime);
            await ReplyAsync($"I have been online for {time.ToString("hh:mm:ss")}");
        }

        //[Command("sysinfo"), Summary("Print system information")]
        public async Task hw()
        {
            await Program.Client.SetStatusAsync(UserStatus.Invisible);
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
                var embedded = new EmbedBuilder()
                            .WithTitle(commandName)
                            .WithDescription(commandSummary)
                            .AddField("Usage", $"!{commandName} {paramString}")
                            .AddField("Aliases", $"{commandAlias}");
                await ReplyAsync($"",embed:embedded);
            }
            else
            {
                var embedded = new EmbedBuilder()
                    .WithTitle("Gachi's Helpdesk.")
                    .WithFooter("Type !help <command name> for more info.");
                foreach (var module in modules)
                {
                    var commandNames = module.Commands.ToList();
                    var builder = new StringBuilder();
                    foreach (var command in commandNames)
                        builder.Append($"{command.Name} ");

                    embedded.AddField($"{module.Name}", $"{builder}");
                }
                await ReplyAsync("", embed:embedded);

            }
        }
    }
}
