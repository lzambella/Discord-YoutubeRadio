using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules.Audio
{
    /// <summary>
    /// Class for a single song, mostly for youtube
    /// </summary>
    public class Song
    {
        public string Url { get; private set; }
        public string Title { get; private set; }
        public string Length { get; private set; }

        public Song(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Gets the video info at the URL
        /// </summary>
        /// <returns></returns>
        public async Task GetVideoInfo()
        {
            var process = Process.Start(new ProcessStartInfo
            {
                // add way to execute bash if on linux
                FileName = "youtube-dl",
                Arguments = $"-e --get-duration {Url} ",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false
            });
            var streamReader = new StreamReader(process.StandardOutput.BaseStream);
            var title = await streamReader.ReadLineAsync();
            var duration = await streamReader.ReadLineAsync();
            streamReader.Dispose();
            process.Dispose();
            Title = title;
            Length = duration;
        }

    }
}
