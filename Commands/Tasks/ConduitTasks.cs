using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Phabricator;
using DisCatSharp.Phabricator.Applications.Maniphest;
using DisCatSharp.Support.Entities.Phabricator;
using DisCatSharp.Support.Providers;
using DisCatSharp.Helpers;

using Newtonsoft.Json;

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
                    Priority = priority,
                    EditPolicy = Bot.Config.ConduitConfig.Subscribers[0]

                };
                m.Edit(task);
                task.SetProjects(Bot.Config.ConduitConfig.Projects);
                task.SetSubscribers(Bot.Config.ConduitConfig.Subscribers);
                m.Edit(task);
                List<ApplicationEditorSearchConstraint> search = new();
                List<int> ids = new()
                {
                    task.Identifier
                };
                search.Add(new("ids", ids));
                var stask = m.Search(null, search).First();
                PhabManiphestTask etask = new(stask, null, null);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task created: {Bot.ConduitClient.Url}T{task.Identifier}").AddEmbed(etask.GetEmbed()));
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
        /// Views a task.
        /// </summary>
        /// <param name="ctx">The interaction context.</param>
        /// <param name="task_id">The task_id.</param>
        /// <returns>A Task.</returns>
        [SlashCommand("view", "Views a task")]
        public static async Task ViewTaskAsync(InteractionContext ctx,
            [Autocomplete(typeof(ConduitTaskProvider)), Option("task_id", "The task", true)] string task_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = false, Content = "Fetching Task..." });
            try
            {
                Maniphest m = new(Bot.ConduitClient);
                List<ApplicationEditorSearchConstraint> search = new();
                List<int> ids = new()
                {
                    Convert.ToInt32(task_id)
                };
                search.Add(new("ids", ids));
                var task = m.Search(null, search).First();
                UserSearch user = null;
                Extended extuser = null;
                if (!string.IsNullOrEmpty(task.Owner))
                {
                    var searchUser = new Dictionary<string, dynamic>();
                    string[] phids = { task.Owner };
                    searchUser.Add("phids", phids);
                    var constraints = new Dictionary<string, dynamic>
                {
                    { "constraints", searchUser }
                };
                    var tdata = Bot.ConduitClient.CallMethod("user.search", constraints);
                    var data = JsonConvert.SerializeObject(tdata);

                    user = JsonConvert.DeserializeObject<UserSearch>(data);
                    var username = new List<string>
                {
                    user.Result.Data[0].Fields.Username
                };

                    var extconstraints = new Dictionary<string, dynamic>
                {
                    { "usernames", username }
                };
                    var tdata2 = Bot.ConduitClient.CallMethod("user.query", extconstraints);
                    var data2 = JsonConvert.SerializeObject(tdata2);
                    extuser = JsonConvert.DeserializeObject<Extended>(data2);
                }
                PhabManiphestTask embed = new(task, user, extuser);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task informations for {Formatter.Bold(task.Title)}\n{Bot.ConduitClient.Url}T{task.Identifier}").AddEmbed(embed.GetEmbed()));
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
            [Autocomplete(typeof(ConduitOpenTaskProvider)), Option("task_id", "The task", true)] string task_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = false, Content = "Closing Task..." });
            try
            {
                Maniphest m = new(Bot.ConduitClient);
                List<ApplicationEditorSearchConstraint> search = new();
                List<int> ids = new()
                {
                    Convert.ToInt32(task_id)
                };
                search.Add(new("ids", ids));
                var task = m.Search(null, search).First();
                task.Status = "resolved";
                m.Edit(task);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task closed: {Formatter.Bold(task.Title)}\n{Bot.ConduitClient.Url}T{task.Identifier}"));
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

    /// <summary>
    /// The conduit tasks context menu.
    /// </summary>
    public class ConduitTasksContextMenu : ApplicationCommandsModule
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
        [ContextMenu(ApplicationCommandType.Message, "Create a task", false)]
        public static async Task CreateTaskFromMessageAsync(ContextMenuContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = false, Content = "Generating Task..." });
            try
            {
                var interactivity = ctx.Client.GetInteractivity();

                var type_select = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddComponents(new DiscordSelectComponent($"task-create-type-{ctx.TargetMessage.Id}", "Select the type of the task", ConduitTaskTypeSelectProvider.GetSelectOptions(), 1, 1, false)).WithContent("What's the type?"));

                var type_request = await interactivity.WaitForSelectAsync(type_select, x => x.Id == $"task-create-type-{ctx.TargetMessage.Id}" && x.User.Id == ctx.User.Id, TimeSpan.FromSeconds(60));

                if (type_request.TimedOut)
                {
                    await ctx.DeleteFollowupAsync(type_select.Id);
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Timed out. Task creation failed"));
                    return;
                }

                await ctx.DeleteFollowupAsync(type_select.Id);

                var type = type_request.Result.Values[0];

                var prio_select = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddComponents(new DiscordSelectComponent($"task-create-prio-{ctx.TargetMessage.Id}", "Select the priority of this task", ConduitTaskPrioritySelectProvider.GetSelectOptions(), 1, 1, false)).WithContent("What's the priority?"));

                var prio_request = await interactivity.WaitForSelectAsync(prio_select, x => x.Id == $"task-create-prio-{ctx.TargetMessage.Id}" && x.User.Id == ctx.User.Id, TimeSpan.FromSeconds(60));

                if (prio_request.TimedOut)
                {
                    await ctx.DeleteFollowupAsync(prio_select.Id);
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Timed out. Task creation failed"));
                    return;
                }

                await ctx.DeleteFollowupAsync(prio_select.Id);

                var priority = prio_request.Result.Values[0];

                var title_response = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("What's the title of the task?"));

                var title_request = await interactivity.WaitForMessageAsync(m => m.Author.Id == ctx.User.Id && m.ChannelId == ctx.Channel.Id, TimeSpan.FromSeconds(60));

                if (title_request.TimedOut)
                {
                    await ctx.DeleteFollowupAsync(title_response.Id);
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Timed out. Task creation failed"));
                    return;
                }

                await ctx.DeleteFollowupAsync(title_response.Id);

                var title = title_request.Result.Content;

                await title_request.Result.DeleteAsync($"Interaction [task-create-{ctx.TargetMessage.Id}]");

                Maniphest m = new(Bot.ConduitClient);
                ManiphestTask task = new()
                {
                    Title = $"{type} {title}",
                    Description = ctx.TargetMessage.Content + $"\n\nTicket source: {ctx.TargetMessage.JumpLink.AbsoluteUri}",
                    Priority = priority,
                    EditPolicy = Bot.Config.ConduitConfig.Subscribers[0]

                };
                m.Edit(task);
                task.SetProjects(Bot.Config.ConduitConfig.Projects);
                task.SetSubscribers(Bot.Config.ConduitConfig.Subscribers);
                m.Edit(task);
                List<ApplicationEditorSearchConstraint> search = new();
                List<int> ids = new()
                {
                    task.Identifier
                };
                search.Add(new("ids", ids));
                var stask = m.Search(null, search).First();
                PhabManiphestTask etask = new(stask, null, null);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Task created: {Bot.ConduitClient.Url}T{task.Identifier}").AddEmbed(etask.GetEmbed()));
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

