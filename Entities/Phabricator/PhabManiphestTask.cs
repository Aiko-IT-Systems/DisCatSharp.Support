using DisCatSharp.Entities;
using DisCatSharp.Phabricator.Applications.Maniphest;

namespace DisCatSharp.Support.Entities.Phabricator
{
    /// <summary>
    /// This class represents entities for maniphest tasks.
    /// </summary>
    internal class PhabManiphestTask
    {
        /// <summary>
        /// Gets the embed.
        /// </summary>
        public DiscordEmbed GetEmbed()
            => Builder.Build();

        /// <summary>
        /// Gets the builder.
        /// </summary>
        internal DiscordEmbedBuilder Builder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhabManiphestTask"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public PhabManiphestTask(ManiphestTask source, UserSearch user, Extended ext)
        {
            Builder = new();
            if (user != null && user.ErrorCode == null && ext != null)
            {
                var userobject = user.Result.Data[0];
                Builder.WithAuthor(userobject.Fields.RealName, ext.QResult[0].Uri, ext.QResult[0].Image);
                if (userobject.Fields.CustomDiscord.HasValue)
                {
                    var duser = Bot.DiscordClient.GetUserAsync(userobject.Fields.CustomDiscord.Value).Result;
                    Builder.AddField(new("Mapped Discord User", duser.Mention));
                    Builder.WithThumbnail(duser.AvatarUrl);
                }
            }
            Builder.WithTitle(Formatter.Bold(source.Title));
            Builder.WithUrl($"{Bot.ConduitClient.Url}T{source.Identifier}");
            Builder.WithDescription(source.Description != null ? source.Description.Replace("lang=", "") : "none");
            Builder.AddField(new("Status", source.StatusName ?? "unkown"));
            Builder.AddField(new("Priority", source.PriorityName ?? "unkown"));
            Builder.AddField(new("Created", Formatter.Timestamp(source.DateCreated, TimestampFormat.RelativeTime)));
            Builder.AddField(new("Last modified", Formatter.Timestamp(source.DateModified, TimestampFormat.RelativeTime)));
            Builder.Color = source.Status switch
            {
                "resolved" => DiscordColor.Green,
                "testing" => DiscordColor.Orange,
                "open" => DiscordColor.Yellow,
                "invalid" => DiscordColor.DarkRed,
                "wontfix" => DiscordColor.DarkRed,
                "duplicate" => DiscordColor.DarkBlue,
                _ => DiscordColor.Yellow,
            };
        }
    }
}
