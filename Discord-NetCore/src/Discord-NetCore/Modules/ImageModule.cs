using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using FacebookSharp;
using static FacebookSharp.GraphAPI.ApiParameters.PhotoField;
using FacebookSharp.GraphAPI.ApiParameters;
using FacebookSharp.GraphAPI;

namespace Discord_NetCore.Modules
{
    [Name("Images")]
    public class ImageModule : ModuleBase
    {

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
        [Command("meme"), Summary("Gets the latest randomly generated meme.")]
        public async Task MeMe()
        {
            try
            {
                var token = Program.FacebookToken;
                // Get the latest meme and post it to the general chat
                var graphApi = new GraphApi(token, GraphApi.ApiVersion.TwoEight);
                var page = await graphApi.GetPage("1663308127217572");
                var fields = new PhotoField();
                fields.Fields.Add(PhotoFields.Source);
                fields.Fields.Add(PhotoFields.Images);
                var images = await page.GetPhotos(fields, true);
                var meme = images.PhotoNodes.First().Images.First().Source;
                var embedded = new EmbedBuilder()
                    .WithTitle($"{Program.botName}'s funky memes")
                    .WithImageUrl(meme)
                    .WithFooter($"New meme in T-minus {((60 - DateTime.Now.Minute) % 30)} minutes.");
                await ReplyAsync($"", embed:embedded);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