namespace DisCatSharp.Helpers
{
    /// <summary>
    /// Useful helper functions for <see cref="DiscordChannel">s.
    /// </summary>
    public class ChannelHelper
    {
        /// <summary>
        /// Gets an ordered <see cref="DiscordChannel"> list.
        /// Returns a <see cref="Dictionary"> where the key is an <see cref="ulong"> and can be mapped to <see cref="ChannelType.Category"> <see cref="DiscordChannel">s.
        /// Ignore the 0 key here, because that indicates that this is the "has no category" list.
        /// Each value contains a ordered list of text/news & voice/stage channels as <see cref="DiscordChannel">.
        /// </summary>
        /// <param name="guild">The <see cref="DiscordGuild"> to fetch the <see cref="DiscordChannel">s from.</param>
        /// <returns>A ordered list of categories with its channels</returns>
        public static async Task<Dictionary<ulong, List<DiscordChannel>>> GetOrderedChannelAsync(DiscordGuild guild)
        {
            IReadOnlyList<DiscordChannel> raw_channels = await guild.GetChannelsAsync();

            Dictionary<ulong, List<DiscordChannel>> ordered_channels = new()
            {
                { 0, new List<DiscordChannel>() }
            };

            foreach (var channel in raw_channels.Where(c => c.Type == ChannelType.Category).OrderBy(c => c.Position))
            {
                ordered_channels.Add(channel.Id, new List<DiscordChannel>());
            }

            foreach (var channel in raw_channels.Where(c => c.ParentId.HasValue && (c.Type == ChannelType.Text || c.Type == ChannelType.News)).OrderBy(c => c.Position))
            {
                ordered_channels[channel.ParentId.Value].Add(channel);
            }
            foreach (var channel in raw_channels.Where(c => c.ParentId.HasValue && (c.Type == ChannelType.Voice || c.Type == ChannelType.Stage)).OrderBy(c => c.Position))
            {
                ordered_channels[channel.ParentId.Value].Add(channel);
            }

            foreach (var channel in raw_channels.Where(c => !c.ParentId.HasValue && c.Type != ChannelType.Category && (c.Type == ChannelType.Text || c.Type == ChannelType.News)).OrderBy(c => c.Position))
            {
                ordered_channels[0].Add(channel);
            }
            foreach (var channel in raw_channels.Where(c => !c.ParentId.HasValue && c.Type != ChannelType.Category && (c.Type == ChannelType.Voice || c.Type == ChannelType.Stage)).OrderBy(c => c.Position))
            {
                ordered_channels[0].Add(channel);
            }

            return ordered_channels;
        }
    }
}