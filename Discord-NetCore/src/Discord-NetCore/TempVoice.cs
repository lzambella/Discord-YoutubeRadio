using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using System.Threading;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Commands;

namespace Discord_NetCore
{
    /// <summary>
    /// Temporary voice channel
    /// Creates a new private voice channel and adds users to a roll that can enter the channel
    /// The voice channel and all rolls are removed after a duration
    /// </summary>
    public class TempVoice
    {
        /// <summary>
        /// The context where the command was originally called from
        /// </summary>
        private CommandContext _context { get; set; }
        /// <summary>
        /// The name of the voice channel
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The duration in minutes the Channel will be active for
        /// </summary>
        public int Duration { get; private set; }
        /// <summary>
        /// The guild the voice channel is in
        /// </summary>
        public SocketGuild Guild { get; private set; }
        /// <summary>
        /// The actual voice channel object
        /// </summary>
        public RestVoiceChannel VoiceChannel { get; private set; }
        /// <summary>
        /// Timer that fires off the delete command, executes after [Duration] minutes.
        /// </summary>
        private Timer _desctructionTimer { get; set; }

        /// <summary>
        /// The role the voice channel is restricted to
        /// </summary>
        private RestRole _role { get; set; }
        /// <summary>
        /// TempVoice constructor
        /// </summary>
        /// <param name="name">Name of the voice channel</param>
        /// <param name="duration">Duration in minutes the channel is active for</param>
        /// <param name="guildId">ID of the guild to add the voice channel to</param>
        public TempVoice(string name, int duration, ulong guildId, CommandContext context)
        {
            _context = context;
            Name = name;
            Duration = duration;
            Guild = Program.Client.GetGuild(guildId);
            _desctructionTimer = new Timer(Destroy, null, duration * 60000, Timeout.Infinite);
        }

        /// <summary>
        /// Creates the channel
        /// </summary>
        /// <returns></returns>
        public async Task CreateChannel()
        {
            try
            {
                VoiceChannel = await Guild.CreateVoiceChannelAsync(Name);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Creates incremental team rolls starting from 1, if that exists then team 2
        /// </summary>
        /// <returns></returns>
        public async Task CreateRolls()
        {
            var teamName = "Team 1";
            var rolls = Guild.Roles;
            if (rolls.Any(r => r.Name == "Team 1"))
                teamName = "Team 2";
            if (rolls.Any(r => r.Name == "Team 2"))
                teamName = "Team 3";
            if (rolls.Any(r => r.Name == "Team 3"))
                teamName = "Team 4";
            _role = await Guild.CreateRoleAsync(teamName);
        }

        /// <summary>
        /// Restricts the voice channel to _role only
        /// </summary>
        /// <returns></returns>
        public async Task RestrictVoiceChannel()
        {
            var perms = new OverwritePermissions();
            perms.Modify(speak: PermValue.Allow, connect: PermValue.Allow);
            await VoiceChannel.AddPermissionOverwriteAsync(_role, perms);
            perms.Modify(speak: PermValue.Deny, connect: PermValue.Deny);
            var everyoneRole = _context.Guild.EveryoneRole;
            await VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, perms);
        }
        /// <summary>
        /// Adds the user to the team
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        public async Task AddPlayerToRoll(IGuildUser user, int team)
        {
            try
            {
                var role = _role;
                await user.AddRolesAsync(role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Destroys the rolls and the voice channel
        /// </summary>
        /// <param name="source"></param>
        private async void Destroy(Object source)
        {
            try
            {
                await VoiceChannel.DeleteAsync();
                await _role.DeleteAsync();
                await _context.Channel.SendMessageAsync("Voice channel expired!");
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
