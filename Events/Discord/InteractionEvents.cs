using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Exceptions;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Events.Discord
{
    internal class InteractionEvents
    {
        public static async Task InteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            #region Default ack
            if (e.Id == "ack")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            }
            #endregion
            #region Rules
            else if (e.Id == "rules-accept-858089281214087179")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917585444697440286);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You already accepted the rules.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role, "Rules accepted");
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Rules accepted.", IsEphemeral = true });
                }
            }
            else if (e.Id == "member-ready")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(858099411900956682);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You are already a member.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role, "Rules accepted");
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Welcome to DisCatSharp!\n\nTake a look into <#891500835543056384> and <#917583400162967552> :3", IsEphemeral = true });
                }
            }
            #endregion
            #region Pings
            else if (e.Id == "selfrole-announcements")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917608912151248927);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == "selfrole-ping-changelogs")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(872657990556200981);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == "selfrole-ping-serverupdates")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917608674619449398);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == "selfrole-ping-polls")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501159481745409);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == "selfrole-ping-gaming")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917608695301550090);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Ping role assigned.", IsEphemeral = true });
                }
            }
            #endregion
            #region Projects
            else if (e.Id == $"selfrole-dcs")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917609333716578324);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Project role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Project role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-dcsm")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917609327987138641);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Project role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Project role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-dcsp")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(917609330877026355);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Project role removed.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Project role assigned.", IsEphemeral = true });
                }
            }
            #endregion
            #region View Git Logs
            else if (e.Id == $"selfrole-891501151592251412-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501151592251412);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-891501151592251412-remove")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501151592251412);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role removed.", IsEphemeral = true });
                }
                else
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You don't have the role.", IsEphemeral = true });
                }
            }
            #endregion
            #region View Commits
            else if (e.Id == $"selfrole-891501156013072404-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501156013072404);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-891501156013072404-remove")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501156013072404);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role removed.", IsEphemeral = true });
                }
                else
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You don't have the role.", IsEphemeral = true });
                }
            }
            #endregion
            #region View Discord Commits
            else if (e.Id == $"selfrole-891501167081816074-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501167081816074);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-891501167081816074-remove")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501167081816074);
                if (member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role removed.", IsEphemeral = true });
                }
                else
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You don't have the role.", IsEphemeral = true });
                }
            }
            #endregion
            #region If nothing else
            else
            {
                try
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
                catch (NotFoundException) { /* This interaction was already responded to. */ }
				catch (BadRequestException) { /* This interaction was already responded to. */ }
			}
            #endregion
        }
    }
}
