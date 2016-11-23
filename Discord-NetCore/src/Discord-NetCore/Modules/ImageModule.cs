using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using NetCoreBot.Modules.facebook;
using System.Threading;

namespace NetCoreBot.Modules
{
    [Module, Name("Images")]
    public class ImageModule
    {
        /// <summary>
        /// Facebook API Token remove this
        /// </summary>
        string token = Program.argv["FacebookToken"];
        /// <summary>
        /// MEMES posting timer (every 30 minutes)
        /// </summary>
        Timer timer { get; set; }
        /// <summary>
        /// Reference to the main DiscordClient objects to reduce code
        /// </summary>
        IDiscordClient client = Program.Client;
        Random rand = new Random();
        /// <summary>
        /// Stores 3000 of the latest memes so we dont have to keep requesting it from facebook
        /// </summary>
        photojson Data { get; set; }
        /// <summary>
        /// Check variable to see if there actually was a new meme
        /// </summary>
        string LatestMeme { get; set; }

        public ImageModule()
        {
            var timeToHalf = ( (60 - DateTime.Now.Minute) % 30 ) * 10000;
            timer = new Timer(ImagePoster, null, timeToHalf, 60000 * 30);
        }

        [Command("thereal"), Summary("Post a random The Real image")]
        public async Task Real(IUserMessage msg)
        {
            try
            {
                await msg.Channel.SendFileAsync(RandomFile($"{Program.argv["DataLocation"]}/real"));
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private string RandomFile(string directory)
        {
            var rand = new Random();
            var files = Directory.GetFiles(directory);
            return files[rand.Next(files.Length)];
        }

        [Command("randomeme"), Summary("Get a random randomly generated meme")]
        public async Task Memes(IUserMessage msg)
        {
            try
            {
                // If we already loaded the json
                if (Data.data.Count() > 0)
                {
                    var linkList = Data.data;
                    int randNum;
                    object syncLock = new object();
                    lock (syncLock)
                    {
                        randNum = rand.Next(linkList.Count);
                    }
                    var latestMemeLink = linkList[randNum].images.First().source;

                    await msg.Channel.SendMessageAsync(latestMemeLink);
                    return;
                }

                // Loads the json content into a variable for quicker access
                var url = $"https://graph.facebook.com/v2.5/421109484727629/photos?access_token={token}&pretty=0&fields=images&type=uploaded&limit=3000";
                var request = WebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                var response = (HttpWebResponse)await request.GetResponseAsync();
                if (response != null)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var json = await sr.ReadToEndAsync();
                        var data = JsonConvert.DeserializeObject<photojson>(json);
                        var linkList = data.data;
                        Console.WriteLine(linkList.Count());
                        int randNum;
                        object syncLock = new object();
                        lock (syncLock)
                        {
                            randNum = rand.Next(linkList.Count);
                        }
                        Console.WriteLine(randNum);
                        var latestMemeLink = linkList[randNum].images.First().source;

                        await msg.Channel.SendMessageAsync(latestMemeLink);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [Command("checkmeme"), Summary("Time until a new meme is avaliable")]
        public async Task CheckMeme(IUserMessage msg)
        {
            try
            {
                var timeRemaining = ((60 - DateTime.Now.Minute) % 30);
                await msg.Channel.SendMessageAsync($"Time until the next meme: `{timeRemaining}`");
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async void ImagePoster(object callback)
        {
            try
            {
                var guild = await client.GetGuildAsync(215339016755740673);
                var voiceChannel = await guild.GetVoiceChannelAsync(215339863254368268);
                var users = await voiceChannel.GetUsersAsync();

                var userCount = users.Where(user => !user.IsBot).Count();
                if (userCount < 2)
                    return;
                var textChannel = await guild.GetTextChannelAsync(215339016755740673);
                var url = $"https://graph.facebook.com/v2.5/421109484727629/photos?access_token={token}&pretty=0&fields=images&type=uploaded&limit=2";
                var request = WebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                var response = (HttpWebResponse)await request.GetResponseAsync();
                var js = "";
                if (response != null)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        js = await sr.ReadToEndAsync();
                    }
                    var data = JsonConvert.DeserializeObject<photojson>(js);
                    var latestMemeLink = data.data.First().images.First().source;

                    // Skip posting the meme if there was no new meme
                    if (latestMemeLink.Equals(LatestMeme))
                        return;

                    LatestMeme = latestMemeLink;
                    await textChannel.SendMessageAsync($"Here's a new meme:\n{latestMemeLink}");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
