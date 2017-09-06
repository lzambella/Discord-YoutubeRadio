using System;
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
        private readonly DbHandler _database = Program.Database;

        [Command("bank"), Summary("Check your points")]
        public async Task Bank()
        {
            var userId = _database.ParseString(Context.User.Mention);
            var points = await _database.GetPoints(userId);
            await ReplyAsync($"{Context.User.Mention}, you have {points} points.");
        }
        [Command("pointsleaderboard"), Summary("Check the leader boards")]
        public async Task PointLeaderboard()
        {
            try
            {
                await _database.FixConnection();
                var board = "";
                var command =
                    new SqlCommand(
                        "SELECT DiscordId, GachiPoints FROM DiscordUser ORDER BY GachiPoints DESC", Program.Database.Connection);
                board += "```";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            var users = await Context.Channel.GetUsersAsync().Flatten();
                            var username = users.Single(user => user.Id == ulong.Parse(reader[0].ToString())).Username;
                            board += $"{username} : {reader[1]} Points\n";
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
                        "SELECT DiscordId, RankLevel FROM DiscordUser ORDER BY RankLevel DESC",
                        Program.Database.Connection);
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

        [Command("add"), Summary("Add a user to the bank")]
        public async Task Add(IGuildUser mention) 
        {
            try
            {
                var database = Program.Database;
                await database.AddUser(mention);
                await ReplyAsync($"Added {mention.Mention} to the bank.");
            }
            catch (Exception)
            {
                await ReplyAsync("Error adding user to bank.");
            }
        }


    }
}