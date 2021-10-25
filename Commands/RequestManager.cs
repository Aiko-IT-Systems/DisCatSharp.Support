using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Phabricator;
using DisCatSharp.Phabricator.Applications.Maniphest;
using DisCatSharp.Support.Entities.Phabricator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Commands
{
    /// <summary>
    /// The request manager.
    /// </summary>
    [SlashCommandGroup("request", "Request management", true)]
    internal class RequestManager : ApplicationCommandsModule
    {
        /// <summary>
        /// Resuests a new feature.
        /// </summary>
        /// <param name="ic">The interaction context.</param>
        /// <param name="module">Target module.</param>
        [SlashCommand("add", "Add a new request", true)]
        public static async Task RequestFeatureAsync(InteractionContext ctx,
            [Choice("DisCatSharp", "[Core]"),
            Choice("DisCatSharp.ApplicationCommands", "[ApplicationCommands]"),
            Choice("DisCatSharp.CommandsNext", "[CommandsNext]"),
            Choice("DisCatSharp.Lavalink", "[Lavalink]"),
            Choice("DisCatSharp.VoiceNext", "[VoiceNext]"),
            Choice("DisCatSharp.Interactivity", "[Interactivity]"),
            Choice("DisCatSharp.Phabricator", "[Phabricator]"),
            Option("module", "For which module do you want a new feature")] string module)
        {
            //await ic.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Not implemented, but we know already, that you want a feature for {module.Replace("[", "").Replace("]", "")}").AsEphemeral(false));
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Please take a look in my dm").AsEphemeral(false));
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Mention} is creating a request"));
            var interactivity = ctx.Client.GetInteractivity();
            
            DiscordMessageBuilder mb = new();
            mb.WithContent("Whats the title of the feature request?");
            
            var title_request = await ctx.Member.SendMessageAsync(mb);
            var title_result = await interactivity.WaitForMessageAsync(wfm => wfm.Author == ctx.User && wfm.Channel.IsPrivate, TimeSpan.FromSeconds(60));
            
            if (title_result.TimedOut)
            {
                await title_request.Channel.SendMessageAsync("Timed out. Aborting request");
                await ctx.DeleteResponseAsync();
                return;
            }

            var title = title_result.Result.Content;

            mb.Clear();
            mb.WithContent($"What should the description be.\nTry to be as accurate as possible and use markdown. To add code use {Formatter.Sanitize("```lang=langauage")}.\n\nWrite CONTINUE to get to the next step.");
            var description_request = await ctx.Member.SendMessageAsync(mb);
            List<string> description_parts = new();
            Bot.DiscordClient.MessageCreated += (sender, args) => Task.Run(() => {
                if (args.Author == ctx.User && args.Channel.IsPrivate && args.Message.Content != "CONTINUE")
                {
                    description_parts.Add(args.Message.Content);
                }
            });
            var description_result = await interactivity.WaitForMessageAsync(wfm => wfm.Author == ctx.User && wfm.Channel.IsPrivate && wfm.Content == "CONTINUE", TimeSpan.FromMinutes(5));

            if (description_result.TimedOut)
            {
                Bot.DiscordClient.MessageCreated -= (sender, args) => Task.CompletedTask;
                await description_request.Channel.SendMessageAsync("Timed out. Aborting request");
                await ctx.DeleteResponseAsync();
                return;
            }

            Bot.DiscordClient.MessageCreated -= (sender, args) => Task.CompletedTask;
            var description = "";
            foreach(var part in description_parts)
            {
                description += part + Environment.NewLine;
            }

            DiscordEmbedBuilder eb = new();
            eb.WithTitle($"{module} {title}");
            eb.WithDescription(description.Replace("lang=", ""));

            mb.Clear();
            mb.WithContent("Are you sure you want to send this request?");
            mb.AddEmbed(eb.Build());
            mb.AddComponents(new DiscordButtonComponent(ButtonStyle.Success, $"{ctx.User.Id}-{title_request.Id}-yes", "Yes", false, new DiscordComponentEmoji(887058605101183017)), new DiscordButtonComponent(ButtonStyle.Danger, $"{ctx.User.Id}-{title_request.Id}-no", "No", false, new DiscordComponentEmoji(887058605247987743)));
            var confirm_request = await ctx.Member.SendMessageAsync(mb);
            var confirm_result = await interactivity.WaitForButtonAsync(confirm_request, wfb => wfb.User == ctx.User && wfb.Channel.IsPrivate && wfb.Id.StartsWith($"{ctx.User.Id}-{title_request.Id}-"), TimeSpan.FromSeconds(30));

            if (confirm_result.TimedOut)
            {
                await confirm_request.Channel.SendMessageAsync("Timed out. Aborting request");
                await ctx.DeleteResponseAsync();
                return;
            } 
            else if(confirm_result.Result.Id == $"{ctx.User.Id}-{title_request.Id}-no")
            {
                await confirm_result.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Request aborted."));
                await ctx.DeleteResponseAsync();
                return;
            } 
            else if(confirm_result.Result.Id == $"{ctx.User.Id}-{title_request.Id}-yes")
            {
                await confirm_result.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Request will be send."));
                try
                {
                    var end_request = await confirm_request.Channel.SendMessageAsync("Sending request..");
                    Maniphest m = new(Bot.ConduitClient);
                    ManiphestTask task = new()
                    {
                        Title = $"{module} {title}",
                        Description = description,
                        Priority = "wish",
                        EditPolicy = Bot.Config.ConduitConfig.Subscribers[0]

                    };
                    m.Edit(task);
                    task.SetProjects(Bot.Config.ConduitConfig.Projects);
                    task.SetSubscribers(Bot.Config.ConduitConfig.Subscribers);
                    m.Edit(task);
                    List<ApplicationEditorSearchConstraint> search = new();
                    List<int> ids = new();
                    ids.Add(task.Identifier);
                    search.Add(new("ids", ids));
                    var stask = m.Search(null, search).First();
                    PhabManiphestTask etask = new(stask, null, null);
                    var msg = await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"New feature request: {Bot.ConduitClient.Url}T{task.Identifier}").AddEmbed(etask.GetEmbed()));
                    await msg.CreateReactionAsync(DiscordEmoji.FromGuildEmote(Bot.DiscordClient, 887058605101183017));
                    await msg.CreateReactionAsync(DiscordEmoji.FromGuildEmote(Bot.DiscordClient, 887058605247987743));
                    await end_request.ModifyAsync($"Request send.\nSee {Bot.ConduitClient.Url}T{task.Identifier}");
                } catch(Exception ex)
                {
                    await ctx.Member.SendMessageAsync(new DiscordEmbedBuilder()
                    {
                        Title = "Error",
                        Description = $"Exception: {ex.Message}\n" +
                            $"```\n" +
                            $"{ex.StackTrace}\n" +
                            $"```"
                    });
                    await ctx.DeleteResponseAsync();
                    return;
                }
            }
        }
    }
}
