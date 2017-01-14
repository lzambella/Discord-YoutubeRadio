using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using Discord_NetCore.Modules.Audio;
using System.IO;

namespace Discord_NetCore.Modules
{
    [Name("Audio")]
    public class AudioModule : ModuleBase
    {
        private MusicPlayer _musicPlayer { get; set; }
        public AudioModule()
        {
            var guildId = Context.Guild.Id;
            if (Program.MusicPlayers.ContainsKey(guildId))
                _musicPlayer = Program.MusicPlayers[guildId];
        }
        // Create a Join command, that will join the parameter or the user's current voice channel
        [Command("joinchannel", RunMode = RunMode.Async), Alias("join", "j", "voice"), Summary("Joins the voice channel the user is in")]
        public async Task JoinChannel()
        {
            try
            {
                var channel = (Context.User as IGuildUser)?.VoiceChannel;
                var guildId = Context.Guild.Id;
                if (channel == null) { await ReplyAsync("You must be in a voice channel for me to join"); return; }

                if (!Program.MusicPlayers.ContainsKey(guildId))
                {
                    Program.MusicPlayers.Add(guildId, new MusicPlayer(Context));
                    await Program.MusicPlayers[guildId].MoveToVoiceChannel(channel);
                    await ReplyAsync($"Joining {Context.User.Mention}'s voice channel: {channel.Name}");
                }
                else
                {
                    await Program.MusicPlayers[guildId].MoveToVoiceChannel(channel);
                    await ReplyAsync($"Moving to {Context.User.Mention}'s voice channel: {channel.Name}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [Command("gachimuchi"), Summary("Play a random gachimuchi sound effect"), Alias("gachi", "g")]
        public async Task Gachimuchi()
        {
            if (_musicPlayer == null)
                await ReplyAsync("I am not in a voice channel");
            else
            {
                var files = Directory.GetFiles("./Data/sfx");
                var rand = (int) DateTime.Now.ToFileTimeUtc() % files.Length;
                await _musicPlayer.PlaySong(files[rand]);
            }
        }
        [Command("skip"), Summary("Skip the currently running song")]
        public async Task SkipSongAsync()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.SkipSong();
            await ReplyAsync("Skipping song!");
        }
        [Command("youtube", RunMode = RunMode.Async), Summary("Stream a youtube video"), Alias("y","stream")]
        public async Task Youtube(string url)
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
                if (voiceChannel == null || audioPlayer == null)
                    await ReplyAsync("You are not currently in a voice channel.");
                else if (audioPlayer.ConnectedChannel == null)
                    await ReplyAsync("I am currently not in a voice channel.");
                else
                {
                    await audioPlayer.AddToQueue(url);
                    await ReplyAsync("Added the song to the queue.");
                }
                if (audioPlayer.AutoPlay && audioPlayer.AudioFree)
                {
                    await RunQueue();
                }
            }
            catch (AudioStreamInUseException a)
            {

                await ReplyAsync(a.Message);
            }
        }
        [Command("shuffle"), Summary("Shuffle the current queue")]
        public async Task Shuffle()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.TruffleShuffle();
            await ReplyAsync("Shuffled the queue!");
        }
        [Command("playqueue", RunMode = RunMode.Async), Summary("Plays the queue"), Alias("run", "start")]
        public async Task RunQueue()
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                audioPlayer.RunQueue();
            } catch (AudioStreamInUseException)
            {
                await ReplyAsync("Something is already playing!");
            }
        }
        [Command("pause", RunMode = RunMode.Async), Summary("Pause the audio stream"), Alias("p")]
        public async Task PauseStream()
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                if (audioPlayer.Paused)
                    await ReplyAsync("It's already paused!");
                else
                {
                    audioPlayer.TogglePause();
                    await ReplyAsync("Stream paused!");
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        [Command("resume", RunMode = RunMode.Async), Summary("Resume a paused song"), Alias("r")]
        public async Task Resume()
        {
            try
            {

                var audioPlayer = GetMusicPlayerForGuild();
                if (!audioPlayer.Paused)
                    await ReplyAsync("It's already playing or its set to play!");
                else
                {
                    audioPlayer.TogglePause();
                    await ReplyAsync("Stream resumed!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }
        [Command("queue", RunMode = RunMode.Async), Summary("Prints the current queue"), Alias("q","check")]
        public async Task CheckQueue()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            if (audioPlayer.GetQueue().Length == 0) await ReplyAsync("Nothing in queue!");
            else await ReplyAsync($"```{audioPlayer.GetQueue()}```");
        }

        [Command("autoplay"), Summary("Toggle autoplay"), Alias("a")]
        public async Task AutoPlay()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.AutoPlay = !audioPlayer.AutoPlay;
            if (!audioPlayer.AutoPlay)
                await ReplyAsync("Autoplay Disabled!");
            else await ReplyAsync("Autoplay enabled!");

        }
        [Command("stop", RunMode = RunMode.Async), Summary("Stops the audio player and clerars the queue")]
        public async Task StopAudio()
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                if (audioPlayer == null) return;
                await ReplyAsync("Stopping all audio...");
                audioPlayer.StopAudio();
            } catch (Exception e)
            {
                //await ReplyAsync(a.Message);
            }
        }

        [Command("volume", RunMode = RunMode.Async), Summary("Change the colume"), Alias("v", "vol")]
        public async Task ChangeVolume([Summary("Volume 0-100")] int vol)
        {
            if (vol > 100) vol = 100;
            if (vol < 0) vol = 0;
            float volume = vol / 100f;
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.Volume = volume;
            await ReplyAsync($"Setting the volume to {vol}!");
        }
        [Command("skip")]
        public async Task SkipSong()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.SkipSong();
            await ReplyAsync("Skipping the current song!");
        }

        private MusicPlayer GetMusicPlayerForGuild()
        {
            var guildId = Context.Guild.Id;
            if (Program.MusicPlayers.ContainsKey(guildId))
                return Program.MusicPlayers[guildId];
            return null;
        }
    }
}
