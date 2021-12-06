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
            if (e.Id == "ack")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            }
            else if (e.Id == "rules-accept-858089281214087179")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(858099411900956682);
                DiscordRole muterole = e.Guild.GetRole(888477455105544274);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You already accepted the rules.", IsEphemeral = true });
                }
                else if (member.Roles.Contains(muterole))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You are muted!.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role, "Rules accepted");
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Rules accepted.", IsEphemeral = true });
                }
            }
            else if (e.Id == "changelog-pings-858089281214087179")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(872657990556200981);
                DiscordRole muterole = e.Guild.GetRole(888477455105544274);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You already enabled changelog pings.\nUse /selfroles to disable it.", IsEphemeral = true });
                }
                else if (member.Roles.Contains(muterole))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You are muted!.", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role, "Changelog ping enabled");
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You will be notified on important updates.\nUse /selfroles to disable it.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-891501159481745409-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501159481745409);
                DiscordRole muterole = e.Guild.GetRole(888477455105544274);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else if (member.Roles.Contains(muterole))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Warning: You are muted!", IsEphemeral = true });
                }
                else
                {
                    await member.GrantRoleAsync(role);
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Success: Role assigned.", IsEphemeral = true });
                }
            }
            else if (e.Id == $"selfrole-891501159481745409-remove")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501159481745409);
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
            else if (e.Id == $"selfrole-891501151592251412-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501151592251412);
                DiscordRole muterole = e.Guild.GetRole(888477455105544274);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else if (member.Roles.Contains(muterole))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Warning: You are muted!", IsEphemeral = true });
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
            else if (e.Id == $"selfrole-891501156013072404-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501156013072404);
                DiscordRole muterole = e.Guild.GetRole(888477455105544274);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else if (member.Roles.Contains(muterole))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Warning: You are muted!", IsEphemeral = true });
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
            else if (e.Id == $"selfrole-891501167081816074-add")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole role = e.Guild.GetRole(891501167081816074);
                DiscordRole muterole = e.Guild.GetRole(888477455105544274);
                if (member.Roles.Contains(role))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Error: You already got the role.", IsEphemeral = true });
                }
                else if (member.Roles.Contains(muterole))
                {
                    await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Warning: You are muted!", IsEphemeral = true });
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
            else
            {
                try
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
                catch (NotFoundException) { /* This interaction was already responded to. */ }
            }
        }
    }
}
