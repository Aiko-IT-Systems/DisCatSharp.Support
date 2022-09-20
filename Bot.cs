using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.EventHandling;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Phabricator;
using DisCatSharp.Support.Entities.Config;
using DisCatSharp.Support.Events.Discord;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Serilog;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DisCatSharp.Support
{
    /// <summary>
    /// The bot.
    /// </summary>
    internal class Bot : IDisposable
    {
        /// <summary>
        /// Gets the shutdown request.
        /// </summary>
        public static CancellationTokenSource ShutdownRequest { get; internal set; }

        /// <summary>
        /// Gets the config.
        /// </summary>
        public static Config Config { get; internal set; }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        public static LogLevel LogLevel { get; internal set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public static string ConnectionString 
            => @$"server={Config.DatabaseConfig.Hostname};userid={Config.DatabaseConfig.User};password={Config.DatabaseConfig.Password};database={Config.DatabaseConfig.Db};SslMode=none";

        /// <summary>
        /// Gets the discord client.
        /// </summary>
        public static DiscordClient DiscordClient { get; internal set; }

        /// <summary>
        /// Gets the conduit client.
        /// </summary>
        public static ConduitClient ConduitClient { get; internal set; }


        /// <summary>
        /// Gets the application commands extension.
        /// </summary>
        public static ApplicationCommandsExtension ApplicationCommandsExtension { get; internal set; }

        /// <summary>
        /// Gets the commands next extension.
        /// </summary>
        public static CommandsNextExtension CommandsNextExtension { get; internal set; }

        /// <summary>
        /// Gets the interactivity extension.
        /// </summary>
        public static InteractivityExtension InteractivityExtension { get; internal set; }


        /// <summary>
        /// Gets the discord configuration.
        /// </summary>
        public static DiscordConfiguration DiscordConfiguration { get; internal set; }

        /// <summary>
        /// Gets the application commands configuration.
        /// </summary>
        public static ApplicationCommandsConfiguration ApplicationCommandsConfiguration { get; internal set; }

        /// <summary>
        /// Gets the commands next configuration.
        /// </summary>
        public static CommandsNextConfiguration CommandsNextConfiguration { get; internal set; }

        /// <summary>
        /// Gets the interactivity configuration.
        /// </summary>
        public static InteractivityConfiguration InteractivityConfiguration { get; internal set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Bot"/> class.
        /// </summary>
        public Bot(LogLevel logLevel)
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            LogLevel = logLevel;
            SetupConfigs();
            SetupClients();
            RegisterEvents();
            RegisterCommands();

        }

        /// <summary>
        /// Starts the bot.
        /// </summary>
        public async Task RunAsync()
        {
            await DiscordClient.ConnectAsync();
//            Console.OutputEncoding = new UTF8Encoding();
//            Console.BackgroundColor = ConsoleColor.DarkBlue;
//            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
//            Console.ResetColor();
//            Center("");
            Console.WriteLine($"Logged in as {DiscordClient.CurrentUser.UsernameWithDiscriminator} with prefix {Config.DiscordConfig.Prefix}");
//            Center("");
//            Console.BackgroundColor = ConsoleColor.DarkBlue;
//            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
//            Console.ResetColor();
            while (!ShutdownRequest.IsCancellationRequested)
            {
                await Task.Delay(2000);
            }
            await DiscordClient.UpdateStatusAsync(activity: null, userStatus: UserStatus.Offline, idleSince: null);
            await DiscordClient.DisconnectAsync();
            DeregisterEvents();
            Dispose();
        }

        /// <summary>
        /// Setups the configs.
        /// </summary>
        private static void SetupConfigs()
        {
            ShutdownRequest = new();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            
            DiscordConfiguration = new()
            {
                Token = Config.DiscordConfig.BotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MessageCacheSize = 2048,
                MinimumLogLevel = LogLevel,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers | DiscordIntents.MessageContent,
                UseCanary = true,
                ReconnectIndefinitely = true,
                LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger)
            };

            CommandsNextConfiguration = new()
            {
                StringPrefixes = new string[] { Config.DiscordConfig.Prefix },
                CaseSensitive = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                DefaultHelpChecks = null,
                EnableDefaultHelp = true,
                EnableDms = true,
                DmHelp = true,
                UseDefaultCommandHandler = true
            };

            InteractivityConfiguration = new()
            {
                Timeout = TimeSpan.FromMinutes(2),
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PollBehaviour = PollBehaviour.DeleteEmojis,
                AckPaginationButtons = true,
                ButtonBehavior = ButtonPaginationBehavior.Disable,
                PaginationButtons = new PaginationButtons()
                {
                    SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-skip-left", "First", false, new DiscordComponentEmoji("⏮️")),
                    Left = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-left", "Previous", false, new DiscordComponentEmoji("◀️")),
                    Stop = new DiscordButtonComponent(ButtonStyle.Danger, "pgb-stop", "Stop", false, new DiscordComponentEmoji("⏹️")),
                    Right = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-right", "Next", false, new DiscordComponentEmoji("▶️")),
                    SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-skip-right", "Last", false, new DiscordComponentEmoji("⏭️"))
                },
                ResponseMessage = "Something went wrong.",
                ResponseBehavior = InteractionResponseBehavior.Respond
            };

            ApplicationCommandsConfiguration = new()
            {
                AutoDefer = false,
                CheckAllGuilds = false,
                DebugStartup = false,
                EnableLocalization = false,
                EnableDefaultHelp = false,
                ManualOverride = false,
                GenerateTranslationFilesOnly = false
            };
        }

        /// <summary>
        /// Setups the clients.
        /// </summary>
        private static void SetupClients()
        {
            DiscordClient = new(DiscordConfiguration);
            CommandsNextExtension = DiscordClient.UseCommandsNext(CommandsNextConfiguration);
            InteractivityExtension = DiscordClient.UseInteractivity(InteractivityConfiguration);
            DiscordClient.UseApplicationCommands(ApplicationCommandsConfiguration);
            ApplicationCommandsExtension = DiscordClient.GetApplicationCommands();

            ConduitClient = new(Config.ConduitConfig.ApiHost, Config.ConduitConfig.ApiToken);
        }

        /// <summary>
        /// Registers the events.
        /// </summary>
        private static void RegisterEvents()
		{/*
            DiscordClient.Ready += Client_Ready;
            DiscordClient.Resumed += Client_Resumed;
            DiscordClient.ClientErrored += Client_Errored;
            DiscordClient.Heartbeated += Client_Heartbeated;

            DiscordClient.UnknownEvent += Client_UnknownEvent;

            DiscordClient.SocketOpened += Client_SocketOpened;
            DiscordClient.SocketClosed += Client_SocketClosed;
            DiscordClient.SocketErrored += Client_SocketErrored;

            DiscordClient.GuildAvailable += Client_GuildAvailable;
            DiscordClient.GuildUnavailable += Client_GuildUnavailable;

            DiscordClient.GuildCreated += Client_GuildCreated;
            DiscordClient.GuildUpdated += Client_GuildUpdated;
            DiscordClient.GuildDeleted += Client_GuildDeleted;
            */
			DiscordClient.Zombied += Client_Zombied;
			DiscordClient.MessageCreated += async (sender, args) => await Task.Run(async () => await MessageEvents.Client_MessageCreated(sender, args));
            DiscordClient.ComponentInteractionCreated += async (sender, args) => await Task.Run(async () => await InteractionEvents.InteractionCreated(sender, args));
            /*
            DiscordClient.MessageReactionAdded += Client_MessageReactionAdded;

            DiscordClient.GuildMemberAdded += Client_GuildMemberAdded;
            DiscordClient.GuildMemberRemoved += Client_GuildMemberRemoved;

            DiscordClient.GuildRoleCreated += Client_GuildRoleCreated;
            DiscordClient.GuildRoleUpdated += Client_GuildRoleUpdated;
            DiscordClient.GuildRoleDeleted += Client_GuildRoleDeleted;

            DiscordClient.ApplicationCommandCreated += Discord_ApplicationCommandCreated;
            DiscordClient.ApplicationCommandUpdated += Discord_ApplicationCommandUpdated;
            DiscordClient.ApplicationCommandDeleted += Discord_ApplicationCommandDeleted;

            DiscordClient.ComponentInteractionCreated += Client_ComponentInteractionCreated;
            DiscordClient.ApplicationCommandPermissionsUpdated += Client_ApplicationCommandPermissionsUpdated;

            ApplicationCommandsExtension.SlashCommandErrored += ApplicationCommand_SlashErrored;
            ApplicationCommandsExtension.SlashCommandExecuted += ApplicationCommand_SlashExecuted;
            ApplicationCommandsExtension.ContextMenuErrored += ApplicationCommand_ContextMenuErrored;
            ApplicationCommandsExtension.ContextMenuExecuted += ApplicationCommand_ContextMenuExecuted;

            CommandsNextExtension.CommandErrored += CommandNext_CommandErrored;*/

            ApplicationCommandsExtension.ApplicationCommandsModuleStartupFinished += ApplicationCommandsExtension_ApplicationCommandsModuleStartupFinished;
        }

        private static Task Client_Zombied(DiscordClient sender, ZombiedEventArgs e)
        {
            ShutdownRequest.Cancel();
            return Task.FromResult(true);
        }

        private static Task ApplicationCommandsExtension_ApplicationCommandsModuleStartupFinished(ApplicationCommandsExtension sender, ApplicationCommands.EventArgs.ApplicationCommandsModuleStartupFinishedEventArgs e)
        {
            sender.Client.Logger.LogInformation($"Application commands module has finished the startup.");
            /*var guild_cmd_count = 0;
            foreach (var cmd in e.RegisteredGuildCommands)
            {
                guild_cmd_count += cmd.Value.Select(x => x.Name).Distinct().Count();
            }
            sender.Client.Logger.LogInformation($"Stats: \n" +
                $" - Found {e.GuildsWithoutScope.Count} guilds without the applications.commands scope\n" +
                $" - Registered {e.RegisteredGlobalCommands.Count} global commands\n" +
                $" - Registered {guild_cmd_count} commands on {e.RegisteredGuildCommands.Count} guilds."
            );*/

            return Task.CompletedTask;
        }

        /// <summary>
        /// Deregisters the events.
        /// </summary>
        private static void DeregisterEvents()
		{/*
            DiscordClient.Ready -= Client_Ready;
            DiscordClient.Resumed -= Client_Resumed;
            DiscordClient.ClientErrored -= Client_Errored;
            DiscordClient.Heartbeated -= Client_Heartbeated;

            DiscordClient.UnknownEvent -= Client_UnknownEvent;

            DiscordClient.SocketOpened -= Client_SocketOpened;
            DiscordClient.SocketClosed -= Client_SocketClosed;
            DiscordClient.SocketErrored -= Client_SocketErrored;

            DiscordClient.GuildAvailable -= Client_GuildAvailable;
            DiscordClient.GuildUnavailable -= Client_GuildUnavailable;

            DiscordClient.GuildCreated -= Client_GuildCreated;
            DiscordClient.GuildUpdated -= Client_GuildUpdated;
            DiscordClient.GuildDeleted -= Client_GuildDeleted;
            */
			DiscordClient.Zombied -= Client_Zombied;
			DiscordClient.MessageCreated -= MessageEvents.Client_MessageCreated;
            DiscordClient.ComponentInteractionCreated -= InteractionEvents.InteractionCreated;
			/*
            DiscordClient.MessageReactionAdded -= Client_MessageReactionAdded;

            DiscordClient.GuildMemberAdded -= Client_GuildMemberAdded;
            DiscordClient.GuildMemberRemoved -= Client_GuildMemberRemoved;

            DiscordClient.GuildRoleCreated -= Client_GuildRoleCreated;
            DiscordClient.GuildRoleUpdated -= Client_GuildRoleUpdated;
            DiscordClient.GuildRoleDeleted -= Client_GuildRoleDeleted;

            DiscordClient.ApplicationCommandCreated -= Client_ApplicationCommandCreated;
            DiscordClient.ApplicationCommandUpdated -= Client_ApplicationCommandUpdated;
            DiscordClient.ApplicationCommandDeleted -= Client_ApplicationCommandDeleted;

            DiscordClient.ComponentInteractionCreated -= Client_ComponentInteractionCreated;

            ApplicationCommandsExtension.SlashCommandErrored -= ApplicationCommand_SlashErrored;
            ApplicationCommandsExtension.SlashCommandExecuted -= ApplicationCommand_SlashExecuted;
            ApplicationCommandsExtension.ContextMenuErrored -= ApplicationCommand_ContextMenuErrored;
            ApplicationCommandsExtension.ContextMenuExecuted -= ApplicationCommand_ContextMenuExecuted;

            DiscordClient.ApplicationCommandPermissionsUpdated -= Client_ApplicationCommandPermissionsUpdated;

            CommandsNextExtension.CommandErrored -= CommandNext_CommandErrored;*/
			ApplicationCommandsExtension.ApplicationCommandsModuleStartupFinished -= ApplicationCommandsExtension_ApplicationCommandsModuleStartupFinished;
		}

        /// <summary>
        /// Registers the commands.
        /// </summary>
        private static void RegisterCommands()
        {
            /*Type commandModule = typeof(BaseCommandModule);
            var commands = Assembly.GetExecutingAssembly().GetTypes().Where(t => commandModule.IsAssignableFrom(t) && !t.IsNested).ToList();

            foreach (var command in commands)
            {
                CommandsNextExtension.RegisterCommands(command);
            }
            */
            Type applicationCommandModule = typeof(ApplicationCommandsModule);
            var applicationCommands = Assembly.GetExecutingAssembly().GetTypes().Where(t => applicationCommandModule.IsAssignableFrom(t) && !t.IsNested).ToList();

            foreach (var command in applicationCommands)
            {
                ApplicationCommandsExtension.RegisterGuildCommands(command, Config.DiscordConfig.ApplicationCommandConfig.Dcs.GuildId);

                ApplicationCommandsExtension.RegisterGuildCommands(command, Config.DiscordConfig.ApplicationCommandConfig.DcsDev.GuildId);

                ApplicationCommandsExtension.RegisterGuildCommands(command, 858089274087309313);
            }
        }


        /// <summary>
        /// Disposes the bot.
        /// </summary>
        public void Dispose()
        {
            DiscordClient.Dispose();

            ApplicationCommandsExtension = null;
            CommandsNextExtension = null;
            InteractivityExtension = null;

            ApplicationCommandsConfiguration = null;
            CommandsNextConfiguration = null;
            InteractivityConfiguration = null;
            DiscordConfiguration = null;

            ConduitClient = null;
            DiscordClient = null;
        }

        /// <summary>
        /// Centers the console.
        /// </summary>
        /// <param name="s">The text.</param>
        public static void Center(string s)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("██");
                Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
                Console.ResetColor();
                Console.Write(s);
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.SetCursorPosition((Console.WindowWidth - 4), Console.CursorTop);
                Console.WriteLine("██");
                Console.ResetColor();
            }
            catch (Exception)
            {
                s = "Console to smoll EXC";
                Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
                Console.Write(s);
                Console.SetCursorPosition((Console.WindowWidth - 4), Console.CursorTop);
                Console.WriteLine("██");
            }
        }
    }
}
