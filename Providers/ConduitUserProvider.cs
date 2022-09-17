using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Phabricator;
using DisCatSharp.Phabricator.Applications.Maniphest;
using DisCatSharp.Support.Entities.Phabricator;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Providers
{
    /// <summary>
    /// The conduit user provider.
    /// </summary>
    internal class ConduitUserProvider : IAutocompleteProvider
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
            var searchUser = new Dictionary<string, dynamic>
            {
                { "nameLike", context.FocusedOption.Value.ToString().ToLower() }
            };
            var constraints = new Dictionary<string, dynamic>
            {
                { "constraints", searchUser }
            };
            var tdata = Bot.ConduitClient.CallMethod("user.search", constraints);
            var data = JsonConvert.SerializeObject(tdata);
            UserSearch user = JsonConvert.DeserializeObject<UserSearch>(data);


            if (!user.Result.Data.Any())
            {
                return null;
            }

            List<DiscordApplicationCommandAutocompleteChoice> choices = new();
            int i = 0;
            foreach (var phabUser in user.Result.Data)
            {
                if (i >= 24)
                    continue;

                choices.Add(new DiscordApplicationCommandAutocompleteChoice(phabUser.Fields.Username.ToLower().Trim(), phabUser.Phid));
                i++;
            }
            return choices;
        }
    }
}
