using DisCatSharp.Entities;
using DisCatSharp.Enums;
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
                DiscordRole role1 = e.Guild.GetRole(917585444697440286);
				DiscordRole role2 = e.Guild.GetRole(858099411900956682);
				if (member.Roles.Contains(role1) && member.Roles.Contains(role2))
					await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You already accepted the rules.", IsEphemeral = true });
				else
                {
                    try
                    {
						await member.GrantRoleAsync(role1, "Rules accepted");
					}
                    catch (Exception) { }
                    try
					{
						await member.GrantRoleAsync(role2, "Rules accepted");
					}
                    catch (Exception) { }
					await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Welcome to DisCatSharp!\n\nTake a look into <#891500835543056384> and say hi in <#859253281741078539> :3.", IsEphemeral = true });
                }
            }
            else if (e.Id == "join_hacktober")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(1024042883264827546);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You already take part :3", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role, "Joined Hacktoberfest");
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "We're happy to have you on board!", IsEphemeral = true });
                }
			}
			else if (e.Id == "leave_hacktober")
			{
				await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
				DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
				DiscordRole role = e.Guild.GetRole(1024042883264827546);
				if (member.Roles.Contains(role))
				{
					await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You don't take part :(.", IsEphemeral = true });
				}
				else
				{
					await member.RevokeRoleAsync(role, "Left Hacktoberfest");
					await e.Interaction.CreateFollowupMessageAsync(new() { Content = "We hope you'll stay anyways <3", IsEphemeral = true });
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
