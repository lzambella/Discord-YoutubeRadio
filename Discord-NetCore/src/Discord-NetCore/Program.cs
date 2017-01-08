using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using FacebookSharp;
using FacebookSharp.GraphAPI.Fields;

namespace Discord_NetCore
{
    public class Program
    {
        // Create an IAudioClient, and store it for later use
        public static IAudioClient Audio { get; set; }
        /// <summary>
        /// The Discord ID of the owner of the bot
        /// </summary>
        public static ulong OwnerId = 215339054416396289;
        /// <summary>
        /// The Database connection string
        /// </summary>
        public static DbHandler Database { get; set; }
        /// <summary>
        /// Discord Command Service
        /// </summary>
        public static CommandService commands;
        /// <summary>
        /// Discord Client object
        /// </summary>
        public static DiscordSocketClient Client;
        /// <summary>
        /// Dictionary of all the arguments
        /// </summary>
        public static Dictionary<string, string> argv = new Dictionary<string, string>();
        public static Modules.Audio.MusicPlayer Player { get; set; }
        public static IList<TempVoice> TempVoiceChannels { get; set; } = new List<TempVoice>();
        /// <summary>
        /// Contains the music player for a specific server
        /// </summary>
        public static Dictionary<ulong, Modules.Audio.MusicPlayer> MusicPlayers {get; set; }

        private DependencyMap map;
        private string LatestMeme { get; set; }
        /// <summary>
        /// Run the main program asynchronously
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => new Program().Start(args).GetAwaiter().GetResult();

        private Timer Timer { get; set; }
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task Start(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {

                try
                {
                    switch (args[i])
                    {
                        case "-dbstring":
                            argv.Add("ConnectionString", args[i + 1]);
                            break;
                        case "-token":
                            argv.Add("DiscordToken", args[i + 1]);
                            break;
                        case "-data":
                            argv.Add("DataLocation", args[i + 1]);
                            break;
                        case "-fbtoken":
                            argv.Add("FacebookToken", args[i + 1]);
                            break;
                        case "-wolframtoken":
                            argv.Add("WolframToken", args[i + 1]);
                            break;
                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("Error in parsing arguments.");
                }
            }
            Console.WriteLine("Logging into server");
            var config = new DiscordSocketConfig {AudioMode = AudioMode.Both};
            Client = new DiscordSocketClient(config);
            commands = new CommandService();
            MusicPlayers = new Dictionary<ulong, Modules.Audio.MusicPlayer>();
            await Client.LoginAsync(TokenType.Bot, argv["DiscordToken"]);

            Database = new DbHandler(argv["ConnectionString"]);

            await Client.ConnectAsync();
            if (Client.ConnectionState == ConnectionState.Connected)
                Console.WriteLine($"{DateTime.Now}: Successfully Logged in.");
            else Console.WriteLine($"{DateTime.Now}: Error Something Happened.");
            await InstallCommands();
            /*
            Client.UserPresenceUpdated += async (guild, user, currentPresence, updatedPresence) =>
            {
                if (user.Id == 262069349815156746 && updatedPresence.Status == UserStatus.Online)
                {
                    await (user as IGuildUser)?.Guild.GetTextChannelAsync(215339016755740673).Result.SendMessageAsync("@everyone Holy fuck anthony has logged in!@!!!@!@!");
                }
                else if (user.Id == 215537300971454464 &&  updatedPresence.Status == UserStatus.Online)
                {
                    await (user as IGuildUser)?.Guild.GetTextChannelAsync(215339016755740673).Result.SendMessageAsync("@everyone dude.... mike has logged in");
                }
            };
            */
            Client.MessageReceived += async (e) =>
            {
                try
                {
                    if (e.Content.Contains('[') && e.Author.Username.Contains("MemeBot"))
                    {
                        var str = e.Content.Trim('[', ']');
                        var arr = str.Split(' ');
                        var points = int.Parse(arr[1]);
                        var user = arr[0];
                        await Database.ChangePoints(user, points * -1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            };
            //Timer = new Timer(ImagePoster, null, 6000, 6000);
            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            Client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(Client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed succesfully)
            var result = await commands.ExecuteAsync(context, argPos, map);
            Console.WriteLine($"{DateTime.Now}: Command request from {messageParam.Author.Username}. Command: {messageParam.Content}.");
            /*
            if (!result.IsSuccess)
                await message.Channel.SendMessageAsync(result.ErrorReason);
            */
        }
        /*
        /// <summary>
        /// Posts a random meme to the chat if there is a new meme
        /// </summary>
        /// <param name="callback"></param>
        private async void ImagePoster(object callback)
        {
            try
            {
                var token = Program.argv["FacebookToken"];
                IDiscordClient client = Program.Client;
                // Get the guild where the memes will be posted
                var guild = await client.GetGuildAsync(215339016755740673);
                var voiceChannel = await guild.GetVoiceChannelAsync(215339863254368268);
                var users = await voiceChannel.GetUsersAsync().Flatten();

                // Check if there are 3 active users in the main voice channel
                var userCount = users.Count(user => !user.IsBot && !user.IsMuted);
                if (userCount < 2)
                    return;
                // Get the latest meme and post it to the general chat
                var textChannel = await guild.GetTextChannelAsync(215339016755740673);
                var graphApi = new GraphApi(token, GraphApi.ApiVersion.TwoEight);
                var page = await graphApi.GetPage("421109484727629");
                var param = new ApiField();
                param.Fields.Add("images");
                var images = await page.GetPhotos(param, true);
                var meme = images.PhotoNodes.First().Images.First().Source;
                if (meme.Equals(LatestMeme))
                    return;

                LatestMeme = meme;
                await textChannel.SendMessageAsync($"Here's a new meme: {meme}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        */
    }
}


