using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AtlasResourceBot.Modles;

namespace AtlasResourceBot.Services
{
    public class WebhookCommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public WebhookCommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _logger = services.GetRequiredService<ILogger<CommandHandler>>();
            _services = services;
        
            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
            
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {




            //// ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                Console.WriteLine("webhook returning 1");
                return;
            }

            //Console.WriteLine(message.Source.ToString());
            //Console.WriteLine(message.Author.ToString());
            //Console.WriteLine(message.Author.IsBot.ToString());
            //Console.WriteLine(message.Author.IsWebhook.ToString());
            //Console.WriteLine(MessageSource.User.ToString());


            var context = new SocketCommandContext(_client, message);




            if (message.Source != MessageSource.Webhook)
            {

                //if its no me testing

                if (message.Author.ToString() != "BigusRedus#2029" || message.Channel.Id != 774132302899838976)
                {
                    Console.WriteLine("webhook returning 2");
                    return;
                }


               // return;
            }




            List<AtlasLogEntry> entries = proccessMessage(rawMessage.Content);

         //   await message.DeleteAsync();
        //    await message.ModifyAsync(m => { m.Content = "message recieced and proccesed"; });
           // Console.WriteLine($"Message proccesssed");

            await context.Channel.SendMessageAsync($"{context.User.Username}... REcied message proccesing <{context.User.Username}>>!");
        }

        public List<AtlasLogEntry> proccessMessage(string message)
        {
            var logEntries = new List<AtlasLogEntry>();
             Console.WriteLine($"Begining  proccess");
            //split the webhook message into single line strings
            //TODO need to fix THIS--------------------------------------------------------
            string[] stringEntries = message.ToString()
             .Split(new[]{ Environment.NewLine }, StringSplitOptions.None);

            stringEntries = System.Text.RegularExpressions.Regex.Split(message.ToString(), "\n");


            //     string[] stringEntries = System.Text.RegularExpressions.Regex.Split(message.ToString(), "\r\n");
            // Console.WriteLine($"Testi2222");
            Console.WriteLine(stringEntries.Length);
           // Console.WriteLine(message.ToString());
            //Console.WriteLine(stringEntries.ToString());
            //add new entry for each line excluding the first
            for (int i = 0; i < stringEntries.Length; i++)
            {
                logEntries.Add(new AtlasLogEntry(stringEntries[i]));
            }

            Console.WriteLine(logEntries.Count);

            foreach(var log in logEntries)
            {
                System.Console.WriteLine(log.ToString());
            }
            
            
            return logEntries;


        }

        // public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        // {
        //     // if a command isn't found, log that info to console and exit this method
        //     if (!command.IsSpecified)
        //     {
        //         System.Console.WriteLine($"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
        //         return;
        //     }
                

        //     // log success to the console and exit this method
        //     if (result.IsSuccess)
        //     {
        //         System.Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.User.Username}]");
        //         return;
        //     }
                

        //     // failure scenario, let's let the user know
        //     await context.Channel.SendMessageAsync($"Sorry, {context.User.Username}... something went wrong -> [{result}]!");
        // }        
    }
}