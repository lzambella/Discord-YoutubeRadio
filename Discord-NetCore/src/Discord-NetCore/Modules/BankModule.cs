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
                await ReplyAsync($"", embed: embedded);
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
                await ReplyAsync("", embed: embedded);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
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