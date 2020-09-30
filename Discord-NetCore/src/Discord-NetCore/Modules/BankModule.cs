using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Discord_NetCore.Modules
{
    [Name("Bank")] 
    public class BankModule : ModuleBase
    {
        private readonly string _bankTitle = $"{Program.botName}'s Credit Union";
        private readonly DbHandler _database = Program.Database;

        [Command("bank"), Summary("Check your points")]
        public async Task Bank()
        {
            try
            {
                var userId = _database.ParseString(Context.User.Mention);
                var points = await _database.GetPoints(await Context.Guild.GetUserAsync(Context.User.Id));
                var embedded = new EmbedBuilder()
                    .WithTitle(_bankTitle)
                    .WithColor(155, 165, 102)
                    .WithCurrentTimestamp()
                    .WithDescription("Check your Points")
                    .AddField($"{Context.User.Username}'s Points", $"{points}");
                await ReplyAsync($"", embed: embedded.Build());
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                await ReplyAsync("Something has gone wrong.");
            }
        }

        [Command("leaderboard"), Summary("Check who has the most points.")]
        public async Task PointLeaderboard()
        {
            try
            {
                var embedded = new EmbedBuilder()
                    .WithTitle($"{_bankTitle}")
                    .WithColor(155, 165, 102)
                    .WithDescription("See who has the most points!");
                var dict = new Dictionary<string, int>();
                var users = await Context.Guild.GetUsersAsync();
                foreach (var user in users)
                {
                    var points = await _database.GetPoints(Context.User as IGuildUser);
                    dict.Add(user.Nickname ?? user.Username, points);
                }
                int x = 0;
                foreach (var i in dict.OrderByDescending(i => i.Value))
                {
                    if (x == 4) break;
                    embedded.AddField($"{i.Key}", $"{i.Value} Points");
                    x++;
                }
                await ReplyAsync("", embed: embedded.Build());
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
            }
        }
        /*
        [Command("promote"), Summary("Spend some points to level up and get even more points.")]
        public async Task Promote()
        {
            var user = Program.Database.ParseString(Context.User.Mention);
            var points = await Program.Database.GetPoints(user);
            var permission = await Program.Database.GetRank(user);
            var requiredPoints = (int)(Math.Pow(permission + 1, 2) + 5 * (permission + 1));
            if (points < requiredPoints)
                await
                    ReplyAsync($"You don't have enough points. You need {requiredPoints} points.");
            else
            {
                var command =
                    new SqlCommand("UPDATE DiscordUser SET RankLevel = @perm WHERE DiscordId = @id",
                        Program.Database.Connection);
                command.Parameters.Add(new SqlParameter("perm", permission + 1));
                command.Parameters.Add(new SqlParameter("id", user));
                await command.ExecuteNonQueryAsync();
                await Program.Database.ChangePoints(user, requiredPoints * -1);
                await ReplyAsync($"Promoted to level {permission + 1}");
            }
        }
        */
        /*
        [Command("exchange"), Summary("Convert your points to carlin coins(TM)")]
        public async Task Exchange(string amount)
        {
            try
            {
                var points = int.Parse(amount);
                Console.WriteLine(points);
                var user = Program.Database.ParseString(Context.User.Mention);
                Console.WriteLine(user);
                if (await Program.Database.GetPoints(user) < points)
                    await ReplyAsync("You don't have that many points!");
                else
                {
                    var bot = Program.Client.GetGuild(215339016755740673).GetUser(215688935030915073);
                    var channel = await bot.CreateDMChannelAsync();
                    await channel.SendMessageAsync($"[{user} {points}]");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
        */
        [Command("rankleaderboard"), Summary("Check rank leader board")]
        public async Task Rank()
        {
            try
            {
                var board = "";
                var command =
                    new SqlCommand(
                        "SELECT DiscordId, RankLevel FROM @serverId ORDER BY Rank DESC",
                        Program.Database.Connection);
                command.Parameters.Add(new SqlParameter("serverId", Context.Guild.Id.ToString()));
                board += "```";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            board +=
                                $"{Context.Channel.GetUserAsync(ulong.Parse(reader[0].ToString())).Result.Username} : Level {reader[1]}\n";

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                board += "```";
                await ReplyAsync(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("add"), Summary("Add a user to the bank. Requires bot ownership.")]
        public async Task Add(IGuildUser mention) 
        {
            try
            {
                if (!(Context.User.Id == Program.OwnerId)) return;
                var database = Program.Database;
                if (await database.AddUser(mention))
                    await ReplyAsync($"Added {mention.Mention} to the bank.");
                else
                    await ReplyAsync("User was not added to the bank.");
            }
            catch (Exception e)
            {
#if DEBUG
                await ReplyAsync("Error adding user to bank. See logs for details");
                Console.WriteLine(e);
#endif
            }
        }


        [Command("addall"), Summary("Add the entire server to the bank.")]
        public async Task AddAll()
        {
            try
            {
                if (!(Context.User.Id == Program.OwnerId)) return;
                var users = await Context.Guild.GetUsersAsync();
                var database = Program.Database;
                foreach (var user in users)
                    await database.AddUser(await Context.Guild.GetUserAsync(user.Id));

                await ReplyAsync($"Added the server to the bank.");
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
            }
        }

        //[Command("AddToServer"), Summary("Add the current server to the bank (existing servers only)")]
        public async Task AddServerToDatabase()
        {
            if (!(Context.User.Id == Program.OwnerId)) return;
            var database = Program.Database;
            await database.AddServer(Context.Guild);
            await ReplyAsync("Server added to the database");
        }
    }
}