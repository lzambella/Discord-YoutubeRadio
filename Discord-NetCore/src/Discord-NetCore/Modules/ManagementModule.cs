using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules
{
    [Name("Management")]
    public class ManagementModule : ModuleBase
    {
        //[Command("createteamchannel"), Summary("Creates a temporary voice channel that only a certain roll can enter")]
        public async Task CreateTeamChannel([Summary("Name")]string name, [Summary("Duration in minutes")]int duration)
        {
        }
        /*
        [Command("mute"), Summary("Server mute an annoying retard")]
        public async Task Mute([Summary("Mention")]IUser user)
        {
            await ReplyAsync($"Muting {user.Mention}");
            var voiceChannel = (user as IGuildUser)?.VoiceChannel ?? null;
            if (voiceChannel == null) return;
            var voiceUser = await voiceChannel.GetUserAsync(user.Id);
            Action<GuildUserProperties> propAction = delegate (GuildUserProperties g)
            {
                g.Mute = true;
            };
            await voiceUser.ModifyAsync(propAction);
        }
        [Command("unmute"), Summary("Unmute a punished noob")]
        public async Task Unmute([Summary("Mention")]IUser user)
        {
            var guildUser = Context.User as IGuildUser;
            
            var voiceChannel = (user as IGuildUser)?.VoiceChannel ?? null;
            if (voiceChannel == null) return;
            var voiceUser = await voiceChannel.GetUserAsync(user.Id);
            Action<GuildUserProperties> propAction = delegate (GuildUserProperties g)
            {
                g.Mute = false;
            };
            await voiceUser.ModifyAsync(propAction);
        }
        */

    }
}
