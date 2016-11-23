using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Commands;
using System.Data.SqlClient;
using System;
using System.Linq;

namespace NetCoreBot.Modules
{
    [Module, Name("Bank")]
    public class BankModule
    {
        DbHandler Database = Program.Database;

        [Command("bank"), Summary("Check your points")]
        public async Task Bank(IUserMessage msg)
        {
            var userId = Database.ParseString(msg.Author.Mention);
            var points = await Database.GetPoints(userId);
            await msg.Channel.SendMessageAsync($"{msg.Author.Mention}, you have {points} points.");
        }
        [Command("pointsleaderboard"), Summary("Check the leader boards")]
        public async Task PointLeaderboard(IUserMessage msg)
        {
            try
            {
                await Database.FixConnection();
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
                            var users = await msg.Channel.GetUsersAsync();
                            var username = users.Single(user => user.Id == ulong.Parse(reader[0].ToString())).Username.ToString();
                            board += $"{username} : {reader[1]} Points\n";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                board += "```";
                await msg.Channel.SendMessageAsync(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        [Command("promote"), Summary("Spend some points to level up and get even more points.")]
        public async Task Promote(IUserMessage msg)
        {
            var user = Program.Database.ParseString(msg.Author.Mention);
            var points = await Program.Database.GetPoints(user);
            var permission = await Program.Database.GetRank(user);
            var requiredPoints = (int)(Math.Pow(permission + 1, 2) + 5 * (permission + 1));
            if (points < requiredPoints)
                await
                    msg.Channel.SendMessageAsync($"You don't have enough points. You need {requiredPoints} points.");
            else
            {
                var command =
                    new SqlCommand("UPDATE DiscordUser SET RankLevel = @perm WHERE DiscordId = @id",
                        Program.Database.Connection);
                command.Parameters.Add(new SqlParameter("perm", permission + 1));
                command.Parameters.Add(new SqlParameter("id", user));
                await command.ExecuteNonQueryAsync();
                await Program.Database.ChangePoints(user, requiredPoints * -1);
                await msg.Channel.SendMessageAsync($"Promoted to level {permission + 1}");
            }
        }
        [Command("exchange"), Summary("Convert your points to carlin coins(TM)")]
        public async Task exchange(IUserMessage msg, string amount)
        {
            try
            {
                var points = Int32.Parse(amount);
                Console.WriteLine(points);
                var user = Program.Database.ParseString(msg.Author.Mention);
                Console.WriteLine(user);
                if (await Program.Database.GetPoints(user) < points)
                    await msg.Channel.SendMessageAsync("You don't have that many points!");
                else
                {
                    var bot = await Program.Client.GetGuildAsync(215339016755740673).Result.GetUserAsync(215688935030915073);
                    var channel = await bot.CreateDMChannelAsync();
                    await channel.SendMessageAsync($"[{user} {points}]");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
        [Command("rankleaderboard"), Summary("Check rank leader board")]
        public async Task Rank(IUserMessage msg)
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
                                $"{msg.Channel.GetUserAsync(ulong.Parse(reader[0].ToString())).Result.Username} : Level {reader[1]}\n";

                        }
                        catch (Exception ex)
                        {
                            // Console.WriteLine("User does not exist!");
                        }
                    }
                }
                board += "```";
                await msg.Channel.SendMessageAsync(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


    }
}