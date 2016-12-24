using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FacebookSharp;
using FacebookSharp.GraphAPI.Fields;

namespace Discord_NetCore.Modules
{
    [Name("Text")]
    public class TextModule : ModuleBase
    {
        [Command("poke"), Summary("Annoy someone")]
        public async Task Annoy([Summary("Mention")]string nickMention = null)
        {
            if (nickMention == null)
                await ReplyAsync("You didn't mention anyone!");
            else
            {
                var id = ulong.Parse(Program.Database.ParseString(nickMention));
                var guild = Program.Client.GetGuild(Context.Channel.Id);
                var userDm = await guild.GetUser(id).CreateDMChannelAsync();
                for (var x = 0; x < 5; x++)
                    await userDm.SendMessageAsync("You are being annoying!!!! Get On!!!!", true);
            }
        }
        [Command("8ball"), Summary("Ask me for advice.")]
        public async Task Ball([Summary("Question")] string s = null)
        {
            if (s == null)
                await ReplyAsync("I didn't get a question.");
            else
            {
                var arr = new[] { "Yes", "No", "Maybe", "Ask again", "I'm not answering that", "That question sucks" };
                var time = DateTime.Now.ToFileTime();
                var rand = (time/100)%arr.Length;
                await ReplyAsync(arr[rand]);
            }
        }
        [Command("purge"), Summary("Delete a number of messages")]
        public async Task Purge([Summary("Number of messages")]string s = null)
        {
            try
            {
                var user = Context.User as IGuildUser;
                if (user != null && user.Id == Program.OwnerId)
                {
                    if (s == null)
                        return;
                    var num = int.Parse(s);
                    var messages = await Context.Channel.GetMessagesAsync(num).Flatten();
                    await Context.Channel.DeleteMessagesAsync(messages);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        [Command("spy"), Summary("Spy on someone's facebook page")]
        public async Task Spy([Summary("User id")] string id = null)
        {
            try
            {
                if (id == null)
                {
                    await ReplyAsync("Please give an ID");
                    return;
                }
                var api = new GraphApi(Program.argv["FacebookToken"], GraphApi.ApiVersion.TwoEight);
                var fields = new ApiField();
                fields.Fields.Add("name");
                fields.Fields.Add("description");
                var user = await api.GetUser(id);
                var name = user.Name ?? "No name";
                var description = user.About ?? "No description";
                await ReplyAsync($"```{name}\n{description}\n```");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

    }
}
