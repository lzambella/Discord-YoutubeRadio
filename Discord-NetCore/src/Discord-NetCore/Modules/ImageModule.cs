using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace Discord_NetCore.Modules
{
    //[Name("Images")]
    public class ImageModule : ModuleBase
    {
        /// <summary>
        /// Facebook API Token remove this
        /// </summary>
        readonly string _token = Program.argv["FacebookToken"];

        //[Command("thereal"), Summary("Post a random The Real image")]
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
        /*
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
                    var time = DateTime.Now.ToFileTime();
                    randNum = (int)(time) % 3000;
                    Console.WriteLine(randNum);
                    var latestMemeLink = linkList[randNum].images.First().source;

                    await ReplyAsync(latestMemeLink);
                    return;
                }

                // Loads the json content into a variable for quicker access
                var url = $"https://graph.facebook.com/v2.5/421109484727629/photos?access_token={_token}&pretty=0&fields=images&type=uploaded&limit=3000";
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
                        var time = DateTime.Now.ToFileTime();
                        randNum = (int) Math.Abs((time / 100)%3000);
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
        */
        //[Command("checkmeme"), Summary("Time until a new meme is avaliable")]
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
    }
}
