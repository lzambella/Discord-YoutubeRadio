using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_NetCore.Modules.Audio
{
    public class Song
    {
        public bool IsFile { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Length { get; set; }
        private string[] Parameters { get; set; }
        public short SkipVotes { get; set; }
        public string DirectLink { get; set; }

        //public ICommandContext RequestedBy { get; private set; }
        public ulong RequestedBy { get; set; }
        public List<ulong> UsersVoted { get; set; }

        public Song()
        {
            IsFile = false;
            Url = "unknown";
            Title = "unknown";
            Length = "unknown";
            DirectLink = "unknown";
        }
        public Song(string url)
        {
            Parameters = url.Split('&');
            Url = Parameters[0];
        }
        public Song(string url, ICommandContext requestedBy)
        {
            Parameters = url.Split('&');
            Url = Parameters[0];
            RequestedBy = requestedBy.User.Id;
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
                DirectLink = url;
            }

            RequestedBy = requestedBy.User.Id;
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
                        FileName = "youtube-dl",
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
                // hack to check whether is youtube link or not, this breaks other sites like soundcloud
                if (!Url.ToUpper().Contains("YOUTUBE"))
                {
                    DirectLink = Url;
                    return;
                }
                // hacky way to get the direct link if youtube
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {

                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "./Binaries/youtube-dl.exe",
                        //Arguments = $"-x -g \"{Url}\" ",
                        Arguments = $" -f worstaudio -g \"{Url}\" ",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = false
                    });
                }
                else
                {
                    process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "youtube-dl",
                        //Arguments = $"-x -g \"{Url}\" ",
                        Arguments = $" -f worstaudio -g \"{Url}\" ",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = false
                    });
                }
                streamReader = new StreamReader(process.StandardOutput.BaseStream);
                DirectLink = await streamReader.ReadLineAsync();
                Console.WriteLine(DirectLink);
            }
            catch (Exception)
            {
                Title = "unknown";
                Length = "unknown";
                DirectLink = "unknown";
            }
        }

    }
}
