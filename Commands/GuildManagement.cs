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
    internal class GuildManagement// : ApplicationCommandsModule
    {
        /// <summary>
        /// Copies a guild.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="guild_id">The target guild_id.</param>
        [SlashCommand("void_copy_guild", "Voides a target guild and copies the source guild then.")]
        public static async Task VoidAndCopyGuildAsync(InteractionContext ctx, [Autocomplete(typeof(GuildProvider)), Option("guild_id", "Target guild id", true)] string guild_id)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Guild void & restore starting.."));
            await VoidGuildAsync(ctx, guild_id);
            await CopyGuildAsync(ctx, guild_id);
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Finished."));
        }

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
            var target_channels = await target_guild.GetChannelsAsync();

            try
            {
                #region Void target
                // Void target
                var tmpl = await target_guild.GetTemplatesAsync();
                if (tmpl != null && tmpl.Count != 0)
                    await target_guild.DeleteTemplateAsync(tmpl[0].Code);

                await target_guild.ModifyAsync(g =>
                {
                    g.Name = "Void";
                    g.Icon = null;
                    g.Description = null;
                    g.AuditLogReason = "Clean up";
                    if (target_guild.PremiumTier == PremiumTier.Tier_1 || target_guild.PremiumTier == PremiumTier.Tier_2 || target_guild.PremiumTier == PremiumTier.Tier_3)
                    {
                        g.Splash = null;
                    }

                    if (target_guild.PremiumTier == PremiumTier.Tier_2 || target_guild.PremiumTier == PremiumTier.Tier_3)
                    {
                        g.Banner = null;
                    }
                });

                foreach(var channel in target_channels.Where(c => c.Id != target_guild.RulesChannel.Id && c.Id != target_guild.PublicUpdatesChannel.Id))
                {
                    await channel.DeleteAsync("Clean up");
                }

                foreach (DiscordRole role in target_guild.Roles.Values.Where(r => r.IsManaged == false))
                {
                    if (role.Name == "Server Booster" || role.Name == "@everyone")
                    {
                        await Task.Delay(100);
                    } else
                    {
                        await role.DeleteAsync("Clean up");
                    }
                }

                foreach (DiscordEmoji emoji in target_guild.Emojis.Values.Where(r => r.IsManaged == false))
                {
                    var gemoji = await target_guild.GetEmojiAsync(emoji.Id);
                    await gemoji.DeleteAsync("Clean up");
                }

                #endregion
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Target voided"));
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
                }.Build());
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
                                // Ignore
                            }
                            else
                            {
                                var srole = await ov.GetRoleAsync();
                                if (!srole.IsManaged || srole.Name == "@everyone")
                                {
                                    var trole = target_guild.Roles.Values.Where(r => r.Name == srole.Name).First();
                                    ovr.Add(new DiscordOverwriteBuilder(trole).Allow(ov.Allowed));
                                    ovr.Add(new DiscordOverwriteBuilder(trole).Deny(ov.Denied));
                                }
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
                                // Ignore
                            }
                            else
                            {
                                var srole = await ov.GetRoleAsync();
                                if (!srole.IsManaged ||srole.Name == "@everyone")
                                {
                                    var trole = target_guild.Roles.Values.Where(r => r.Name == srole.Name).First();
                                    ovr.Add(new DiscordOverwriteBuilder(trole).Allow(ov.Allowed));
                                    ovr.Add(new DiscordOverwriteBuilder(trole).Deny(ov.Denied));
                                }
                            }
                        }
                        var rchan = channel.Type switch
                        {
                            ChannelType.Voice => await target_guild.CreateVoiceChannelAsync(channel.Name, channel.Parent == null ? null : target_guild.Channels.Values.Where(c => c.Name == channel.Parent.Name).First(), channel.Bitrate ?? null, channel.UserLimit ?? null, ovr.AsEnumerable(), channel.QualityMode ?? null, "Restore"),
                            ChannelType.Stage => await target_guild.CreateChannelAsync(channel.Name, ChannelType.Stage, channel.Parent == null ? null : target_guild.Channels.Values.Where(c => c.Name == channel.Parent.Name).First(), null, null, null, ovr.AsEnumerable(), null, null, null, "Restore"),
                            ChannelType.News => await target_guild.CreateChannelAsync(channel.Name, channel.Type, channel.Parent == null ? null : target_guild.Channels.Values.Where(c => c.Name == channel.Parent.Name).First(), channel.Topic, null, null, ovr.AsEnumerable(), channel.IsNSFW, channel.PerUserRateLimit ?? null, null, "Restore"),
                            ChannelType.Text => await target_guild.CreateChannelAsync(channel.Name, channel.Type, channel.Parent == null ? null : target_guild.Channels.Values.Where(c => c.Name == channel.Parent.Name).First(), channel.Topic, null, null, ovr.AsEnumerable(), channel.IsNSFW, channel.PerUserRateLimit ?? null, null, "Restore"),
                            _=> null
                        };
                        rchan = null;

                    }
                }


                WebClient wc = new();
                
                var t_rules_old = target_guild.RulesChannel;
                await t_rules_old.ModifyAsync(c => c.Name = "old-rules");
                var t_public_old = target_guild.PublicUpdatesChannel;
                await t_public_old.ModifyAsync(c => c.Name = "old-updates");

                target_guild = await ctx.Client.GetGuildAsync(target_guild.Id);
                var new_channels = await target_guild.GetChannelsAsync();

                await target_guild.ModifyAsync(g => 
                {
                    g.Name = ctx.Guild.Name + " Copy";
                    g.Description = ctx.Guild.Description;
                    g.AuditLogReason = "Restore";
                    g.RulesChannel = new_channels.Where(x => x.Name == ctx.Guild.RulesChannel.Name).First();
                    g.PublicUpdatesChannel = new_channels.Where(x => x.Name == ctx.Guild.PublicUpdatesChannel.Name).First();
                    g.AfkChannel = new_channels.Where(x => x.Name == ctx.Guild.AfkChannel.Name).First();
                    g.AfkTimeout = ctx.Guild.AfkTimeout;
                    g.SystemChannel = new_channels.Where(x => x.Name == ctx.Guild.SystemChannel.Name).First();
                    g.SystemChannelFlags = ctx.Guild.SystemChannelFlags;
                    g.DefaultMessageNotifications = ctx.Guild.DefaultMessageNotifications;
                    //g.PreferredLocale = ctx.Guild.PreferredLocale;
                });
                target_guild = await ctx.Client.GetGuildAsync(target_guild.Id);
                await target_guild.ModifyWidgetSettingsAsync(ctx.Guild.WidgetEnabled, ctx.Guild.WidgetChannel ?? null, "Restore");
                
                wc.DownloadFile(new Uri(ctx.Guild.IconUrl), ctx.Guild.IconHash + (ctx.Guild.PremiumTier != PremiumTier.None && target_guild.PremiumTier != PremiumTier.None ? ".gif" : ".png"));
                var fs = File.OpenRead(ctx.Guild.IconHash + (ctx.Guild.PremiumTier != PremiumTier.None && target_guild.PremiumTier != PremiumTier.None ? ".gif" : ".png"));
                fs.Position = 0;
                MemoryStream ms = new();
                await fs.CopyToAsync(ms);
                ms.Position = 0;
                await target_guild.ModifyAsync(g => g.Icon = ms);
                fs.Close();
                await fs.DisposeAsync();
                File.Delete(ctx.Guild.IconHash + (ctx.Guild.PremiumTier != PremiumTier.None && target_guild.PremiumTier != PremiumTier.None ? ".gif" : ".png"));

                if (target_guild.PremiumTier == PremiumTier.Tier_1 || target_guild.PremiumTier == PremiumTier.Tier_2 || target_guild.PremiumTier == PremiumTier.Tier_3)
                {
                    wc.DownloadFile(new Uri(ctx.Guild.SplashUrl), ctx.Guild.SplashHash + ".png");
                    var sfs = File.OpenRead(ctx.Guild.SplashHash + ".png");
                    sfs.Position = 0;
                    MemoryStream sms = new();
                    await sfs.CopyToAsync(sms);
                    sms.Position = 0;
                    await target_guild.ModifyAsync(g => g.Splash = sms);
                    sfs.Close();
                    await sfs.DisposeAsync();
                    File.Delete(ctx.Guild.SplashHash + ".png");
                }

                if (target_guild.PremiumTier == PremiumTier.Tier_2 || target_guild.PremiumTier == PremiumTier.Tier_3)
                {
                    wc.DownloadFile(new Uri(ctx.Guild.BannerUrl), ctx.Guild.BannerHash + ".png");
                    var bfs = File.OpenRead(ctx.Guild.BannerHash + ".png");
                    bfs.Position = 0;
                    MemoryStream bms = new();
                    await bfs.CopyToAsync(bms);
                    bms.Position = 0;
                    await target_guild.ModifyAsync(g => g.Banner = bms);
                    bfs.Close();
                    await bfs.DisposeAsync();
                    File.Delete(ctx.Guild.BannerHash + ".png");
                }

                if (target_guild.Features.IsDiscoverable)
                {
                    wc.DownloadFile(new Uri(ctx.Guild.DiscoverySplashUrl), ctx.Guild.DiscoverySplashHash + ".png");
                    var dfs = File.OpenRead(ctx.Guild.DiscoverySplashHash + ".png");
                    dfs.Position = 0;
                    MemoryStream dms = new();
                    await dfs.CopyToAsync(dms);
                    dms.Position = 0;
                    await target_guild.ModifyAsync(g => g.DiscoverySplash = dms);
                    dfs.Close();
                    await dfs.DisposeAsync();
                    File.Delete(ctx.Guild.DiscoverySplashHash + ".png");
                }

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
                foreach (DiscordEmoji emoji in ctx.Guild.Emojis.Values.Where(e => !e.IsAnimated))
                {

                    if (ei < max_emoji)
                    {
                        await CopyEmojiAsync(emoji, target_guild);
                    }
                    // Ignore
                }
                
                ei = 0;
                foreach (DiscordEmoji emoji in ctx.Guild.Emojis.Values.Where(e => e.IsAnimated))
                {

                    if (ei < max_emoji)
                    {
                        await CopyEmojiAsync(emoji, target_guild);
                    }
                    // Ignore
                }

                // TODO: Fix, does not work with sticker smh
                /*
                int si = 0;
                foreach (DiscordSticker sticker in ctx.Guild.Stickers.Values)
                {
                    if (si < max_sticker)
                    {
                        await CopyStickerAsync(sticker, target_guild);
                    }
                    // Ignore
                }
                */
                target_guild = await ctx.Client.GetGuildAsync(target_guild.Id);

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
                            var schannel = ctx.Guild.GetChannel(wsc.ChannelId);
                            wscs.Add(new DiscordGuildWelcomeScreenChannel(new_channels.Where(c => c.Name == schannel.Name).First().Id, wsc.Description, DiscordEmoji.FromUnicode(Bot.DiscordClient, wsc.EmojiName)));
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
                        int i = 0;
                        foreach(var field in membership_gate.Fields)
                        {
                            fields[i] = new DiscordGuildMembershipScreeningField(field.Type, field.Label, field.Values.AsEnumerable(), field.IsRequired);
                            i++;
                        }

                        g.Fields = fields;
                    });
                }

                var tmpl = await ctx.Guild.GetTemplatesAsync();
                DiscordGuildTemplate template = null;
                if(tmpl != null && tmpl.Count != 0)
                {
                    template = await target_guild.CreateTemplateAsync(tmpl[0].Name, tmpl[0].Description);
                } 
                else
                {
                    template = await target_guild.CreateTemplateAsync(ctx.Guild.Name, ctx.Guild.Description);
                }
                

                #endregion
                await ctx.Channel.SendMessageAsync($"Target restored\n\nTemplate: https://discord.new/{template.Code}");
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
                }.Build());
            }
        }

        /// <summary>
        /// Copies the emoji.
        /// </summary>
        /// <param name="emoji">The source emoji.</param>
        /// <param name="target">The target guild.</param>
        private static async Task CopyEmojiAsync(DiscordEmoji emoji, DiscordGuild target)
        {
            WebClient wc = new(); 
            wc.DownloadFile(new Uri(emoji.Url), emoji.Id.ToString() + (emoji.IsAnimated ? ".gif" : ".png"));
            var fs = File.OpenRead(emoji.Id.ToString() + (emoji.IsAnimated ? ".gif" : ".png"));
            fs.Position = 0;
            MemoryStream ms = new();
            fs.CopyTo(ms);
            ms.Position = 0;
            await target.CreateEmojiAsync(emoji.Name, ms, reason: "Restore");
            fs.Close();
            await fs.DisposeAsync();
            File.Delete(emoji.Id.ToString() + (emoji.IsAnimated ? ".gif" : ".png"));
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
            wc.DownloadFile(new Uri(sticker.Url), sticker.Asset + (sticker.FormatType == StickerFormat.LOTTIE ? ".json" : ".png"));
            var fs = File.OpenRead(sticker.Asset + (sticker.FormatType == StickerFormat.LOTTIE ? ".json" : ".png"));
            fs.Position = 0;
            MemoryStream ms = new();
            fs.CopyTo(ms);
            ms.Position = 0;
            await target.CreateStickerAsync(sticker.Name, sticker.Description, DiscordEmoji.FromName(Bot.DiscordClient, sticker.Tags.First()), ms, sticker.FormatType,"Restore");
            fs.Close();
            await fs.DisposeAsync();
            File.Delete(sticker.Asset + (sticker.FormatType == StickerFormat.LOTTIE ? ".json" : ".png"));
            ms.Close();
        }
    }
}
