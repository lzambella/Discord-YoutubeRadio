using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Discord;
using NetCoreBot.Modules;
using NetCoreBot.Entity;
using System.Linq;

namespace Discord_NetCore
{
    public class DbHandler
    {
        public SqlConnection Connection { get; }
        private Timer timer { get; set; }
        public string ParseString(string str)
        {
            return str.Substring(str.Length == 22 ? 3 : 2, 18);
        }
        public DbHandler(string connectionString)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now}: Connected to database.");
                // Start the Point Incrementation Timer
                // 1 minute interval
                timer = new Timer(60000);
                timer.Elapsed += PointIncrementer;
                timer.AutoReset = true;
                timer.Enabled = true;

                Connection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                if (Program.DEBUG)
                    Console.WriteLine(e);
                Console.WriteLine("Error establishing a connection... Database commands disabled");
            }
        }
        public async Task<int> GetPoints(IGuildUser user)
        {
            try
            {
                using (var db = new UserContext()) {
                    var u = await db.Users.AsAsyncEnumerable().Where(x => x.GuildId == user.GuildId.ToString() && x.DiscordId == user.Id.ToString()).FirstOrDefaultAsync();
                    if (u == null)
                        return 0;

                    return u.Points;
                }
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
        /// Directly change points from a user from a server
        /// </summary>
        /// <returns></returns>
        public async Task ChangePoints(IGuildUser user, int points)
        {
            try
            {
                using (var db = new UserContext()) {
                    var u = await db.Users.AsAsyncEnumerable().Where(x => x.GuildId == user.GuildId.ToString() && x.DiscordId == user.Id.ToString()).FirstOrDefaultAsync();
                    if (u == null)
                        return;

                    u.Points = points;
                    db.Update(u);
                    await db.SaveChangesAsync();
                }
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
                using (var db = new UserContext()) {
                    var u = await db.Users.AsAsyncEnumerable().Where(x => x.GuildId == user.GuildId.ToString() && x.DiscordId == user.Id.ToString()).FirstOrDefaultAsync();
                    if (u == null) {
                        var newUser = new User();
                        newUser.DiscordId = user.Id.ToString();
                        newUser.GuildId = user.GuildId.ToString();
                        newUser.Points = 0;
                        newUser.RankLevel = 0;
                        newUser.PermLevel = 0;
                        await db.AddAsync(newUser);
                        await db.SaveChangesAsync();
                        return true;
                    } else {
                        return false;
                    }                   
                }
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

        private async void PointIncrementer(object source, ElapsedEventArgs e)
        {
            try
            {
                Console.WriteLine("Points");
                if (((DateTime.Now.Minute) % 5) == 0) // Every time ending in a zero or 5 will activate
                {
                    Console.WriteLine($"{DateTime.Now}: It's Time! Adding points!");
                    using (var db = new UserContext()) {
                        // get all users
                        var users = await db.Users.AsAsyncEnumerable().ToListAsync();

                        // Check if that user is currently in a voice channel
                        foreach (var user in users) {
                            try {
                                var guild = Program.Client.GetGuild(ulong.Parse(user.GuildId));
                                var guildUser = guild.GetUser(ulong.Parse(user.DiscordId));
                                if (guildUser.VoiceChannel != null) {
                                    // if the user is in a voice channel then increment their points
                                    // take in account for their rank as well
                                    // so the formula would be (rank + 1) * 2
                                    user.Points = user.Points + (user.RankLevel + 1) * 2;
                                    await db.SaveChangesAsync();
                                }
                            } catch (Exception ex) {
                                Console.WriteLine("Something went wrong, skipping user...");
                                Console.WriteLine(ex);
                            }
                            

                        }
                    }

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
                using (var db = new UserContext()) {
                    var thisServer = await db.Servers.AsAsyncEnumerable().FirstOrDefaultAsync(x => x.GuildId == guild.Id.ToString());

                    if (thisServer == null) {
                        Console.WriteLine("Err. Server not in database");
                        return -1;
                    } else {
                        Console.WriteLine(thisServer.ChannelId);
                        return Int64.Parse(thisServer.ChannelId);
                    }
                }
            } catch (Exception e)
            {
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
                using (var db = new UserContext()) {
                    var server = await db.Servers.AsAsyncEnumerable().Where(x => x.GuildId == channel.GuildId.ToString()).FirstOrDefaultAsync();
                    if (server == null) {
                        // If the server is not in the database then add it
                        var newServer = new Server();
                        newServer.ChannelId= channel.Id.ToString();
                        newServer.GuildId = channel.GuildId.ToString();
                        await db.AddAsync(newServer);
                        await db.SaveChangesAsync();
                        return 0;
                    } else {
                        server.GuildId = channel.GuildId.ToString();
                        server.ChannelId = channel.Id.ToString();
                        await db.SaveChangesAsync();
                        return 0;
                    }
                }
            } catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                return -1;
            }
        }
    }
}