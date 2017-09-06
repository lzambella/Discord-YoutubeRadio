using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using NetCoreBot;

namespace Discord_NetCore.Modules
{
    //[Name("RPG")]
    public class RpgModule : ModuleBase
    {
        [Command("register"), Summary("Register a new character")]
        public async Task Register(IUserMessage msg, [Summary("Name of your character")]string name = null)
        {
            try
            {
                if (name == null)
                {
                    await msg.Channel.SendMessageAsync("No name specified.");
                }
                else
                {
                    var id = Program.Database.ParseString(msg.Author.Mention);
                    var command = new SqlCommand("SELECT COUNT(DiscordId) FROM RPG WHERE DiscordId = @id", Program.Database.Connection);
                    command.Parameters.Add(new SqlParameter("id", id));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (int.Parse(reader[0].ToString()) > 0)
                            {
                                await msg.Channel.SendMessageAsync("You already registered! Type `!stats` to view your stats.");
                                return;
                            }
                        }
                    }

                    command = new SqlCommand("INSERT INTO RPG (DiscordId, RPGName, RoomID, XPOS, YPOS, Money, InventoryID, EquipmentID, Level, XP) VALUES (@id, @name, 0, 0, 0, 0, 0, 0, 0, 0);", Program.Database.Connection);

                    command.Parameters.Add(new SqlParameter("id", id));
                    command.Parameters.Add(new SqlParameter("name", name));

                    await command.ExecuteNonQueryAsync();
                    await msg.Channel.SendMessageAsync("Successfully created a new character~");
                }
            } catch (Exception e)
            {
                await msg.Channel.SendMessageAsync("An error has ocurred");
                Console.WriteLine(e);
            }
        }
        [Command("stats"), Summary("Check your stats")]
        public async Task CheckStats(IUserMessage msg)
        {
            try
            {

                var id = Program.Database.ParseString(msg.Author.Mention);

                var name = "";
                var level = "";
                var xpos = "";
                var ypos = "";
                var money = "";
                var xp = "";
                var command = new SqlCommand("SELECT * FROM RPG WHERE DiscordID = @id", Program.Database.Connection);
                command.Parameters.Add(new SqlParameter("id", id));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        name = reader[2].ToString();
                        level = reader[9].ToString();
                        xpos = reader[4].ToString();
                        ypos = reader[5].ToString();
                        money = reader[6].ToString();
                        xp = reader[10].ToString();
                    }
                }
                var output = $"Name: {name}\n" +
                             $"Level: {level}\n" +
                             $"X-Coords: {xpos}\n" + 
                             $"Y-Coords: {ypos}\n" +
                             $"Money: {money}\n" +
                             $"XP: {xp}\n";

                await msg.Channel.SendMessageAsync($"```{output}```");
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [Command("move"), Summary("Move in a direction")]
        public async Task Move(IUserMessage msg, [Summary("N/S/E/W")]string direction = null)
        {
            try
            {
                if (direction == null)
                {
                    await msg.Channel.SendMessageAsync("Please enter a direction");
                }
                else
                {
                    var id = Program.Database.ParseString(msg.Author.Mention);
                    var d = char.ToUpper(direction[0]);
                    var coords = await Program.Database.GetCoords(id);
                    var x = coords.Item1;
                    var y = coords.Item2;
                    SqlCommand command;
                    switch (d)
                    {
                        case 'N':
                            if (y == 21)
                                return;
                            command = new SqlCommand("UPDATE RPG SET YPOS = YPOS + 1 WHERE DiscordId = @id", Program.Database.Connection);
                            command.Parameters.Add(new SqlParameter("id", id));
                            await command.ExecuteNonQueryAsync();
                            await msg.Channel.SendMessageAsync("You move 1 space north.");
                            break;
                        case 'S':
                            if (y == 0)
                                return;
                            command = new SqlCommand("UPDATE RPG SET YPOS = YPOS - 1 WHERE DiscordId = @id", Program.Database.Connection);
                            command.Parameters.Add(new SqlParameter("id", id));
                            await command.ExecuteNonQueryAsync();
                            await msg.Channel.SendMessageAsync("You move 1 space south.");
                            break;
                        case 'E':
                            if (x == 21)
                                return;
                            command = new SqlCommand("UPDATE RPG SET XPOS = XPOS + 1 WHERE DiscordId = @id", Program.Database.Connection);
                            command.Parameters.Add(new SqlParameter("id", id));
                            await command.ExecuteNonQueryAsync();
                            await msg.Channel.SendMessageAsync("You move 1 space east.");
                            break;
                        case 'W':
                            if (x == 0)
                                return;
                            command = new SqlCommand("UPDATE RPG SET XPOS = XPOS - 1 WHERE DiscordId = @id", Program.Database.Connection);
                            command.Parameters.Add(new SqlParameter("id", id));
                            await command.ExecuteNonQueryAsync();
                            await msg.Channel.SendMessageAsync("You move 1 space west.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("map"), Summary("Check your position on the map.")]
        public async Task Map(IUserMessage msg)
        {
            try
            {
                var xBox = 500;
                var yBox = 500;
                // TODO:
                //await msg.Channel.SendFileAsync($"map.png");
            } catch (Exception ex)
            {

            }
        }

        /*
        [Command("inventory"), Summary("Check your inventory")]
        public async Task Inventory(IUserMessage msg)
        {
            //TODO: Add a cache of inventory IDs
            try
            {
                var id = Program.Database.ParseString(msg.Author.Mention);

                var command = new SqlCommand("SELECT * FROM INVENTORY WHERE DiscordID = @id", Program.Database.Connection);
                command.Parameters.Add(new SqlParameter("id", id));
                var itemId = new List<string>(); // ???
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // There are 5 item spots starting at reader[2];
                        for (var x = 2; x < 7; x++)
                            itemId.Add(reader[x].ToString());
                    }
                }

                var param = "";

                // Get all item ids and add it to the parameters such that (item1,item2,item3)
                for (var x = 0; x < itemId.Count; x++)
                {
                    // Final param shouldnt have a comma
                    if (x == itemId.Count - 1)
                        param += itemId[x];
                    else
                        param += itemId[x] + ",";
                }
                var sql = $"SELECT * FROM ITEMS WHERE id IN ({param});";
                command = new SqlCommand(sql, Program.Database.Connection);
                var output = "";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var count = itemId.Where(item => item == reader[0].ToString()).Count();
                        // For loop for duplicates
                        for (var i = 0; i < count; ++i)
                            output += $"{reader[1]}: atk:{reader[3]} def:{reader[4]} value: {reader[5]}\n";
                    }
                }
                var guildUser = msg.Author as IGuildUser;
                var pmChannel = await guildUser.CreateDMChannelAsync();
                await pmChannel.SendMessageAsync($"```{output}```");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        */
        [Command("check"), Summary("Check what's around you")]
        public async Task Check(IUserMessage msg)
        {
            try
            {
                var playerXCoord = 0;
                var playerYCoord = 0;
                var id = Program.Database.ParseString(msg.Author.Mention);

                var sql = $"SELECT XCoord, YCoord from RPG WHERE DiscordID = @id";
                var command = new SqlCommand(sql, Program.Database.Connection);
                command.Parameters.Add(new SqlParameter("id", id));

                var coords = await Program.Database.GetCoords(id);
                playerXCoord = coords.Item1;
                playerYCoord = coords.Item2;
                // TODO: Check what's around you

            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
