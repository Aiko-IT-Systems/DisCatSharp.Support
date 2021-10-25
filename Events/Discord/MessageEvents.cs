using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Phabricator;
using DisCatSharp.Phabricator.Applications.Maniphest;
using DisCatSharp.Support.Entities.Phabricator;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            await Task.Run(async() =>
            {
                if (e.Guild != null && e.Guild.Id == Bot.Config.DiscordConfig.ApplicationCommandConfig.Dcs.GuildId)
                {
                    Regex reg = new(@"\b(?:https:\/\/(?:bugs\.)?aitsys\.dev\/T(\d{1,4)|(?:T)(\d{1,4}))\b");
                    Match match = reg.Match(e.Message.Content);
                    if (match.Success)
                    {
                        await SearchAndSendTaskAsync(match, e.Message, e.Channel);
                    }
                    else
                    {
                        await Task.FromResult(true);
                    }
                }
            });
        }

        /// <summary>
        /// Searches and sends a task.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="message">The message.</param>
        /// <param name="channel">The channel.</param>
        private static async Task<DiscordMessage> SearchAndSendTaskAsync(Match match, DiscordMessage message, DiscordChannel channel)
        {
            try
            {
                Maniphest m = new(Bot.ConduitClient);
                List<ApplicationEditorSearchConstraint> search = new();
                List<int> ids = new();
                ids.Add(Convert.ToInt32(match.Groups[2].Value));
                search.Add(new("ids", ids));
                var task = m.Search(null, search).First();
                UserSearch user = null;
                Extended extuser = null;
                if (!string.IsNullOrEmpty(task.Owner))
                {
                    var searchUser = new Dictionary<string, dynamic>();
                    string[] phids = { task.Owner };
                    searchUser.Add("phids", phids);
                    var constraints = new Dictionary<string, dynamic>
                {
                    { "constraints", searchUser }
                };
                    var tdata = Bot.ConduitClient.CallMethod("user.search", constraints);
                    var data = JsonConvert.SerializeObject(tdata);

                    user = JsonConvert.DeserializeObject<UserSearch>(data);
                    var username = new List<string>
                {
                    user.Result.Data[0].Fields.Username
                };

                    var extconstraints = new Dictionary<string, dynamic>
                {
                    { "usernames", username }
                };
                    var tdata2 = Bot.ConduitClient.CallMethod("user.query", extconstraints);
                    var data2 = JsonConvert.SerializeObject(tdata2);
                    extuser = JsonConvert.DeserializeObject<Extended>(data2);
                }
                PhabManiphestTask embed = new(task, user, extuser);
                DiscordMessageBuilder builder = new();
                builder.AddEmbed(embed.GetEmbed());
                return await message.RespondAsync(builder);
            }
            catch (Exception ex)
            {
                return await channel.SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = "Error",
                    Description = $"Exception: {ex.Message}\n" +
                    $"```\n" +
                    $"{ex.StackTrace}\n" +
                    $"```"
                });
            }
        }
    }
}
