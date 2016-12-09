using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Commands;

namespace Discord_NetCore.Modules
{
    [Name("Math")]
    public class MathModule : ModuleBase
    {
        private readonly string _wolframToken = Program.argv["WolframToken"];
        [Command("wolfram"), Summary("Get a wolfram query")]
        public async Task Wolfram([Summary("wolfram query")]string query = null)
        {
            if (query == null)
            {
                await ReplyAsync("Error. No query.");
            }
            else
            {
                var str = "";
                var httpClient = new HttpClient();
                var url = $"http://api.wolframalpha.com/v2/query";
                var param = $"?input=\"{query}\"&appid={_wolframToken}";
                httpClient.BaseAddress = new Uri(url);
                var response = await httpClient.GetAsync(param);
                if (response.IsSuccessStatusCode)
                {
                    var xml = await response.Content.ReadAsStringAsync();
                    XmlReader reader = XmlReader.Create(new StringReader(xml));
                    try
                    {
                        while (reader.ReadToFollowing("pod"))
                        {
                            reader.MoveToFirstAttribute();
                            var title = reader.Value;
                            reader.ReadToFollowing("plaintext");
                            var val = reader.ReadElementContentAsString();
                            if (val.Length > 0)
                                str += $"`{title}`: `{val}`\n";
                        }
                        await ReplyAsync(str);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

            }
        }
    }
}
