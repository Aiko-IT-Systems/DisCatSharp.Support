using Newtonsoft.Json;

namespace DisCatSharp.Support.Entities
{
    /// <summary>
    /// The config.
    /// </summary>
    public partial class Config
    {
        /// <summary>
        /// Gets or sets the bot token.
        /// </summary>
        [JsonProperty("bot_token")]
        public string BotToken { get; set; }

        /// <summary>
        /// Gets or sets the database config.
        /// </summary>
        [JsonProperty("database")]
        public DatabaseConfig DatabaseConfig { get; set; }

        /// <summary>
        /// Gets or sets the phabricator conduit config.
        /// </summary>
        [JsonProperty("conduit")]
        public ConduitConfig ConduitConfig { get; set; }

        /// <summary>
        /// Gets or sets the guild application command config.
        /// </summary>
        [JsonProperty("guild_ac")]
        public ApplicationCommandConfig ApplicationCommandConfig { get; set; }
    }
}