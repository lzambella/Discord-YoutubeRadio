using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Discord.Commands;
using Discord;
using System.Net.Http;
using System.IO;

namespace NetCoreBot.Modules
{
    [Module, Name("Math")]
    public class MathModule
    {
        string WolframToken = Program.argv["WolframToken"];
        [Command("wolfram"), Summary("Get a wolfram query")]
        public async Task wolfram(IUserMessage msg, [Summary("wolfram query")]string query = null)
        {
            if (query == null)
            {
                await msg.Channel.SendMessageAsync("Error. No query.");
            }
            else
            {
                var str = "";
                var httpClient = new HttpClient();
                var url = $"http://api.wolframalpha.com/v2/query";
                var param = $"?input=\"{query}\"&appid={WolframToken}";
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
                        await msg.Channel.SendMessageAsync(str);
                    }
                    catch (Exception)
                    {
                    }
                }

            }
        }
    }
}
