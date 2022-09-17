using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Providers
{
    /// <summary>
    /// The guild provider.
    /// </summary>
    internal class GuildProvider : IAutocompleteProvider
    {
        /// <summary>
        /// Provides the autocomplete choices for the current bot guilds.
        /// </summary>
        /// <param name="context">The autocomplete context.</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var guilds = Bot.DiscordClient.Guilds.Values;

            if (context.FocusedOption == null)
            {
                return null;
            }

            var filtered_guilds = context.FocusedOption.Value == null ? guilds : guilds.Where(t => t.Name.ToLower().Contains(context.FocusedOption.Value.ToString().ToLower()));

            if (!filtered_guilds.Any())
            {
                Console.WriteLine("Nothing found B");
                return null;
            }

            List<DiscordApplicationCommandAutocompleteChoice> choices = new();
            int i = 0;
            foreach (var task in filtered_guilds)
            {
                if (i >= 24)
                    break;

                choices.Add(new DiscordApplicationCommandAutocompleteChoice(task.Name, task.Id.ToString()));
                i++;
            }
            return choices;
        }
    }
}
