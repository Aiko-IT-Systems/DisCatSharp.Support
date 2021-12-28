using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Phabricator;
using DisCatSharp.Phabricator.Applications.Maniphest;
using DisCatSharp.Support.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Commands
{
    /// <summary>
    /// The support guild.
    /// </summary>
    internal class SupportGuild : ApplicationCommandsModule
    {
        [SlashCommand("bind", "Binds a user to an aitsys.dev account", false)]
        public static async Task BindUser(InteractionContext ctx, [Option("user", "User to bind to a aitsys.dev account")] DiscordUser user, [Autocomplete(typeof(ConduitUserProvider)), Option("account", "aitsys.dev account", true)] string account)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"We want to map {user.UsernameWithDiscriminator} to {account}"));
        }

        /// <summary>
        /// Adds a task to https://aitsys.dev.
        /// </summary>
        /// <param name="ctx">The interaction context.</param>
        /// <param name="title">The title.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="description">The description.</param>
        /// <returns>The url of the added task.</returns>
        [SlashCommand("task-add", "Add a task", false)]
        public static async Task AddTaskAsync(InteractionContext ctx, [Option("title", "Title of task")] string title, [ChoiceProvider(typeof(TaskPriorityChoiceProvider)), Option("priority", "Priority of task")] string priority, [Option("description", "Description of task")] string description)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = false, Content = "Generating Task..." });
            try
            {
                ConduitClient cc = new("https://aitsys.dev/", "api-7j3ohnnj45u4szrbzxkgodwjimxq");
                Maniphest m = new(cc);
                var projects = new List<string>
                {
                    "PHID-PROJ-n2ipblqjbyg7bjajhs5w",
                    "PHID-PROJ-vph6nqahxvu5ufcz226u",
                    "PHID-PROJ-kw4l4rrcmc3j6lrcxq4i"
                };
                var subscribers = new List<string>
                {
                    "PHID-PROJ-n2ipblqjbyg7bjajhs5w",
                    "PHID-PROJ-k3b4ls3iwze6r25t4teh",
                    "PHID-PROJ-tc4d56dldzwdi6tqxtow"
                };
                ManiphestTask task = new()
                {
                    Title = title,
                    Description = description,
                    Priority = priority

                };
                m.Edit(task);
                task.SetProjects(projects);
                task.SetSubscribers(subscribers);
                m.Edit(task);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task created: https://aitsys.dev/T{task.Identifier}"));
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = "Error",
                    Description = $"Exception: {ex.Message}\n" +
                    $"```\n" +
                    $"{ex.StackTrace}\n" +
                    $"```"
                });
            }
        }

        /// <summary>
        /// The task priority choice provider.
        /// </summary>
        public class TaskPriorityChoiceProvider : IChoiceProvider
        {
            /// <summary>
            /// Provides the priority choices.
            /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                return new DiscordApplicationCommandOptionChoice[]
                {
                    new DiscordApplicationCommandOptionChoice("Unbreak now!", "unbreak"),
                    new DiscordApplicationCommandOptionChoice("Triage", "triage"),
                    new DiscordApplicationCommandOptionChoice("High", "high"),
                    new DiscordApplicationCommandOptionChoice("Normal", "normal"),
                    new DiscordApplicationCommandOptionChoice("Low", "low"),
                    new DiscordApplicationCommandOptionChoice("Wishlist", "wish")
                };
            }
        }

        /// <summary>
        /// Shutdowns the bot down.
        /// </summary>
        /// <param name="ctx">The command context.</param>
        [SlashCommand("shutdown", "Bot shutdown", false)]
        public static async Task ShutdownAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Shutdown request"));
            if (ctx.Client.CurrentApplication.Team.Members.Where(x => x.User == ctx.User).Any() || ctx.User.Id == 856780995629154305)
            {
                await Task.Delay(5000);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Shutdown request accepted."));
                DisCatSharp.Support.Bot.ShutdownRequest.Cancel();
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Shuting down!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You are not allowed to execute this request!"));
            }
        }

        /// <summary>
        /// Shutdowns the bot down.
        /// </summary>
        /// <param name="ctx">The command context.</param>
        [SlashCommand("preview", "Guild preview", false)]
        public static async Task GetGuildPreviewAsync(InteractionContext ctx, [Option("invite_code", "The invite to check")] string invite_code)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading"));

            var invite = await ctx.Client.GetInviteByCodeAsync(invite_code);
            var features = invite.Guild.Features;
            var fstr = "";
            foreach(var f in features)
            {
                fstr += " " + f;
            }
            DiscordEmbedBuilder eb = new();
            eb.WithTitle(invite.Guild.Name);
            eb.WithAuthor("Discord Guild Lookup", invite.Url, invite.Guild.IconUrl);
            eb.WithDescription(invite.Guild.Description);
            eb.AddField("Features", Formatter.InlineCode(fstr));
            eb.AddField("ID", invite.Guild.Id.ToString());
            eb.AddField("Inviter", invite.Inviter != null ? invite.Inviter.Username + "#" + invite.Inviter.Discriminator : "None");
            eb.AddField("Verification Level", invite.Guild.VerificationLevel.ToString());
            eb.AddField("Vanity", invite.Guild.VanityUrlCode ?? "none");
            eb.WithImageUrl(invite.Guild.BannerUrl);
            eb.WithThumbnail(invite.Guild.SplashUrl);
            eb.WithFooter("Created " + invite.Guild.CreationTimestamp.DateTime.ToString());

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(eb.Build()));
        }

        /// <summary>
        /// Gets invite for a guild the bot is on.
        /// </summary>
        /// <param name="ctx">The command context.</param>
        [SlashCommand("get_invite", "Get a guild invite", false)]
        public static async Task GetGuildInvite(InteractionContext ctx, [Option("gid", "The guild id")] string gid)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading"));

            var guild = await ctx.Client.GetGuildAsync(Convert.ToUInt64(gid));
            var invite = await guild.GetDefaultChannel().CreateInviteAsync(0, 0, false);
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(invite.Url));
        }
    }
}
