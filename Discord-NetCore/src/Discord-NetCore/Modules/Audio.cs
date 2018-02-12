using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using Discord_NetCore.Modules.Audio;
using System.IO;
using System.Linq;

namespace Discord_NetCore.Modules
{
    [Name("Audio")]
    public class AudioModule : ModuleBase
    {

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
        /*
        [Command("annoy"), Summary("Play a random sound effect"), Alias("sfx", "g")]
        public async Task Gachimuchi()
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                if (audioPlayer == null)
                    await ReplyAsync("I am not in a voice channel");
                else
                {
                    var files = Directory.GetFiles("Data/sfx/");
                    var rand = DateTime.Now.ToFileTimeUtc() % files.Length;
                    Console.WriteLine($"Selected {files[rand]}");
                    await audioPlayer.PlaySong(files[(int)rand]);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        */
        [Command("custom"), Summary("plays a direct song.")]
        public async Task PlayCustomSong(string path)
        {
            try
            {
                var audioPlayer = GetMusicPlayerForGuild();
                if (audioPlayer == null)
                    await ReplyAsync("I am not in a channel!");
                else
                {
                    Console.WriteLine("Trying to play a song");
                    await ReplyAsync("Playing a custom song...");
                    await audioPlayer.AddFileToQueue(path, Context, true);
                    if (audioPlayer.AutoPlay && audioPlayer.AudioFree)
                    {
                        await RunQueue();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [Command("skip", RunMode = RunMode.Async), Summary("Skip the currently running song")]
        public async Task SkipSongAsync()
        {
            await ReplyAsync("Skipping song!");
            var audioPlayer = GetMusicPlayerForGuild();
            await audioPlayer.SkipSong(Context);
        }
        [Command("youtube", RunMode = RunMode.Async), Summary("Stream a youtube video"), Alias("y", "stream")]
        public async Task Youtube(string url)
        {
            var audioPlayer = GetMusicPlayerForGuild();
            var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
            if (voiceChannel == null || audioPlayer == null)
                await ReplyAsync("You are not currently in a voice channel.");
            else if (audioPlayer.ConnectedChannel == null)
                await ReplyAsync("I am currently not in a voice channel.");
            else
            {
                await audioPlayer.AddToQueue(url, Context);
                await Context.Message.DeleteAsync();
                await ReplyAsync("Added the song to the queue.");
            }
            if (audioPlayer.AutoPlay && audioPlayer.AudioFree)
            {
                await RunQueue();
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
            }
            catch (AudioStreamInUseException)
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [Command("resume", RunMode = RunMode.Async), Summary("Resume a paused song"), Alias("r")]
        public async Task Resume()
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
        [Command("queue", RunMode = RunMode.Async), Summary("Prints the current queue"), Alias("q", "check")]
        public async Task CheckQueue()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            if (audioPlayer.GetQueue().Length == 0) await ReplyAsync("Nothing in queue!");
            else await ReplyAsync($"```{audioPlayer.GetQueue()}```");
        }

        //[Command("autoplay"), Summary("Toggle autoplay"), Alias("a")]
        public async Task AutoPlay()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            audioPlayer.AutoPlay = !audioPlayer.AutoPlay;
            if (!audioPlayer.AutoPlay)
                await ReplyAsync("Autoplay Disabled!");
            else await ReplyAsync("Autoplay enabled!");

        }
        [Command("stop", RunMode = RunMode.Async), Summary("Stops the current song.")]
        public async Task StopAudio()
        {
            if (Context.User.Id != Program.OwnerId)
            {
                await ReplyAsync("You can't stop the audio");
                return;
            }

            var audioPlayer = GetMusicPlayerForGuild();
            if (audioPlayer == null) return;
            await ReplyAsync("Stopping the queue...");
            audioPlayer.StopAudio();
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
            await audioPlayer.SkipSong(Context);
        }
        //[Command("repeat", RunMode = RunMode.Async)]
        public async Task Repeater()
        {
            var audioPlayer = GetMusicPlayerForGuild();
            await audioPlayer.RepeatAudio();
            await ReplyAsync("I'm listening");
        }
        [Command("upload", RunMode = RunMode.Async)]
        public async Task UploadSong()
        {
            try
            {

                var musicPlayer = GetMusicPlayerForGuild();
                if (Context.Message.Attachments.Any())
                {
                    var url = Context.Message.Attachments.First().Url;
                    await musicPlayer.AddFileToQueue(url, Context, false);
                    await ReplyAsync("Recieved a custom song");
                    if (musicPlayer.AudioFree == true)
                        musicPlayer.RunQueue();
                }
                else
                {
                    await ReplyAsync("You didn't upload anything");
                }
            }
            catch (NullReferenceException)
            {
                await ReplyAsync("I am not in a voice channel!");
            }
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
