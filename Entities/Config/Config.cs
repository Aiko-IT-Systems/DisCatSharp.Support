using Newtonsoft.Json;

namespace DisCatSharp.Support.Entities.Config
{
	/// <summary>
	/// The config.
	/// </summary>
	public partial class Config
	{
		/// <summary>
		/// Gets or sets the bot config.
		/// </summary>
		[JsonProperty("discord")]
		public DiscordConfig DiscordConfig { get; set; }

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
	}
}