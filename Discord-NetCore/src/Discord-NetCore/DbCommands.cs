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
                Console.WriteLine("Connected to database.");
                // Start the Point Incrementation Timer
                timer = new Timer(PointIncrementer, null, 0, 60000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<int> GetPoints(string discordId)
        {
            try
            {
                await FixConnection();
                var points = 0;
                var command = new SqlCommand("SELECT GachiPoints from DiscordUser WHERE DiscordId = @id", Connection);
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



        private async void PointIncrementer(Object StateInfo)
        {
            try
            {
                if (Connection.State == System.Data.ConnectionState.Broken)
                {
                    Connection.Close();
                    await Connection.OpenAsync();
                }
                if (!await IncrementPointAlgorithm())
                {
                    Console.WriteLine($"{DateTime.Now}: Requirements not met!");
                    return;
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
                if (minute >= 30)
                {
                    var usersAsync = Program.Client.GetGuild(215339016755740673).GetVoiceChannelAsync(215339863254368268).Result.GetUsersAsync();
                    Console.WriteLine($"{DateTime.Now}: It's Time! Adding points!");
                    var users = await usersAsync.Flatten();
                    foreach (var user in users)
                    {
                        if (SkipIncrement(user))
                            continue;

                        var userId = ParseString(user.Mention);
                        var permission = await GetRank(userId);
                        var points = (int)(Math.Pow(permission, 2)) / 50;
                        if (points < 1) points = 1;
                        command =
                            new SqlCommand("UPDATE DiscordUser SET GachiPoints = GachiPoints + @points WHERE DiscordId = @id",
                                Connection);
                        command.Parameters.Add(new SqlParameter("id", userId));
                        command.Parameters.Add(new SqlParameter("points", points));
                        await command.ExecuteNonQueryAsync();
                    }
                    command = new SqlCommand("UPDATE Timer SET Minute = 0 WHERE Name = @0", Connection);
                    command.Parameters.Add(new SqlParameter("0", "PointAccumulator"));
                    await command.ExecuteNonQueryAsync();
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
        private async Task<bool> IncrementPointAlgorithm()
        {
            try
            {
                var usersAsync = Program.Client.GetGuild(215339016755740673).GetVoiceChannelAsync(215339863254368268).Result.GetUsersAsync();
                var users = await usersAsync.Flatten();
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
            return user.IsBot || user.IsSelfDeafened || user.IsSelfMuted || user.IsMuted || user.IsDeafened;
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

    }
}