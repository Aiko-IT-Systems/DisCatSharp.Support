using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using DisCatSharp.Support.Providers;

using Stwalkerster.SharphConduit.Applications.Maniphest;

using System;
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
        /// Adds a task to https://bugs.aitsys.dev.
        /// </summary>
        /// <param name="ctx">The interaction context.</param>
        /// <param name="type">The type.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <returns>The url of the added task.</returns>
        [SlashCommand("task", "Add a task")]
        public static async Task AddTaskAsync(InteractionContext ctx, 
            [ChoiceProvider(typeof(IConduitTaskTypeProvider)), Option("type", "Type")] string type,
            [ChoiceProvider(typeof(IConduitTaskPriorityProvider)), Option("priority", "Priority")] string priority,
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
    }
}