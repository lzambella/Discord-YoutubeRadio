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
using System.Xml.Serialization;
using System.IO;
using System.Xml;
//using FacebookSharp;

namespace Discord_NetCore
{
    public class Program
    {
        public static bool DEBUG = true;
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
        /// <summary>
        /// TODO:
        /// Key value pair for quickly getting the channel the bot responds to
        /// reduces database calls
        /// </summary>
        public static Dictionary<ulong, ulong> BotChatChannel = new Dictionary<ulong, ulong>();

        public static Modules.Audio.MusicPlayer Player { get; set; }
        public static IList<TempVoice> TempVoiceChannels { get; set; } = new List<TempVoice>();
        /// <summary>
        /// Contains the music player for a specific server
        /// </summary>
        public static Dictionary<ulong, Modules.Audio.MusicPlayer> MusicPlayers { get; set; } = new Dictionary<ulong, Modules.Audio.MusicPlayer>();

        //private DependencyMap map;
        private string LatestMeme { get; set; }
        /// <summary>
        /// Run the main program asynchronously
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => new Program().Start(args).GetAwaiter().GetResult();
        public static string FacebookToken { get; private set; }
        private Timer Timer { get; set; }
        /// <summary>
        /// List of playlists
        /// </summary>
        public static List<Playlist> Playlists = new List<Playlist>();
        public static string botName;
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string DatabaseString {get; set;}

        public async Task Start(string[] args)
        {
            var discordToken = Environment.GetEnvironmentVariable("discordToken");
            DatabaseString = Environment.GetEnvironmentVariable("databaseString");
            FacebookToken = Environment.GetEnvironmentVariable("facebookToken");
            Console.WriteLine("Successfully read the settings");
            Console.WriteLine("Logging into server");

            Database = new DbHandler(DatabaseString);
            Console.WriteLine("Successfully initialized database");
            var config = new DiscordSocketConfig
            {
                ConnectionTimeout = 100000
            };
            Client = new DiscordSocketClient(config);
            commands = new CommandService();

            await Client.LoginAsync(TokenType.Bot, discordToken);

            await Client.StartAsync();
            await InstallCommands();
            Client.Connected +=  async () =>
            {
                await Client.SetGameAsync("Bose of this gym.");
                Console.WriteLine("Successfully logged in.");
                Console.WriteLine($"Connected to {Client.Guilds.Count} servers!");
                var Name = await Client.GetApplicationInfoAsync();
                botName = Name.Name;
            };

            ///TODO: Deserialize all the playlists in the playlists folder
            Console.WriteLine("Attempting to read the playlists.");
            XmlSerializer deserializer = new XmlSerializer(typeof(Playlist));
            var files = Directory.GetFiles($"{Directory.GetCurrentDirectory()}/Playlists");
            foreach (string file in files)
            {
                try
                {
                    StreamReader reader = new StreamReader(file);
                    if (!file.Contains(".xml")) continue;
                    Playlist p = (Playlist)deserializer.Deserialize(reader);
                    
                    Playlists.Add(p);
                    reader.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine("Playlist reading finished.");
            //Timer = new Timer(ImagePoster, null, 6000, 6000);
            await Task.Delay(-1);

        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            Client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                            services: null);
        }
        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            
            int argPos = 0;
            // Check if the command is an operator command that overrides the chat channel rule
            var context = new CommandContext(Client, message);
            if (message.HasCharPrefix('#', ref argPos) && (message.Author.Id == OwnerId || await Database.GetPermission(Client.GetGuild(context.Guild.Id).GetUser(message.Author.Id)) == 5))
            {
                // Execute the command
                var res = await commands.ExecuteAsync(context: context, 
                                                        argPos: argPos,
                                                        services: null);
                if (res.IsSuccess)
                    Console.WriteLine($"Operator request from {messageParam.Author.Username}. Command: {messageParam.Content}.");
            }
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))) return;

            var channelId = context.Channel.Id;
            if (await Database.GetChatChannel(context.Guild) != (Int64)channelId) return; // If the command was not executed from the correct channel

            var result = await commands.ExecuteAsync(context: context, 
                                                        argPos: argPos,
                                                        services: null);
            if (result.IsSuccess)
                Console.WriteLine($"Command request from {messageParam.Author.Username}. Command: {messageParam.Content}.");
        }
        /// <summary>
        /// Posts a random meme to the chat if there is a new meme
        /// </summary>
        /// <param name="callback"></param>
        /*
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
                var page = await graphApi.GetPage("1708210979407800");

                var images = await page.GetPhotos(true);
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


