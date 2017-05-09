using Discord.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules.Audio
{
    public class Song
    {
        public bool IsFile { get; private set; }
        public string Url { get; private set; }
        public string Title { get; private set; }
        public string Length { get; private set; }
        private string[] Parameters { get; set; }
        public ICommandContext RequestedBy { get; private set; }

        public Song(string url)
        {
            Parameters = url.Split('&');
            Url = Parameters[0];
        }
        public Song(string url, ICommandContext requestedBy)
        {
            Parameters = url.Split('&');
            Url = Parameters[0];
            RequestedBy = requestedBy;
        }
        public Song(string url, ICommandContext requestedBy, bool file)
        {
            IsFile = file;
            if (!file)
            {
                Parameters = url.Split('&');
                Url = Parameters[0];
            }
            else
            {
                Parameters = url.Split('\\');
                Title = Parameters[Parameters.Count() - 1];
                Url = url;
            }

            RequestedBy = requestedBy;
        }
        /// <summary>
        /// Gets the video info at the URL
        /// </summary>
        /// <returns></returns>
        public async Task GetVideoInfo()
        {
            try
            {
                Process process;
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "./Binaries/youtube-dl.exe",
                        Arguments = $"-e --get-duration {Url} ",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = false
                    });
                }
                else
                {
                    var files = Directory.GetFiles(Directory.GetCurrentDirectory());
                    foreach (var file in files)
                    {
                        Console.WriteLine(file);
                    }
                    Console.WriteLine();
                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "~/vendor/youtube-dl/youtube-dl",
                        Arguments = $"-e --get-duration {Url} ",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = false,
                    });
                }
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
