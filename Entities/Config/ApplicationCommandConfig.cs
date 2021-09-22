using Newtonsoft.Json;

using System.Collections.Generic;

namespace DisCatSharp.Support.Entities.Config
{
    /// <summary>
    /// The application command config.
    /// </summary>
    public partial class ApplicationCommandConfig
    {
        /// <summary>
        /// Gets or sets the DisCatSharp guild.
        /// </summary>
        [JsonProperty("dcs")]
        public AcGuildConfig Dcs { get; set; }

        /// <summary>
        /// Gets or sets the DisCatSharp Development guild.
        /// </summary>
        [JsonProperty("dcs_dev")]
        public AcGuildConfig DcsDev { get; set; }
    }

    /// <summary>
    /// The application command guild config.
    /// </summary>
    public partial class AcGuildConfig
    {
        /// <summary>
        /// Gets or sets the guild id.
        /// </summary>
        [JsonProperty("id")]
        public ulong GuildId { get; set; }

        /// <summary>
        /// Gets or sets the role ids.
        /// </summary>
        [JsonProperty("roles")]
        public List<ulong> RoleIds { get; set; }
    }
}