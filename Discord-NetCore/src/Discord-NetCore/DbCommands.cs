using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using NetCoreBot.Modules;

namespace Discord_NetCore
{
    public class DbHandler
    {
        public SqlConnection Connection { get; }
        private AutoResetEvent autoEvent { get; set; }
        private Timer timer { get; set; }
        public string ParseString(string str)
        {
            return str.Substring(str.Length == 22 ? 3 : 2, 18);
        }
        public DbHandler(string connectionString)
        {
            try
            {
                Connection = new SqlConnection(connectionString);
                Console.WriteLine($"{DateTime.Now}: Connected to database.");
                // Start the Point Incrementation Timer
                timer = new Timer(PointIncrementer, null, 6000, 60000);
            }
            catch (Exception e)
            {
                if (Program.DEBUG)
                    Console.WriteLine(e);
                Console.WriteLine("Error establishing a connection... Database commands disabled");
            }
        }
        /// <summary>
        /// Gets points from a user in a certain server
        /// </summary>
        /// <param name="discordId"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<int> GetPoints(IGuildUser user)
        {
            try
            {
                await Connection.OpenAsync();
                var points = 0;
                var command = new SqlCommand($"SELECT Points from Users WHERE DiscordId = @id AND ServerId = @serverid", Connection);
                command.Parameters.Add(new SqlParameter("id", (Int64)user.Id));
                command.Parameters.Add(new SqlParameter("serverid", (Int64)user.GuildId));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        points = int.Parse(reader[0].ToString());
                    }
                }
                Connection.Close();
                return points;
            }
            catch (Exception e)
            {
                Connection.Close();
                Console.WriteLine(e);
                return 0;
            }
        }
       /// <summary>
       /// Gets rank of someone from a server
       /// </summary>
       /// <param name="discordId"></param>
       /// <param name="serverId"></param>
       /// <returns></returns>
        public async Task<int> GetRank(IGuildUser user)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var permission = 0;
                var command = new SqlCommand("SELECT RankLevel FROM Users WHERE DiscordId = @id AND ServerId = @serverid",
                    Connection);
                command.Parameters.Add(new SqlParameter("id", (Int64)user.Id));
                command.Parameters.Add(new SqlParameter("serverId", (Int64)user.GuildId));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        permission = int.Parse(reader[0].ToString());
                }
                Connection.Close();
                return permission;
            }
            catch (Exception ex)
            {
                Connection.Close();
                Console.WriteLine(ex);
                return 0;
            }
        }
        /// <summary>
        /// Get the bot permission from a user from a server
        /// </summary>
        /// <param name="discordId"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<int> GetPermission(IGuildUser user)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var permission = 0;
                var command = new SqlCommand("SELECT PermLevel FROM Users WHERE DiscordId = @id AND ServerId = @serverid",
                    Connection);
                command.Parameters.Add(new SqlParameter("id", (Int64)user.Id));
                command.Parameters.Add(new SqlParameter("serverId", (Int64)user.GuildId));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        permission = int.Parse(reader[0].ToString());
                }
                Connection.Close();
                return permission;
            }
            catch (Exception ex)
            {
                Connection.Close();
                Console.WriteLine(ex);
                return 0;
            }
        }
        /// <summary>
        /// Sets the permission level of a user in a server
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permlevel"></param>
        /// <returns></returns>
        public async Task<int> SetPermission(IGuildUser user, int permlevel)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var command = new SqlCommand("UPDATE Users Set PermLevel = @level WHERE DiscordId = @userid AND ServerId = @serverid", Connection);
                command.Parameters.Add(new SqlParameter("level", permlevel));
                command.Parameters.Add(new SqlParameter("userid", (Int64)user.Id));
                command.Parameters.Add(new SqlParameter("serverid", (Int64)user.GuildId));
                var x = await command.ExecuteNonQueryAsync();
                Connection.Close();
                return x;
            } catch (Exception e)
            {
                Connection.Close();
                return -1;
            }
        }
        /// <summary>
        /// Directly change points from a user from a server
        /// </summary>
        /// <returns></returns>
        public async Task ChangePoints(IGuildUser user, int points)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var command =
                    new SqlCommand("UPDATE Users SET points = points + @value WHERE DiscordId = @id AND ServerId = @serverid", Connection);
                command.Parameters.Add(new SqlParameter("value", points));
                command.Parameters.Add(new SqlParameter("id", (Int64) user.Id));
                command.Parameters.Add(new SqlParameter("serverId", (Int64) user.GuildId));
                await command.ExecuteNonQueryAsync();
                Connection.Close();
            }
            catch (Exception e)
            {
                Connection.Close();
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Add a user to a server's database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> AddUser(IGuildUser user)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                // First see if the userid/serverid pair exists
                var command = new SqlCommand($"SELECT DiscordId, ServerId FROM Users WHERE DiscordId = @userid AND ServerId = @serverid", Connection);
                command.Parameters.Add(new SqlParameter("userid", (Int64) user.Id));
                command.Parameters.Add(new SqlParameter("serverid", (Int64) user.GuildId));
                var results = new List<Int64>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        results.Add(Int64.Parse(reader[0].ToString()));
                    }
                    if (results.Count > 0)
                    {
                        Connection.Close();
#if DEBUG
                        Console.WriteLine("User already exists!");
#endif
                        return false;
                    }
                }
                command.Dispose();

                // If they dont exist then continue
                command =
                    new SqlCommand(
                        $"INSERT INTO Users (DiscordId, ServerId, PermLevel, RankLevel, MessageCount, Points) VALUES (@id, @serverid, @perm, @rank, @MessageCount, @points)", Connection);
                command.Parameters.Add(new SqlParameter("id", (Int64) user.Id));
                command.Parameters.Add(new SqlParameter("serverid", (Int64) user.GuildId));
                command.Parameters.Add(new SqlParameter("perm", "0"));
                command.Parameters.Add(new SqlParameter("rank", "0"));
                command.Parameters.Add(new SqlParameter("MessageCount", "0"));
                command.Parameters.Add(new SqlParameter("Points", "0"));
                await command.ExecuteNonQueryAsync();
                Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Connection.Close();
                Console.WriteLine("Error adding user");
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Fix the shitty db connection if it breaks
        /// </summary>
        /// <returns></returns>
        public async Task FixConnection()
        {
            return;
            if (Connection.State == System.Data.ConnectionState.Broken)
            {
                Connection.Close();
                await Connection.OpenAsync();
            }
        }

        private async Task DeleteServer(string serverId)
        {
            var sql = "DELETE FROM MasterList WHERE serverId = @id";
            var command = new SqlCommand(sql, Connection);
            command.Parameters.Add(new SqlParameter("id", $"{serverId}"));
            await command.ExecuteNonQueryAsync();
        }

        private async void PointIncrementer(Object StateInfo)
        {
            try
            {
                await Connection.OpenAsync();

                if (((60 - DateTime.Now.Minute) % 10) == 0) // Every time ending in a zero will activate
                {
                    Console.WriteLine($"{DateTime.Now}: It's Time! Adding points!");
                    // Get the master server list
                    var sql = "SELECT DiscordId, ServerId FROM Users";
                    var command = new SqlCommand(sql, Connection);
                    var serverList = new List<(ulong, ulong)>();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            serverList.Add( ( ulong.Parse(reader[0].ToString()), ulong.Parse(reader[1].ToString()) ) );
                    }

                    foreach (var r in serverList)
                    {
                        try
                        {
                            var user = Program.Client.GetGuild(r.Item2).GetUser(r.Item1);
                            if (user.VoiceChannel != null)
                            {
                                if (SkipIncrement(user)) continue;
                                await ChangePoints(user, 1);
                            }                               
                        }
                        catch (NullReferenceException e)
                        {
#if DEBUG
                            Console.WriteLine(e);
                            Console.WriteLine("Error: skipping user");
#endif
                        }
                    }
                }
                Connection.Close();
            }
            catch (Exception ex)
            {
                Connection.Close();
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// Check if we will actually increment points for the users in a channel
        /// There should be at least two unmuted users who are not a bot
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        private async Task<bool> IncrementPointAlgorithm(ulong serverId)
        {
            try
            {
                var users = Program.Client.GetGuild(serverId).VoiceChannels.First().Users;
                var userCount = users.Count(user => !user.IsBot);
                //Console.WriteLine($"There are {userCount} users.");
                return userCount > 2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Console.WriteLine("Can't increment points, bot not fully initialized!");
                return false;
            }
        }

        private static bool SkipIncrement(IGuildUser user)
        {
            return user.IsBot || user.IsSelfDeafened || user.IsDeafened;
        }
        /// <summary>
        /// Get the XY Coordinates of a player
        /// </summary>
        /// <returns>Tuple that contains the x coord as item1, y coord as item2</returns>
        public async Task<Tuple<int, int>> GetCoords(string discordId)
        {
            try
            {
                var sql = "SELECT XPOS, YPOS FROM RPG WHERE DiscordID = @id";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));
                var x = 0;
                var y = 0;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Should read only once
                    await reader.ReadAsync();
                    x = int.Parse(reader[0].ToString());
                    y = int.Parse(reader[1].ToString());
                }
                return new Tuple<int, int>(x, y);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        /// <summary>
        /// Get the specified object at the coordinates
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns>List of RPGObjects containing the data</returns>
        public async Task<List<RPGObject>> ObjectAtCoord(int x, int y)
        {
            // TODO: Create the object lookup table 

            try
            {
                var objects = new List<RPGObject>();
                var sql = $"SELECT * FROM WORLD WHERE XPOS = @x AND YPOS = @y";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("x", x));
                command.Parameters.Add(new SqlParameter("y", y));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // TODO:
                        var xpos = int.Parse(reader[1] as string);
                        var ypos = int.Parse(reader[2] as string);
                        var id = int.Parse(reader[3] as string);
                        objects.Add(new RPGObject(xpos, ypos, id));
                    }
                }
                return objects;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Creates a new table for a new discord server
        /// The schema should contain the user's ID, Points, bot permission level, and possibly other statistics.
        /// </summary>
        /// <returns></returns>
        public async Task CreateTableForServer(string serverId)
        {
            try
            {
                await Connection.OpenAsync();
                var sql = $"CREATE TABLE DISCORD_SERVER_{serverId} (" +
                  $"UserId varchar(255)," +
                  $"Points int," +
                  $"PermLevel int," +
                  $"Rank int," +
                  $"MessageCount int );";
                var command = new SqlCommand(sql, Connection);
                //command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                await command.ExecuteNonQueryAsync();
                Connection.Close();
            }
            catch (Exception e)
            {
                Connection.Close();
                Console.WriteLine("Error.");
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// Checks if the table for a certain server has already been created
        /// check if it has been added to the master list already
        /// </summary>
        /// <returns>True if exists</returns>
        public async Task<Boolean> TableExists(string serverId)
        {
            var sql = $"SELECT * FROM MasterList WHERE ServerId = @serverId;";
            var command = new SqlCommand(sql, Connection);
            command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
            using (var reader = await command.ExecuteReaderAsync())
            {
                string id = "";
                while (await reader.ReadAsync())
                {
                    id = reader[0] as string;
                }
                if (id.Any()) return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the master list of servers that the bot is connected to
        /// Used to determine which servers the bot will add points to.
        /// </summary>
        /// <returns></returns>
        private async Task CreateMasterList()
        {
            try
            {
                var sql = "CREATE TABLE MasterList (" +
                          "ServerId varchar(255));";
                var command = new SqlCommand(sql, Connection);
                var result = await command.ExecuteNonQueryAsync();
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Add a server by id to the master list
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        private async Task AddToMasterList(string serverId)
        {
            try
            {
                var sql = "INSERT INTO MasterList(serverId) VALUES (@serverId);";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                var result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Adds a server to the Servers Table which allows for serverwide configuration
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns>True if the server was successfully created</returns>
        public async Task<int> AddServer(IGuild guild)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                // First check if the server already exists in the table
                var sql = "SELECT ServerId FROM Servers WHERE ServerId = @id";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("id", (Int64)guild.Id));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                        if (reader[0] != null) return -2;
                }

                sql = "INSERT INTO Servers(ServerId, BotChatChannelId, AutoMemes) VALUES (@serverid, @chatid, 0)";
                command = new SqlCommand(sql, Connection);
                var channels = await guild.GetTextChannelsAsync();
                command.Parameters.Add(new SqlParameter("serverid", (Int64)guild.Id));
                command.Parameters.Add(new SqlParameter("chatid", (Int64)channels.First().Id)); // Sets the first known text channel as the default bot channel
                var x = await command.ExecuteNonQueryAsync();
                Connection.Close();
                return x;
            } catch (Exception e)
            {
                Connection.Close();
#if DEBUG
                Console.WriteLine(e);
#endif
                return -1;
            }
        }

        /// <summary>
        /// Get the channel id that the bot should only be allowed to talk in
        /// this can be useless if the bot is only allowed access to a certain channel but acts as a alternative method
        /// Otherwise this is the channel the bot will post new memes if enabled
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<Int64> GetChatChannel(IGuild guild)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var sql = "SELECT BotChatChannelId FROM Servers WHERE ServerId = @serverid";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("serverid", (Int64)guild.Id));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    try
                    {
                        await reader.ReadAsync();
                        var x = Int64.Parse(reader[0].ToString());
                        Connection.Close();
                        return x;
                    } catch (Exception e)
                    {
                        Connection.Close();
                        Console.WriteLine(e); 
                        Console.WriteLine("Error, no data.");
                        return 0;
                    }
                }
            } catch (Exception e)
            {
                Connection.Close();
#if DEBUG
                Console.WriteLine(e);
#endif
                return -1;
            }
        }

        /// <summary>
        /// Gets the status of the automatic meme poster for a server
        /// The memes are pulled form shitpostbot2000
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<bool> GetAutoMemePosting(IGuild guild)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var sql = "SELECT AutoMemes FROM Servers WHERE ServerId = @serverid";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("serverid", guild.Id));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    var b = bool.Parse(reader[0].ToString());
                    Connection.Close();
                    return b;
                }
            } catch (Exception e)
            {
                Connection.Close();
#if DEBUG
                Console.WriteLine(e);
#endif
                return false;
            }
        }

        /// <summary>
        /// Sets whether the bot should automatically post a meme in the server
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<int> SetAutoMemePosting(IGuild guild, bool status)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var sql = $"UPDATE Servers SET AutoMemes = {(status ? 1 : 0)} WHERE ServerId = @id";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("id", (Int64)guild.Id));
                var x = await command.ExecuteNonQueryAsync();
                Connection.Close();
                return x;
            } catch (Exception e)
            {
                Connection.Close();
#if DEBUG
                Console.WriteLine(e);
#endif
                return -1;
            }
        }

        /// <summary>
        /// Sets the channel id the bot is allowed to talk in 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task<int> SetBotChatChannel(ITextChannel channel)
        {
            try
            {
                await Connection.OpenAsync();
                await FixConnection();
                var sql = "UPDATE Servers SET BotChatChannelId = @chanid WHERE ServerId = @serverid";
                var command = new SqlCommand(sql, Connection);
                command.Parameters.Add(new SqlParameter("chanid", (Int64)channel.Id));
                command.Parameters.Add(new SqlParameter("serverid", (Int64)channel.GuildId));
                var x = await command.ExecuteNonQueryAsync();
                Connection.Close();
                return x;

            } catch (Exception e)
            {
                Connection.Close();
#if DEBUG
                Console.WriteLine(e);
#endif
                return -1;
            }
        }
    }
}