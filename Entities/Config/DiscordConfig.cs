using Newtonsoft.Json;

namespace DisCatSharp.Support.Entities.Config
{
    /// <summary>
    /// The discord config.
    /// </summary>
    public partial class DiscordConfig
    {
        /// <summary>
        /// Gets or sets the bot token.
        /// </summary>
        [JsonProperty("bot_token")]
        public string BotToken { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the guild application command config.
        /// </summary>
        [JsonProperty("guild_ac")]
        public ApplicationCommandConfig ApplicationCommandConfig { get; set; }

    }
}