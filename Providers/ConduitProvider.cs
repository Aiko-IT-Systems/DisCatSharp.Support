using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Providers
{

    /// <summary>
    /// The conduit task priority provider.
    /// </summary>
    internal class ConduitTaskPriorityProvider : IChoiceProvider
    {
        /// <summary>
        /// Provides the priority choices.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return new DiscordApplicationCommandOptionChoice[]
            {
                    new DiscordApplicationCommandOptionChoice("Unbreak now!", "unbreak"),
                    new DiscordApplicationCommandOptionChoice("Triage", "triage"),
                    new DiscordApplicationCommandOptionChoice("High", "high"),
                    new DiscordApplicationCommandOptionChoice("Normal", "normal"),
                    new DiscordApplicationCommandOptionChoice("Low", "low"),
                    new DiscordApplicationCommandOptionChoice("Wishlist", "wish")
            };
        }
    }

    /// <summary>
    /// The conduit task priority select choices.
    /// </summary>
    internal class ConduitTaskPrioritySelectProvider
    {
        /// <summary>
        /// Provides the priority choices for selects.
        /// </summary>
        public static IEnumerable<DiscordSelectComponentOption> GetSelectOptions()
        {
            return new DiscordSelectComponentOption[]
            {
                    new DiscordSelectComponentOption("Unbreak now!", "unbreak", null, false),
                    new DiscordSelectComponentOption("Triage", "triage", null, false),
                    new DiscordSelectComponentOption("High", "high", null, false),
                    new DiscordSelectComponentOption("Normal", "normal", null, true),
                    new DiscordSelectComponentOption("Low", "low", null, false),
                    new DiscordSelectComponentOption("Wishlist", "wish", null, false)
            };
        }
    }

    /// <summary>
    /// The conduit task type provider.
    /// </summary>
    internal class ConduitTaskTypeProvider : IChoiceProvider
    {
        /// <summary>
        /// Provides the type choices.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return new DiscordApplicationCommandOptionChoice[]
            {
                    new DiscordApplicationCommandOptionChoice("Bug", "[Bug]"),
                    new DiscordApplicationCommandOptionChoice("Feature", "[Feature]"),
                    new DiscordApplicationCommandOptionChoice("Docs", "[Docs]"),
                    new DiscordApplicationCommandOptionChoice("Research", "[Research]")
            };
        }
    }

    /// <summary>
    /// The conduit task priority provider.
    /// </summary>
    internal class ConduitTaskTypeSelectProvider
    {
        /// <summary>
        /// Provides the priority choices for selects.
        /// </summary>
        public static IEnumerable<DiscordSelectComponentOption> GetSelectOptions()
        {
            return new DiscordSelectComponentOption[]
            {
                    new DiscordSelectComponentOption("Bug", "[Bug]", null, false),
                    new DiscordSelectComponentOption("Feature", "[Feature]", null, false),
                    new DiscordSelectComponentOption("Docs", "[Docs]", null, false),
                    new DiscordSelectComponentOption("Research", "[Research]", null, false)
            };
        }
    }
}
