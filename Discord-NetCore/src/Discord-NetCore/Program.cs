using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using NetCoreBot;

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
        /// Discord COmmand Service
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
        private DependencyMap map;
        /// <summary>
        /// Run the main program asynchronously
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => new Program().Start(args).GetAwaiter().GetResult();

        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task Start(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
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
            Console.WriteLine($"Connection String: {argv["ConnectionString"].Substring(16)}");
            Console.WriteLine($"Token: {argv["DiscordToken"].Substring(5)}");
            Console.WriteLine($"Data: {argv["DataLocation"]}");
            Console.WriteLine("Logging into server");
            var config = new DiscordSocketConfig {AudioMode = AudioMode.Both};

            Client = new DiscordSocketClient(config);
            commands = new CommandService();
            await Client.LoginAsync(TokenType.Bot, argv["DiscordToken"]);

            Console.WriteLine("Successfully Logged in.");

            Database = new DbHandler(argv["ConnectionString"]);
            await Client.ConnectAsync();
            await InstallCommands();
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
            /*
            if (!result.IsSuccess)
                await message.Channel.SendMessageAsync(result.ErrorReason);
            */
        }
    }
}


