using Newtonsoft.Json;

namespace DisCatSharp.Support.Entities.Config
{
    /// <summary>
    /// The database config.
    /// </summary>
    public partial class DatabaseConfig
    {
        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        [JsonProperty("db")]
        public string Db { get; set; }
    }
}