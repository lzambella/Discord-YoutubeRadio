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
                Connection.Open();
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
        /*
        public async Task<int> GetPoints(string discordId)
        {
            try
            {
                await FixConnection();
                var points = 0;
                var command = new SqlCommand("SELECT Points from DiscordUser WHERE DiscordId = @id", Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        points = int.Parse(reader[0].ToString());
                    }
                }
                return points;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }
        */
        /// <summary>
        /// Gets points from a user in a certain server
        /// </summary>
        /// <param name="discordId"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<int> GetPoints(string discordId, string serverId)
        {
            try
            {
                await FixConnection();
                var points = 0;
                var command = new SqlCommand($"SELECT Points from DISCORD_SERVER_{ serverId } WHERE UserId = @id", Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        points = int.Parse(reader[0].ToString());
                    }
                }
                return points;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        /// <param name="discordId">ID num only</param>
        /// Obsolete
        public async Task<int> GetRank(string discordId)
        {
            try
            {
                await FixConnection();
                var permission = 0;
                var command = new SqlCommand("SELECT RankLevel FROM DiscordUser WHERE DiscordId = @id",
                    Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        permission = int.Parse(reader[0].ToString());
                }
                return permission;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

       /// <summary>
       /// Gets rank of someone from a server
       /// </summary>
       /// <param name="discordId"></param>
       /// <param name="serverId"></param>
       /// <returns></returns>
        public async Task<int> GetRank(string discordId, string serverId)
        {
            try
            {
                await FixConnection();
                var permission = 0;
                var command = new SqlCommand("SELECT Rank FROM @serverId WHERE UserdId = @id",
                    Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));
                command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        permission = int.Parse(reader[0].ToString());
                }
                return permission;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public async Task<int> GetPermission(string discordId)
        {
            try
            {
                await FixConnection();
                var permission = 0;
                var command = new SqlCommand("SELECT PermissionLevel FROM DiscordUser WHERE DiscordId = @id",
                    Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        permission = int.Parse(reader[0].ToString());
                }
                return permission;
            }
            catch (Exception ex)
            {
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
        public async Task<int> GetPermission(string discordId, string serverId)
        {
            try
            {
                await FixConnection();
                var permission = 0;
                var command = new SqlCommand("SELECT PermLevel FROM @serverId WHERE UserId = @id",
                    Connection);
                command.Parameters.Add(new SqlParameter("id", discordId));
                command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        permission = int.Parse(reader[0].ToString());
                }
                return permission;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public async Task ChangePoints(string discordId, int value)
        {
            try
            {
                await FixConnection();
                var command =
                    new SqlCommand("UPDATE DiscordUser SET GachiPoints = GachiPoints + @value WHERE DiscordId = @id", Connection);
                command.Parameters.Add(new SqlParameter("value", value));
                command.Parameters.Add(new SqlParameter("id", discordId));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Directly change points from a user from a server
        /// </summary>
        /// <param name="discordId"></param>
        /// <param name="serverId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task ChangePoints(string discordId, string serverId, int value)
        {
            try
            {
                await FixConnection();
                var command =
                    new SqlCommand("UPDATE @serverId SET GachiPoints = GachiPoints + @value WHERE DiscordId = @id", Connection);
                command.Parameters.Add(new SqlParameter("value", value));
                command.Parameters.Add(new SqlParameter("id", discordId));
                command.Parameters.Add(new SqlParameter("serverId", serverId));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /*
        public async Task AddUser(IGuildUser user)
        {
            try
            {
                await FixConnection();
                var command =
                    new SqlCommand(
                        "INSERT INTO DiscordUser(DiscordId, GachiPoints, RankLevel, PermissionLevel, DiscordGuild) VALUES (@id, @points, @rank, @permission, @guild)");
                command.Parameters.Add(new SqlParameter("id", user.Id));
                command.Parameters.Add(new SqlParameter("points", 0));
                command.Parameters.Add(new SqlParameter("rank", 0));
                command.Parameters.Add(new SqlParameter("permission", 0));
                command.Parameters.Add(new SqlParameter("guild", user.GuildId));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error adding user");
                Console.WriteLine(e.StackTrace);
            }
        }
        */
        /// <summary>
        /// Add a user to a server's database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task AddUser(IGuildUser user, string serverId)
        {
            try
            {
                await FixConnection();
                var command =
                    new SqlCommand(
                        $"INSERT INTO DISCORD_SERVER_{serverId} (UserId, Points, PermLevel, Rank, MessageCount) VALUES (@id, @points, @rank, @permission, @MessageCount)", Connection);
                //command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                command.Parameters.Add(new SqlParameter("id", user.Id.ToString()));
                command.Parameters.Add(new SqlParameter("points", "0"));
                command.Parameters.Add(new SqlParameter("rank", "0"));
                command.Parameters.Add(new SqlParameter("permission", "0"));
                command.Parameters.Add(new SqlParameter("MessageCount", "0"));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error adding user");
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Fix the shitty db connection if it breaks
        /// </summary>
        /// <returns></returns>
        public async Task FixConnection()
        {
            if (Connection.State == System.Data.ConnectionState.Broken)
            {
                Connection.Close();
                await Connection.OpenAsync();
            }
        }


        // TODO: update the points for all people in all servers
        // Perhaps create flags for each server that trigger how users get points
        // Also make it check the current minute from the system clock instead of keeping track in a database (old method for when the bot restarted every 5 minutes)
        private async void PointIncrementer(Object StateInfo)
        {
            try
            {
                if (Connection.State == System.Data.ConnectionState.Broken)
                {
                    Connection.Close();
                    await Connection.OpenAsync();
                }

                var minute = 0;
                var command = new SqlCommand("SELECT Minute FROM Timer WHERE Name = @0", Connection);
                command.Parameters.Add(new SqlParameter("0", "PointAccumulator"));
                //Console.WriteLine("Adding a minute!");
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                        minute = int.Parse(reader[0].ToString());
                }
                if (minute >= 10)
                {
                    // Get the master server list
                    var sql = "SELECT * FROM MasterList";
                    command = new SqlCommand(sql, Connection);
                    var serverList = new List<string>();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            serverList.Add(reader[0].ToString());
                    }

                    foreach (var serverId in serverList)
                    {
                        var fixedId = ulong.Parse(serverId.Substring(15));
                        var channels = Program.Client.GetGuild(fixedId).VoiceChannels;
                        var users = new List<IGuildUser>();
                        foreach (var channel in channels)
                        {
                            foreach (var user in channel.Users)
                            {
                                users.Add(user); //ineffecient code
                            }
                        }
                        Console.WriteLine($"{DateTime.Now}: It's Time! Adding points!");
                        foreach (var user in users)
                        {
                            if (SkipIncrement(user))
                                continue;

                            var userId = ParseString(user.Mention);
                            //var permission = await GetRank(userId);
                            var points = 1;
                            if (points < 1) points = 1;
                            command =
                                new SqlCommand($"UPDATE DISCORD_SERVER_{fixedId} SET Points = Points + @points WHERE UserId = @id",
                                    Connection);
                            //command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                            command.Parameters.Add(new SqlParameter("id", userId));
                            command.Parameters.Add(new SqlParameter("points", points));
                            await command.ExecuteNonQueryAsync();
                        }
                        command = new SqlCommand("UPDATE Timer SET Minute = 0 WHERE Name = @0", Connection);
                        command.Parameters.Add(new SqlParameter("0", "PointAccumulator"));
                        await command.ExecuteNonQueryAsync(); 
                    }
                }
                else
                {
                    command = new SqlCommand("UPDATE Timer SET Minute = Minute + 1 WHERE Name = @0", Connection);
                    command.Parameters.Add(new SqlParameter("0", "PointAccumulator"));
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
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
                var sql = $"CREATE TABLE DISCORD_SERVER_{serverId} (" +
                  $"UserId varchar(255)," +
                  $"Points int," +
                  $"PermLevel int," +
                  $"Rank int," +
                  $"MessageCount int );";
                var command = new SqlCommand(sql, Connection);
                //command.Parameters.Add(new SqlParameter("serverId", $"DISCORD_SERVER_{serverId}"));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
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
        /// Creates a new server entry by firsting adding the id to the master list and then creating a new table for that server
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns>True if the server was successfully created</returns>
        public async Task<Boolean> AddServer(string serverId)
        {
            try
            {
                await CreateTableForServer(serverId);
                await AddToMasterList(serverId);
                return true;
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}