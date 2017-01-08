﻿using Discord;
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
        [Command("createteamchannel"), Summary("Creates a temporary voice channel that only a certain roll can enter")]
        public async Task CreateTeamChannel([Summary("Name of the channel")]string name, [Summary("Duration in minutes the channel is active for")]int duration)
        {
            var voiceChannel = new TempVoice(name, duration, Context.Guild.Id, Context);
            Program.TempVoiceChannels.Add(voiceChannel);
            await voiceChannel.CreateChannel();
            await voiceChannel.CreateRolls();
            await voiceChannel.RestrictVoiceChannel();
            await ReplyAsync("Created new channel");
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
