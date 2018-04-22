using System;
using System.Collections.Generic;
using System.Text;
using Discord_NetCore.Modules.Audio;
namespace Discord_NetCore
{
    public class Playlist
    {
        /// <summary>
        /// A list of songs
        /// each song contains a local file path and the names of the songs
        /// </summary>
        public List<Song> Songs;

        public String PlaylistName { get; set; }
        public String PlaylistDescription { get; set; }
        public String PlaylistPath { get; set; } // what is this

        public Playlist(string name, string description)
        {
            PlaylistName = name;
            PlaylistDescription = description;
            Songs = new List<Song>();
        }

        public Playlist()
        {
            PlaylistName = "Unknown";
            PlaylistDescription = "Unknown";
            Songs = new List<Song>();
        }
    }
}
