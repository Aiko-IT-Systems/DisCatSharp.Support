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
                var cur = await target_guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
                foreach (DiscordRole role in target_guild.Roles.Values.Where(r => r.IsManaged == false))
                {
                    if (cur.Roles.Contains(role))
                    {
                        // Nothing
                    } else
                    {
                        await role.DeleteAsync("Clean up");
                    }
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
        [SlashCommand("copy_guild", "Copies a guild")]
        public static async Task CopyGuildAsync(InteractionContext ctx, [Autocomplete(typeof(GuildProvider)), Option("guild_id", "Target guild id", true)] string guild_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Guild copy starting.."));

            var target_guild = await ctx.Client.GetGuildAsync(Convert.ToUInt64(guild_id));
            var source_channels = await ChannelHelper.GetOrderedChannelAsync(ctx.Guild);
            var source_roles = ctx.Guild.Roles.Values;
            var source_everyone = ctx.Guild.EveryoneRole;

            try
            {
                #region Restore target
                // Restore target

                foreach (DiscordRole role in source_roles.Where(r => r.IsManaged == false).OrderByDescending(r => r.Position))
                {
                    if (role.Name != "@everyone")
                        await target_guild.CreateRoleAsync(role.Name, role.Permissions, role.Color, role.IsHoisted, role.IsMentionable, "Restore");
                }

                await target_guild.EveryoneRole.ModifyAsync(permissions: source_everyone.Permissions, reason: "Restore");

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
                            ChannelType.Voice => await target_guild.CreateVoiceChannelAsync(channel.Name, channel.Parent ?? null, channel.Bitrate ?? null, channel.UserLimit ?? null, ovr.AsEnumerable(), channel.QualityMode ?? null, "Restore"),
                            ChannelType.Stage => await target_guild.CreateStageChannelAsync(channel.Name, ovr.AsEnumerable(), reason: "Restore"),
                            ChannelType.News => await target_guild.CreateChannelAsync(channel.Name, channel.Type, channel.Parent ?? null, channel.Topic, null, null, ovr.AsEnumerable(), channel.IsNSFW, channel.PerUserRateLimit ?? null, null, "Restore"),
                            ChannelType.Text => await target_guild.CreateChannelAsync(channel.Name, channel.Type, channel.Parent ?? null, channel.Topic, null, null, ovr.AsEnumerable(), channel.IsNSFW, channel.PerUserRateLimit ?? null, null, "Restore"),
                            _=> null
                        };
                        rchan = null;

                    }
                }

                var new_channels = await target_guild.GetChannelsAsync();

                WebClient wc = new();

                var t_rules_old = target_guild.RulesChannel;
                var t_public_old = target_guild.PublicUpdatesChannel;

                await target_guild.ModifyAsync(g =>
                {
                    g.Name = ctx.Guild.Name;
                    wc.DownloadFile(new Uri(ctx.Guild.IconUrl), ctx.Guild.IconHash);
                    var fs = File.OpenRead(ctx.Guild.IconHash);
                    fs.Position = 0;
                    MemoryStream ms = new();
                    fs.CopyTo(ms);
                    fs.Close();
                    ms.Position = 0;
                    g.Icon = ms;
                    g.Description = ctx.Guild.Description;
                    g.AuditLogReason = "Restore";
                    g.RulesChannel = new_channels.Where(x => x.Name == ctx.Guild.RulesChannel.Name).First();
                    g.PublicUpdatesChannel = new_channels.Where(x => x.Name == ctx.Guild.PublicUpdatesChannel.Name).First();
                    g.AfkChannel = new_channels.Where(x => x.Name == ctx.Guild.AfkChannel.Name).First();
                    g.AfkTimeout = ctx.Guild.AfkTimeout;
                    g.SystemChannel = new_channels.Where(x => x.Name == ctx.Guild.SystemChannel.Name).First();
                    g.SystemChannelFlags = ctx.Guild.SystemChannelFlags;
                    g.DefaultMessageNotifications = ctx.Guild.DefaultMessageNotifications;
                    if (target_guild.PremiumTier == PremiumTier.Tier_2)
                    {
                        wc.DownloadFile(new Uri(ctx.Guild.SplashUrl), ctx.Guild.SplashHash);
                        var sfs = File.OpenRead(ctx.Guild.SplashHash);
                        sfs.Position = 0;
                        MemoryStream sms = new();
                        sfs.CopyTo(sms);
                        sfs.Close();
                        File.Delete(ctx.Guild.SplashHash);
                        sms.Position = 0;
                        g.Splash = sms;
                        sms.Close();
                    }

                    if (target_guild.PremiumTier == PremiumTier.Tier_2)
                    {
                        wc.DownloadFile(new Uri(ctx.Guild.BannerUrl), ctx.Guild.BannerHash);
                        var bfs = File.OpenRead(ctx.Guild.BannerHash);
                        bfs.Position = 0;
                        MemoryStream bms = new();
                        bfs.CopyTo(bms);
                        bfs.Close();
                        File.Delete(ctx.Guild.BannerHash);
                        bms.Position = 0;
                        g.Banner = bms;
                        bms.Close();
                    }

                    if (target_guild.Features.IsDiscoverable)
                    {
                        wc.DownloadFile(new Uri(ctx.Guild.DiscoverySplashUrl), ctx.Guild.DiscoverySplashHash);
                        var dfs = File.OpenRead(ctx.Guild.DiscoverySplashHash);
                        dfs.Position = 0;
                        MemoryStream dms = new();
                        dfs.CopyTo(dms);
                        dfs.Close();
                        File.Delete(ctx.Guild.DiscoverySplashHash);
                        dms.Position = 0;
                        g.DiscoverySplash = dms;
                        dms.Close();
                    }


                    File.Delete(ctx.Guild.IconHash);
                    ms.Close();
                });

                await t_public_old.DeleteAsync("Cleanup");
                await t_rules_old.DeleteAsync("Cleanup");

                int max_emoji = target_guild.PremiumTier switch {
                    PremiumTier.None => 50,
                    PremiumTier.Tier_1 => 100,
                    PremiumTier.Tier_2 => 150,
                    PremiumTier.Tier_3 => 250,
                    _ =>  50
                };

                int max_sticker = target_guild.PremiumTier switch
                {
                    PremiumTier.None => 0,
                    PremiumTier.Tier_1 => 15,
                    PremiumTier.Tier_2 => 30,
                    PremiumTier.Tier_3 => 60,
                    _ => 0
                };

                int ei = 0;
                foreach (DiscordGuildEmoji emoji in ctx.Guild.Emojis.Values)
                {

                    if (ei < max_emoji)
                    {
                        await CopyEmojiAsync(emoji, target_guild);
                    }
                    // Ignore
                }

                int si = 0;
                foreach (DiscordSticker sticker in ctx.Guild.Stickers.Values)
                {
                    if (si < max_sticker)
                    {
                        await CopyStickerAsync(sticker, target_guild);
                    }
                    // Ignore
                }

                var welcome_screen = ctx.Guild.HasWelcomeScreen ? await ctx.Guild.GetWelcomeScreenAsync() : null;
                var membership_gate = ctx.Guild.HasMemberVerificationGate ? await ctx.Guild.GetMembershipScreeningFormAsync() : null;

                if (welcome_screen != null)
                {
                    var t_welcome_screen = await target_guild.ModifyWelcomeScreenAsync(g =>
                    {
                        g.Description = welcome_screen.Description;
                        g.Enabled = ctx.Guild.HasWelcomeScreen;
                        List<DiscordGuildWelcomeScreenChannel> wscs = new();
                        foreach(var wsc in welcome_screen.WelcomeChannels)
                        {
                            wscs.Add(new DiscordGuildWelcomeScreenChannel(new_channels.Where(c => c.Name == ctx.Guild.GetChannel(wsc.ChannelId).Name).First().Id, wsc.Description, DiscordEmoji.FromUnicode(Bot.DiscordClient, wsc.EmojiName)));
                        }
                        g.WelcomeChannels = Optional.FromValue(wscs.AsEnumerable());
                    });
                }

                if (membership_gate != null)
                {
                    var t_membership_gate = await target_guild.ModifyMembershipScreeningFormAsync(g =>
                    {
                        g.Description = membership_gate.Description;
                        g.Enabled = ctx.Guild.HasMemberVerificationGate;
                        g.AuditLogReason = "Restore";
                        var fields = new DiscordGuildMembershipScreeningField[membership_gate.Fields.Count];
                        int i = 1;
                        foreach(var field in membership_gate.Fields)
                        {
                            fields[i] = new DiscordGuildMembershipScreeningField(field.Type, field.Label, field.Values.AsEnumerable(), field.IsRequired);
                            i++;
                        }

                        g.Fields = fields;
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

        /// <summary>
        /// Copies the emoji.
        /// </summary>
        /// <param name="emoji">The source emoji.</param>
        /// <param name="target">The target guild.</param>
        private static async Task CopyEmojiAsync(DiscordGuildEmoji emoji, DiscordGuild target)
        {
            WebClient wc = new();
            wc.DownloadFile(new Uri(emoji.Url), emoji.Id.ToString());
            var fs = File.OpenRead(emoji.Id.ToString());
            fs.Position = 0;
            MemoryStream ms = new();
            fs.CopyTo(ms);
            fs.Close();
            ms.Position = 0;
            await target.CreateEmojiAsync(emoji.Name, ms, reason: "Restore");
            File.Delete(emoji.Id.ToString());
            ms.Close();
        }

        /// <summary>
        /// Copies the sticker.
        /// </summary>
        /// <param name="sticker">The source sticker.</param>
        /// <param name="target">The target guild.</param>
        private static async Task CopyStickerAsync(DiscordSticker sticker, DiscordGuild target)
        {
            WebClient wc = new();
            wc.DownloadFile(new Uri(sticker.Url), sticker.Asset);
            var fs = File.OpenRead(sticker.Asset);
            fs.Position = 0;
            MemoryStream ms = new();
            fs.CopyTo(ms);
            fs.Close();
            ms.Position = 0;
            await target.CreateStickerAsync(sticker.Name, sticker.Description, DiscordEmoji.FromName(Bot.DiscordClient, sticker.Tags.First()), ms, sticker.FormatType,"Restore");
            File.Delete(sticker.Asset);
            ms.Close();
        }
    }
}
