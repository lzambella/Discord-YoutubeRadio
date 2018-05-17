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
        public async Task Uptime()
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

            // Update the server-sided dictionary
            if (Program.BotChatChannel.Any(i => i.Key == Context.Guild.Id))
            {
                Program.BotChatChannel.Remove(Context.Guild.Id);
                Program.BotChatChannel.Add(Context.Guild.Id, Context.Channel.Id);
            }
            else
                Program.BotChatChannel.Add(Context.Guild.Id, Context.Channel.Id);
        }

        [Command("SetPerm"), Summary("Set the bot permission level of someone.")]
        public async Task SetPermission([Summary("The user to change the perm level of")]IUser mention, [Summary("Permission level (1-5)")]int permlevel)
        {
            try
            {
                var userPerm = await Program.Database.GetPermission(Context.User as IGuildUser);
                if (permlevel == 5 || Context.User.Id == Program.OwnerId)
                {
                    var p = permlevel;
                    if (permlevel > 5) p = 5;
                    else if (permlevel < 1) p = 1;

                    var user = await Context.Guild.GetUserAsync(mention.Id);
                    var x = await Program.Database.SetPermission(user, p);
                    if (x != -1) await ReplyAsync("User's permission has been set");
                    else await ReplyAsync("User's permission has not been set");
                }
            } catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
            }
        }
        [Command("check"), Summary("Check if required binaries are available")]
        public async Task CheckApps()
        {
            if (!IsOperator()) return;
            try
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg"
                });
                process.Start();
                await Context.Channel.SendMessageAsync("FFMPEG found!");
            }
            catch (System.IO.FileNotFoundException)
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
