using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Support.Providers;

using Org.BouncyCastle.Crypto;

using Stwalkerster.SharphConduit;
using Stwalkerster.SharphConduit.Applications.Maniphest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Commands.Tasks
{
    /// <summary>
    /// The conduit tasks.
    /// </summary>
    [SlashCommandGroup("tasks", "Tasks management for bugs.aitsys.dev", false)]
    public class ConduitTasks : ApplicationCommandsModule
    {
        /// <summary>
        /// Creates a task for https://bugs.aitsys.dev.
        /// </summary>
        /// <param name="ctx">The interaction context.</param>
        /// <param name="type">The type.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <returns>The url of the added task.</returns>
        [SlashCommand("create", "Creates a task")]
        public static async Task CreateTaskAsync(InteractionContext ctx, 
            [ChoiceProvider(typeof(ConduitTaskTypeProvider)), Option("type", "Type")] string type,
            [ChoiceProvider(typeof(ConduitTaskPriorityProvider)), Option("priority", "Priority")] string priority,
            [Option("title", "Title")] string title,
            [Option("description", "Description")] string description)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = false, Content = "Generating Task..." });
            try
            {
                Maniphest m = new(Bot.ConduitClient);
                ManiphestTask task = new()
                {
                    Title = $"{type} {title}",
                    Description = description,
                    Priority = priority

                };
                m.Edit(task);
                task.SetProjects(Bot.Config.ConduitConfig.Projects);
                task.SetSubscribers(Bot.Config.ConduitConfig.Subscribers);
                m.Edit(task);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task created: https://bugs.aitsys.dev/T{task.Identifier}"));
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
        /// Closes a task.
        /// </summary>
        /// <param name="ctx">The interaction context.</param>
        /// <param name="task_id">The task_id.</param>
        /// <returns>A Task.</returns>
        [SlashCommand("close", "Closes a task")]
        public static async Task CloseTaskAsync(InteractionContext ctx,
            [Option("task_id", "The task id")] string task_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = false, Content = "Deleting Task..." });
            try
            {
                Maniphest m = new(Bot.ConduitClient);
                List<ApplicationEditorSearchConstraint> search = new();
                List<int> ids = new();
                ids.Add(Convert.ToInt32(task_id));
                search.Add(new("ids", ids));
                var task = m.Search(null, search).First();
                task.Status = "resolved";
                m.Edit(task);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task closed: {Formatter.Bold(task.Title)}\nhttps://bugs.aitsys.dev/T{task.Identifier}"));
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
    }
}