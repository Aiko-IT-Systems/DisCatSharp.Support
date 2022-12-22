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
            else if (e.Id == "dcs_rules")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new() { IsEphemeral = true });
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole userRole = e.Guild.GetRole(1055218048879054879);
				if (member.Roles.Contains(userRole))
					await e.Interaction.CreateFollowupMessageAsync(new() { Content = "You already accepted the rules.", IsEphemeral = true });
				else
                {
                    try
                    {
						await member.GrantRoleAsync(userRole, "Rules accepted");
					}
                    catch (Exception) { }
                    
					await e.Interaction.CreateFollowupMessageAsync(new() { Content = "Welcome to DisCatSharp!", IsEphemeral = true });
                }
            }
			#endregion
			#region Ping Roles

			#endregion
			#region Section Roles

			#endregion

			#region Topic Roles

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
