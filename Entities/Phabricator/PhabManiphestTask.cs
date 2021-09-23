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
        public PhabManiphestTask(ManiphestTask source)
        {
            this.Builder = new();
            this.Builder.WithTitle(Formatter.Bold(source.Title));
            this.Builder.WithUrl($"{Bot.Config.ConduitConfig.ApiHost}T{source.Identifier}");
            this.Builder.WithDescription(source.Description != null ? source.Description.Replace("lang=", "") : "none");
            this.Builder.AddField("Status", source.StatusName ?? "unkown");
            this.Builder.AddField("Priority", source.PriorityName ?? "unkown");
            this.Builder.AddField("Created", Formatter.Timestamp(source.DateCreated, TimestampFormat.RelativeTime));
            this.Builder.AddField("Last modified", Formatter.Timestamp(source.DateModified, TimestampFormat.RelativeTime));
            this.Builder.Color = source.Status switch
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
