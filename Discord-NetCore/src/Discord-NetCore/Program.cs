using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace NetCoreBot
{
    public class Program
    {
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

            Client = new DiscordSocketClient();
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

        /// <summary>
        /// Installs the Discord Command Service
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommands()
        {
            Client.MessageReceived += HandleCommand;
            await commands.LoadAssembly(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Handler for executing a command
        /// </summary>
        /// <param name="paramMessage"></param>
        /// <returns></returns>
        public async Task HandleCommand(IMessage paramMessage)
        {
            var msg = paramMessage as IUserMessage;
            if (msg == null) return;
            var argPos = 0;
            var currentUser = await Client.GetCurrentUserAsync();

            if (msg.HasCharPrefix('!', ref argPos) || msg.HasMentionPrefix(currentUser, ref argPos))
            {
                var result = await commands.Execute(msg, argPos);
                if (result.IsSuccess)
                    Console.WriteLine($"{DateTime.Now.ToString()}: Command request from {msg.Author.Username}. Command: {msg.Content}");
            }
        }
    }
}


