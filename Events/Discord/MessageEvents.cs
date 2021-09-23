using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.EventArgs;

using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;
using Stwalkerster.SharphConduit.Applications.Maniphest;
using Stwalkerster.SharphConduit;
using System.Collections.Generic;
using System.Linq;

namespace DisCatSharp.Support.Events.Discord
{
    /// <summary>
    /// The message events.
    /// </summary>
    internal class MessageEvents
    {
        /// <summary>
        /// Fired when a new message gets created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        public static async Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if(e.Guild.Id == Bot.Config.DiscordConfig.ApplicationCommandConfig.Dcs.GuildId)
            {
                Regex reg = new(@"(?:https:\/\/bugs.aitsys.dev\/T(\d{1,4)|(?:T)(\d{1,4}))");
                Match match = reg.Match(e.Message.Content);
                if(match.Success)
                {
                    await SearchAndSendTaskAsync(match, e.Message, e.Channel);
                }
            }
            else
            {
                await Task.FromResult(true);
            }
        }

        private static async Task<DiscordMessage> SearchAndSendTaskAsync(Match match, DiscordMessage message, DiscordChannel channel)
        {
            Maniphest m = new(Bot.ConduitClient);
            List<ApplicationEditorSearchConstraint> search = new();
            List<int> ids = new();
            ids.Add(Convert.ToInt32(match.Groups[2]));
            search.Add(new("ids", ids));
            var task = m.Search(null, search).First();
        }
    }
}
