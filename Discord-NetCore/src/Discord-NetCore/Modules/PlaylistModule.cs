using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Linq;
using Discord_NetCore.Modules.Audio;
using System.Xml.Serialization;
using System.IO;

namespace Discord_NetCore.Modules
{
    //[Name("Playlist")]
    public class PlaylistModule : ModuleBase
    {
        /// <summary>
        /// List playlists by how they are loadad
        /// </summary>
        /// <returns></returns>
        [Command("ListPlaylists"), Summary("List all the loaded playlists")]
        public async Task ListPlaylists()
        {
            try
            {
                var output = "`";
                int i = 0;
                Program.Playlists.ForEach(song => {
                    output += $"{i} : {song.PlaylistName}\n";
                    i++;
                });
                output += "`";
                await ReplyAsync(output);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("ListSongs"), Summary("List all the songs in a playlist")]
        public async Task ListSongs([Summary("Index of the playlist")] int index)
        {
            try
            {
                var output = "'";
                int i = 0;
                Program.Playlists[index].Songs.ForEach(song =>
                {
                    output += $"{i} : {song.Title}\n";
                    i++;
                });
                output += "'";
                await ReplyAsync(output);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Playlist not found.");
            }
        }
        /// <summary>
        /// Add a song to a loaded playlist by its index
        /// </summary>
        /// <returns></returns>
        [Command("AddSong"), Summary("Add a new song to the playlist")]
        public async Task AddSong([Summary("URL of the song, whether a local file link or a youtube link.")] string location, [Summary("The index of the playlist")] int index)
        {
            try
            {
                var song = new Song(location, Context);
                await song.GetVideoInfo();
                Program.Playlists[index].Songs.Add(song);
                await ReplyAsync(
                    $"Added the song.\n" +
                    $"Name: {song.Title}.\n" +
                    $"Playlist: {Program.Playlists[index].PlaylistName}.\n");
                await SavePlaylist(); // very ineffecient
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Playlist not found.");
            }
        }

        /// <summary>
        /// Create a new playlist
        /// </summary>
        /// <returns></returns>
        [Command("CreatePlaylist"), Summary("Create a new playlist")]
        public async Task AddPlaylist([Summary("Playlist Name.")] string name, [Summary("Playlist Description")] string description)
        {
            Program.Playlists.Add(new Playlist(name, description));
            await ReplyAsync("Created a new playlist.");
        }
        /// <summary>
        /// Save a playlist by its loaded id
        /// </summary>
        //[Command("SavePlaylist"), Summary("Save a playlist")]
        public async Task SavePlaylist()
        {
            ///TODO: Serialize a playlist into an xml by its index number
            try
            {
                var serializer = new XmlSerializer(typeof(Playlist));
                StreamWriter writer;
                foreach (var playlist in Program.Playlists)
                {
                    writer = new StreamWriter($"Playlists\\{playlist.PlaylistName}.xml");
                    serializer.Serialize(writer, playlist);
                    writer.Dispose();
                }
                //await ReplyAsync("Successfully saved the playlist.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("PlayPlaylist", RunMode = RunMode.Async), Summary("Play a playlist.")]
        public async Task RunPlaylist([Summary("Index of the playlist to use")] int index)
        {
            try
            {


                MusicPlayer player;
                var guildId = Context.Guild.Id;
                if (Program.MusicPlayers.ContainsKey(guildId))
                {
                    player = Program.MusicPlayers[guildId];
                    Program.Playlists[index].Songs.ForEach(async song =>
                    {
                        await player.AddToQueue(song.Url, Context);
                    });
                    await ReplyAsync(
                        $"Playlist added to queue.\n" +
                        $"Playlist Name: {Program.Playlists[index].PlaylistName}.");
                    return;
                }
                await ReplyAsync("I am not connected to a voice channel.");
            } catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Wrong index specified");
            }
        }

    }
}
