using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DiscordBot
{
    public class BasicBot
    {

        // Non-static readonly fields can only be assigned in a constructor.
        // If you want to assign it elsewhere, consider removing the readonly keyword.
        private readonly DiscordSocketClient _client;
        private readonly SettingsHelper _settingsHelper;
        private readonly ILogger<BasicBot> _logger;
        public BasicBot(SettingsHelper settingsHelper, ILogger<BasicBot> logger)
        {
            _settingsHelper = settingsHelper;
            _logger = logger;
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            // Subscribing to client events, so that we may receive them whenever they're invoked.
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.InteractionCreated += InteractionCreatedAsync;
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, _settingsHelper.Token);
            // Different approaches to making your token a secret is by putting them in local .json, .yaml, .xml or .txt files, then reading them on startup.

            await _client.StartAsync();


            // Block the program until it is closed.
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            if (log.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{log.Severity}] {cmdException.Command.Aliases.First()}"
                                  + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            var strLog = log.ToString();
            Log.Information(strLog);
            Console.WriteLine(strLog);
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Log.Information(message.Content);

            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "選辣雞")
            {
                var options = new List<SelectMenuOptionBuilder>
                {
                    new SelectMenuOptionBuilder
                    {
                        Label = "李元傑",
                        Value = "李元傑",
                        Description = "小垃圾"
                    },
                    new SelectMenuOptionBuilder
                    {
                        Label = "黃冠傑",
                        Value = "黃冠傑",
                        Description = "大垃圾"
                    },
                    new SelectMenuOptionBuilder
                    {
                        Label = "RA",
                        Value = "RA",
                        Description = "追尋廷寶的老鼠"
                    }
                };
                var bb = new ComponentBuilder()
                    .WithSelectMenu("選辣雞", options, minValues: 1, maxValues: options.Count, placeholder: "選一個傢伙");
                await message.Channel.SendMessageAsync("Hi",components: bb.Build());
                return;
            }


            if (message.Content == "!ping")
            {

                // Create a new componentbuilder, in which dropdowns & buttons can be created.
                var cb = new ComponentBuilder()
                    .WithButton("Click me!", "unique-id", ButtonStyle.Primary);




                // Send a message with content 'pong', including a button.
                // This button needs to be build by calling .Build() before being passed into the call.
                await message.Channel.SendMessageAsync("pong!", components: cb.Build());
                return;
            }
        }

        // For better functionality & a more developer-friendly approach to handling any kind of interaction, refer to:
        // https://discordnet.dev/guides/int_framework/intro.html
        private async Task InteractionCreatedAsync(SocketInteraction interaction)
        {

            // safety-casting is the best way to prevent something being cast from being null.
            // If this check does not pass, it could not be cast to said type.
            if (interaction is SocketMessageComponent component)
            {
                if (component.Data.Type == ComponentType.SelectMenu)
                {
                    if (component.Data.CustomId == "選辣雞")
                    {
                        var contents = component.Data.Values.ToArray();
                        var stringBuilder = new StringBuilder();
                        foreach (var value in contents)
                        {
                            stringBuilder.Append("你選了" + value + "這個辣雞\n");
                        }

                        await interaction.RespondAsync(stringBuilder.ToString());
                        return;
                    }
                }


                // Check for the ID created in the button mentioned above.
                if (component.Data.CustomId == "unique-id")
                    await interaction.RespondAsync("Thank you for clicking my button!");

                else Console.WriteLine("An ID has been received that has no handler!");

                if (interaction.User.Id == 557508424951267328)
                {
                    await interaction.RespondAsync("親愛的");
                    return;
                }
            }
        }
    }


}
