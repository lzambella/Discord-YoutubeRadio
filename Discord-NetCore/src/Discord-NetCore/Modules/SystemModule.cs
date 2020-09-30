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

        [Command("uptime"), Summary("Print how long the bot has been online")]
        public async Task uptime()
        {
            var time = (DateTime.Now - Process.GetCurrentProcess().StartTime);
            await ReplyAsync($"I have been online for {time.ToString("hhmmss")}");
        }

        [Command("help"), Summary("Prints help message")]
        public async Task Help([Summary("(Optional) Command Name")]string c = null)
        {
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
                await ReplyAsync($"",embed:embedded.Build());
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
                await ReplyAsync("", embed:embedded.Build());

            }
        }

        [Command("AddServer"), Summary("Adds the current server to the database. Operator only")]
        public async Task AddServer()
        {
            try
            {
                if (Context.User.Id == Program.OwnerId || await Program.Database.GetRank(Context.User as IGuildUser) == 5)
                {
                    var x = await Program.Database.AddServer(Context.Guild);
                    if (x != -1) await ReplyAsync("Added this server to the master server list. Now set the bot channel.");
                    else if (x == -2) await ReplyAsync("This server has already been added");
                    else if (x == -1) await ReplyAsync("Error. Something has gone wrong");
                }
            } catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                await ReplyAsync("Error adding the server");
            }
        }

        [Command("SetChannel"), Summary("Sets the current channel as the channel the bot will work in. Operator Only")]
        public async Task SetBotChannel()
        {
            if (Context.User.Id == Program.OwnerId || await Program.Database.GetRank(Context.User as IGuildUser) == 5)
            {
                var x = await Program.Database.SetBotChatChannel(Context.Channel as ITextChannel);
                if (x != -1) await ReplyAsync("I will only respond in this channel from now on");
                else await ReplyAsync("Error. Something has gone wrong");
            }
        }
    }
}
