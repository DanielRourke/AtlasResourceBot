// using System;
// using Discord;
// using System.Threading.Tasks;
// using Discord.WebSocket;
// using Microsoft.Extensions.DependencyInjection;

// namespace AtlasResourceBot.Services
// {
//     public class WebhookNotifyService
//     {
//         // setup fields
//        // private readonly IConfiguration _config;
//         private readonly IServiceProvider _services;
//         private readonly DiscordSocketClient _client;
//         public WebhookNotifyService(IServiceProvider services)
//         {
//             //_config = services.GetRequiredService<IConfiguration>();
//             _client = services.GetRequiredService<DiscordSocketClient>();

//             _services = services;

//             _client.MessageReceived += MessageReceivedAsync;
//         }

//         // this class is where the magic starts, and takes actions upon receiving messages
//         public async Task MessageReceivedAsync(SocketMessage rawMessage)
//         {
//             // ensures we don't process system/other bot messages
//             if (!(rawMessage is SocketUserMessage message)) 
//             {
//                 return;
//             }
            
//             if (message.Source != MessageSource.User) 
//             {
//                 return;
//             }

//             if (!message.Author.IsBot || !message.ToString().StartsWith("**tester"))
//             {
//                 return;
//             }

//                 Console.WriteLine("recieved atlas message");
           
//                 return ;
//            // var context = new SocketCommandContext(_client, message);

//             // execute command if one is found that matches
//            // await _commands.ExecuteAsync(context, argPos, _services); 
//         }
//     }
// }