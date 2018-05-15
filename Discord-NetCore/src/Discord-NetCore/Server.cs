using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Discord_NetCore
{
    public class ServerContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerMember> Members { get; set; }

    }

    public class Server
    {
        public ulong ServerId { get; set; }
        public ulong BotChatChannelId { get; set; }
        public bool AutoMemes { get; set; }
    }

    public class ServerMember
    {
        public ulong DiscordId { get; set; }
        public ulong ServerId { get; set; }
        public int PermLevel { get; set; }
        public int RankLevel { get; set; }
        public int MessageCount { get; set; }
    }
}
