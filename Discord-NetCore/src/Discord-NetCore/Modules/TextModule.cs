using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Discord_NetCore.Modules
{
    [Name("Text")]
    public class TextModule : ModuleBase
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
                var guild = Program.Client.GetGuild(msg.Channel.Id);
                var userDM = await guild.GetUser(id).CreateDMChannelAsync();
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
                if (user.Id == Program.OwnerId)
                {
                    if (s == null)
                        return;
                    var num = Int32.Parse(s);
                    var messages = await msg.Channel.GetMessagesAsync(num).Flatten();
                    await msg.Channel.DeleteMessagesAsync(messages);
                }
            }
            catch (Exception)
            {

            }
        }

    }
}
