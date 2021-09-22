using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
