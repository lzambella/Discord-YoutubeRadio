using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules.Audio
{
    public class Song
    {
        public string Url { get; private set; }
        public string Title { get; private set; }
        public string Length { get; private set; }
        private string[] Parameters { get; set; }

        public Song(string url)
        {
            Parameters = url.Split('&');
            Url = Parameters[0];
        }

        /// <summary>
        /// Gets the video info at the URL
        /// </summary>
        /// <returns></returns>
        public async Task GetVideoInfo()
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl",
                Arguments = $"-e --get-duration {Url} ",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false
            });
            try
            {
                var streamReader = new StreamReader(process.StandardOutput.BaseStream);
                var title = await streamReader.ReadLineAsync();
                var duration = await streamReader.ReadLineAsync();
                streamReader.Dispose();
                process.Dispose();
                Title = title;
                Length = duration;
            }
            catch (Exception)
            {
                Title = "unknown";
                Length = "unknown";
            }
        }

    }
}
