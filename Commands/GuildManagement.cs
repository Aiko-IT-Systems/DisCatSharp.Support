using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Helpers;
using DisCatSharp.Support.Providers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DisCatSharp.Support.Commands
{
    /// <summary>
    /// The guild management.
    /// </summary>
    [SlashCommandGroup("guild_management", "Guild managemend module", false)]
    internal class GuildManagement : ApplicationCommandsModule
    {
        /// <summary>
        /// Voids the guild.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="guild_id">The target guild_id.</param>
        [SlashCommand("void_guild", "Voides a guild")]
        public static async Task VoidGuildAsync(InteractionContext ctx, [Autocomplete(typeof(GuildProvider)), Option("guild_id", "Target guild id", true)] string guild_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Guild void starting.."));

            var target_guild = await ctx.Client.GetGuildAsync(Convert.ToUInt64(guild_id));
            //var source_channels = await ChannelHelper.GetOrderedChannelAsync(ctx.Guild);

            try
            {
                #region Void target
                // Void target

                await target_guild.ModifyAsync(g =>
                {
                    g.Name = "Void";
                    g.Icon = null;
                    g.Description = null;
                    g.AuditLogReason = "Clean up";
                    //g.RulesChannel = null;
                    //g.PublicUpdatesChannel = null;
                    //g.AfkChannel = null;
                    if (target_guild.PremiumTier == PremiumTier.Tier_2)
                    {
                        g.Splash = null;
                    }

                    if (target_guild.PremiumTier == PremiumTier.Tier_2)
                    {
                        g.Banner = null;
                    }
                });

                await target_guild.DeleteAllChannelsAsync();

                foreach (DiscordRole role in target_guild.Roles.Values.Where(r => r.IsManaged == false))
                {
                    await role.DeleteAsync("Clean up");
                }

                foreach (DiscordGuildEmoji emoji in target_guild.Emojis.Values.Where(r => r.IsManaged == false))
                {
                    await emoji.DeleteAsync("Clean up");
                }

                foreach (DiscordSticker sticker in target_guild.Stickers.Values)
                {
                    await sticker.DeleteAsync("Clean up");
                }
                #endregion
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Target voided"));
            }
            catch (Exception ex)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Error",
                    Description = $"Exception: {ex.Message}\n" +
                    $"```\n" +
                    $"{ex.StackTrace}\n" +
                    $"```"
                }));
            }
        }

        /// <summary>
        /// Copies a guild.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="guild_id">The target guild_id.</param>
        [SlashCommand("void_guild", "Copies a guild")]
        public static async Task CopyGuildAsync(InteractionContext ctx, [Autocomplete(typeof(GuildProvider)), Option("guild_id", "Target guild id", true)] string guild_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Guild copy starting.."));

            var target_guild = await ctx.Client.GetGuildAsync(Convert.ToUInt64(guild_id));
            var source_channels = await ChannelHelper.GetOrderedChannelAsync(ctx.Guild);
            var source_roles = ctx.Guild.Roles.Values;
            var source_everyone = ctx.Guild.EveryoneRole;

            foreach(DiscordRole role in source_roles.Where(r => r.IsManaged == false).OrderBy(r => r.Position))
            {
                await target_guild.CreateRoleAsync(role.Name, role.Permissions, role.Color, role.IsHoisted, role.IsMentionable, "Restore");
            }

            await target_guild.EveryoneRole.ModifyAsync(permissions: source_everyone.Permissions, reason: "Restore");

            try
            {
                #region Restore target
                // Restore target

                foreach (var category in source_channels)
                {
                    if (category.Key != 0)
                    {
                        var s_cat = ctx.Guild.GetChannel(category.Key);
                        List<DiscordOverwriteBuilder> ovr = new();
                        foreach(var ov in s_cat.PermissionOverwrites)
                        {
                            if(ov.Type == OverwriteType.Member)
                            {
                                /*var tmem = await ov.GetMemberAsync();
                                ovr.Add(new DiscordOverwriteBuilder(tmem).Allow(ov.Allowed));
                                ovr.Add(new DiscordOverwriteBuilder(tmem).Deny(ov.Denied));*/
                            } else
                            {
                                var srole = await ov.GetRoleAsync();
                                var trole = target_guild.Roles.Values.Where(r => r.Name == srole.Name).First();
                                ovr.Add(new DiscordOverwriteBuilder(trole).Allow(ov.Allowed));
                                ovr.Add(new DiscordOverwriteBuilder(trole).Deny(ov.Denied));
                            }
                        }
                        await target_guild.CreateChannelCategoryAsync(s_cat.Name, ovr.AsEnumerable(), "Restore");
                    }

                    foreach (var channel in category.Value)
                    {
                        List<DiscordOverwriteBuilder> ovr = new();
                        foreach (var ov in channel.PermissionOverwrites)
                        {
                            if (ov.Type == OverwriteType.Member)
                            {
                                /*var tmem = await ov.GetMemberAsync();
                                ovr.Add(new DiscordOverwriteBuilder(tmem).Allow(ov.Allowed));
                                ovr.Add(new DiscordOverwriteBuilder(tmem).Deny(ov.Denied));*/
                            }
                            else
                            {
                                var srole = await ov.GetRoleAsync();
                                var trole = target_guild.Roles.Values.Where(r => r.Name == srole.Name).First();
                                ovr.Add(new DiscordOverwriteBuilder(trole).Allow(ov.Allowed));
                                ovr.Add(new DiscordOverwriteBuilder(trole).Deny(ov.Denied));
                            }
                        }
                        var rchan = channel.Type switch
                        {
                            ChannelType.Voice => await target_guild.CreateVoiceChannelAsync(channel.Name, channel.Parent, channel.Bitrate, channel.UserLimit, ovr.AsEnumerable(), channel.QualityMode, "Restore"),
                            ChannelType.Stage => await target_guild.CreateChannelAsync(channel.Name, ChannelType.Stage, channel.Parent, overwrites: ovr.AsEnumerable(), reason: "Restore"),
                            ChannelType.News => await target_guild.CreateChannelAsync(channel.Name, channel.Type, channel.Parent, channel.Topic, channel.Bitrate, channel.UserLimit, ovr.AsEnumerable(), channel.IsNSFW, channel.PerUserRateLimit, channel.QualityMode, "Restore"),
                            ChannelType.Text => await target_guild.CreateChannelAsync(channel.Name, channel.Type, channel.Parent, channel.Topic, channel.Bitrate, channel.UserLimit, ovr.AsEnumerable(), channel.IsNSFW, channel.PerUserRateLimit, channel.QualityMode, "Restore"),
                            _=> null
                        };

                    }

                    var channels = await target_guild.GetChannelsAsync();

                    await target_guild.ModifyAsync(g =>
                    {
                        g.Name = ctx.Guild.Name;
                        WebClient wc = new();
                        wc.DownloadFileAsync(new Uri(ctx.Guild.IconUrl), ctx.Guild.IconHash);
                        var fs = File.OpenRead(ctx.Guild.IconHash);
                        fs.Position = 0;
                        MemoryStream ms = new();
                        fs.CopyTo(ms);
                        fs.Close();
                        ms.Position = 0;
                        g.Icon = ms;
                        g.Description = ctx.Guild.Description;
                        g.AuditLogReason = "Restore";
                        g.RulesChannel = channels.Where(x => x.Name == ctx.Guild.RulesChannel.Name).First();
                        g.PublicUpdatesChannel = channels.Where(x => x.Name == ctx.Guild.PublicUpdatesChannel.Name).First();
                        g.AfkChannel = channels.Where(x => x.Name == ctx.Guild.AfkChannel.Name).First(); ;
                        if (target_guild.PremiumTier == PremiumTier.Tier_2)
                        {
                            wc.DownloadFileAsync(new Uri(ctx.Guild.SplashUrl), ctx.Guild.SplashHash);
                            var sfs = File.OpenRead(ctx.Guild.SplashHash);
                            sfs.Position = 0;
                            MemoryStream sms = new();
                            sfs.CopyTo(sms);
                            sfs.Close();
                            sms.Position = 0;
                            g.Splash = sms;
                        }

                        if (target_guild.PremiumTier == PremiumTier.Tier_2)
                        {
                            wc.DownloadFileAsync(new Uri(ctx.Guild.BannerUrl), ctx.Guild.BannerHash);
                            var bfs = File.OpenRead(ctx.Guild.BannerHash);
                            bfs.Position = 0;
                            MemoryStream bms = new();
                            bfs.CopyTo(bms);
                            bfs.Close();
                            bms.Position = 0;
                            g.Banner = bms;
                        }
                    });
                }

                #endregion
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Target restored"));
            }
            catch (Exception ex)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "Error",
                    Description = $"Exception: {ex.Message}\n" +
                    $"```\n" +
                    $"{ex.StackTrace}\n" +
                    $"```"
                }));
            }
        }
    }
}
