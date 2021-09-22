using Newtonsoft.Json;

using System.Collections.Generic;

namespace DisCatSharp.Support.Entities.Config
{
    /// <summary>
    /// The phabricator conduit config.
    /// </summary>
    public partial class ConduitConfig
    {
        /// <summary>
        /// Gets or sets the api token.
        /// </summary>
        [JsonProperty("token")]
        public string ApiToken { get; set; }

        /// <summary>
        /// Gets or sets the api host.
        /// </summary>
        [JsonProperty("host")]
        public string ApiHost { get; set; }

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        [JsonProperty("projects")]
        public List<string> Projects { get; set; }

        /// <summary>
        /// Gets or sets the subscribers.
        /// </summary>
        [JsonProperty("subscribers")]
        public List<string> Subscribers { get; set; }
    }
}