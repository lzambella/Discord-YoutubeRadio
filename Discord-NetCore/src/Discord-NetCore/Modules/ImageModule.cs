using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using NetCoreBot.Modules.facebook;
using Newtonsoft.Json;

namespace Discord_NetCore.Modules
{
    [Name("Images")]
    public class ImageModule : ModuleBase
    {
        /// <summary>
        /// Facebook API Token remove this
        /// </summary>
        readonly string token = Program.argv["FacebookToken"];
        /// <summary>
        /// MEMES posting timer (every 30 minutes)
        /// </summary>
        private Timer Timer { get; set; }
        /// <summary>
        /// Reference to the main DiscordClient objects to reduce code
        /// </summary>
        IDiscordClient client = Program.Client;

        readonly Random rand = new Random();

        /// <summary>
        /// Stores 3000 of the latest memes so we dont have to keep requesting it from facebook
        /// </summary>
        private photojson Data { get; set; }

        /// <summary>
        /// Check variable to see if there actually was a new meme
        /// </summary>
        private string LatestMeme { get; set; }


        public ImageModule()
        {
            Timer = new Timer(ImagePoster, null, 0, 6000);
            //timer = new Timer(ImagePoster2, null, 0, 5000);
        }

        [Command("thereal"), Summary("Post a random The Real image")]
        public async Task Real()
        {
            try
            {
                await Context.Channel.SendFileAsync(RandomFile($"{Program.argv["DataLocation"]}/real"));
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
        public async Task Memes()
        {
            try
            {
                // If we already loaded the json
                if (Data != null && Data.data.Any())
                {
                    var linkList = Data.data;
                    int randNum;
                    var syncLock = new object();
                    lock (syncLock)
                    {
                        randNum = rand.Next(linkList.Count);
                    }
                    var latestMemeLink = linkList[randNum].images.First().source;

                    await ReplyAsync(latestMemeLink);
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

                        await ReplyAsync(latestMemeLink);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [Command("checkmeme"), Summary("Time until a new meme is avaliable")]
        public async Task CheckMeme()
        {
            try
            {
                var timeRemaining = ((60 - DateTime.Now.Minute) % 30);
                await ReplyAsync($"Time until the next meme: `{timeRemaining}`");
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /*
        public async void ImagePoster2(object callback)
        {
            var guild = await client.GetGuildAsync(217497297880088598);
            //var voiceChannel = await guild.GetVoiceChannelAsync(215339863254368268);
            //var users = await voiceChannel.GetUsersAsync();

            //var userCount = users.Count(user => !user.IsBot);
            //if (userCount < 2)
            //return;
            var textChannel = await guild.GetTextChannelAsync(217497297880088598);
            var graphApi = new GraphApi(token, GraphApi.ApiVersion.TwoEight);
            var pageHandler = new PageHandler(graphApi, "421109484727629");
            var fields = new ApiField();
            fields.Fields.Add("images");
            var photos = await pageHandler.GetPhotos(fields, true);
            var latestPhoto = photos.PhotoNodes[0].Images[0];
            var latestLink = latestPhoto.Source;
            await textChannel.SendMessageAsync($"Here's a new meme:\n{latestLink}");
        }
        */
        public async void ImagePoster(object callback)
        {
            try
            {
                var guild = await client.GetGuildAsync(215339016755740673);
                var voiceChannel = await guild.GetVoiceChannelAsync(215339863254368268);
                var users = await voiceChannel.GetUsersAsync().Flatten();
                var userCount = users.Count(user => !user.IsBot);
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
