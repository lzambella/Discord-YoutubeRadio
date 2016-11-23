using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
namespace GachiNetCore.Modules
{
    [Module]
    public class GachiBankModule
    {
        [Command("bank"), Summary("Check your GachiPoints")]
        public async Task Bank(IUserMessage msg)
        {
            var userId = DbHandler.ParseString(msg.Author.Mention);
            var points = await Program.Database.GetGachiPoints(userId);
            await msg.Channel.SendMessageAsync($"{msg.Author.Mention}, you have {points} Gachi Points.");
        }

    }
}