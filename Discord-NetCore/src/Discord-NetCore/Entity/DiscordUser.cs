using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace NetCoreBot.Entity
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Server> Servers {get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(Environment.GetEnvironmentVariable("databaseString"));
    }

    public class User
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string GuildId {get; set;}
        public int Points {get; set; }
        public int PermLevel {get; set;}
        public int RankLevel {get; set;}

    }

    public class Server
    {
        public int Id { get; set; }
        public string GuildId {get; set;}
        public string ChannelId {get; set;}

    }
}