using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Phabricator.Applications.Maniphest;
using DisCatSharp.Phabricator;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

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
                    new DiscordApplicationCommandOptionChoice("Unbreak now!", "ubreak"),
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
                    new DiscordSelectComponentOption("Unbreak now!", "ubreak", null, false),
                    new DiscordSelectComponentOption("Needs Triage", "triage", null, false),
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

    /// <summary>
    /// The conduit task provider.
    /// </summary>
    internal class ConduitTaskProvider : IAutocompleteProvider
    {
        /// <summary>
        /// Providers the.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A Task.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (context.FocusedOption == null)
            {
                return null;
            }

            Maniphest m = new(Bot.ConduitClient);
            List<ApplicationEditorSearchConstraint> search = new();
            List<string> projects = new();
            projects.Add("DisCatSharp");
            search.Add(new("projects", projects));
            var tasks = m.Search("open", search);
            if (!tasks.Any())
            {
                return null;
            }

            string tsearch = context.FocusedOption.Value as string;
            var filtered_tasks = tasks.Where(t => t.Title.ToLower().Contains(tsearch.ToLower()));

            if (!filtered_tasks.Any())
            {
                return null;
            }

            List<DiscordApplicationCommandAutocompleteChoice> choices = new();
            int i = 0;
            foreach(var task in filtered_tasks)
            {
                if (i > 25)
                    continue;

                choices.Add(new DiscordApplicationCommandAutocompleteChoice(task.Title, task.Identifier.ToString()));
                i++;
            }
            return choices;
        }
    }
}
