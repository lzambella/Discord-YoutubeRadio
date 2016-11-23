using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
namespace NetCoreBot.Modules
{
    [Module, Name("Text")]
    public class TextModule
    {
        [Command("poke"), Summary("Annoy someone")]
        public async Task Annoy(IUserMessage msg, [Summary("Mention")]string nickMention = null)
        {
            if (nickMention == null)
            {
                await msg.Channel.SendMessageAsync("You didn't mention anyone!");
            }
            else
            {
                var id = ulong.Parse(Program.Database.ParseString(nickMention));
                var guild = await Program.Client.GetGuildAsync(msg.Channel.Id);
                var userDM = await guild.GetUserAsync(id).Result.CreateDMChannelAsync();
                for (var x = 0; x < 5; x++)
                    await userDM.SendMessageAsync("You are being annoying!!!! Get On!!!!", true);
            }
        }
        [Command("8ball"), Summary("Ask me for advice.")]
        public async Task Ball(IUserMessage msg, [Summary("Question")] string s = null)
        {
            if (s == null)
                await msg.Channel.SendMessageAsync("I didn't get a question.");
            else
            {
                var arr = new string[] { "Yes", "No", "Maybe", "Ask again", "I'm not answering that", "That question sucks" };
                var rand = new Random();
                var index = rand.Next(arr.Length);
                await msg.Channel.SendMessageAsync(arr[index]);
            }
        }
        [Command("purge"), Summary("Delete a number of messages")]
        public async Task Purge(IUserMessage msg, [Summary("Number of messages")]string s = null)
        {
            try
            {
                var user = msg.Author as IGuildUser;
                if (user.Roles.Count(role => role.Name.Equals("King")) > 0 || user.Id == Program.OwnerId)
                {
                    if (s == null)
                        return;
                    var num = Int32.Parse(s);
                    var messages = await msg.Channel.GetMessagesAsync(num);
                    await msg.Channel.DeleteMessagesAsync(messages);
                }
            }
            catch (Exception)
            {

            }
        }

    }
}
