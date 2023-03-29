using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SaiyanBot.SlashCommands
{
    public class MusicCommand: ApplicationCommandModule
    {
        [SlashCommand("join", "Join a voice channel")]
        public async Task Join(InteractionContext ctx, [Option("channel", "語音頻道", true)] DiscordChannel channel)
        {
            // 檢查是否連結LavaLink
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("Lavalink is not connected."));
                return;
            }

            // 檢查是否有指定頻道，且為語音頻道
            var node = lava.ConnectedNodes.Values.First();
            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("You must specify a voice channel."));
                return;
            }


            // 檢查Bot是否有該頻道的權限
            var botMember = await channel.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id); // 取得機器人ID
            var permissions = botMember.PermissionsIn(channel);
            if (permissions == null || !permissions.HasPermission(Permissions.UseVoice | Permissions.Speak))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("I don't have permissions to join or speak in that voice channel."));
                return;
            }

            // 加入通道，返回訊息
            await node.ConnectAsync(channel);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent($"Joined {channel.Name}!"));

        }

        [SlashCommand("leave", "Leave a voice channel")]
        public async Task Leave(InteractionContext ctx, [Option("channel", "語音頻道", true)] DiscordChannel channel)
        {
            // 檢查是否連結LavaLink
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("The Lavalink connection is not established"));
                return;
            }

            // 檢查是否有指定頻道，且為語音頻道
            var node = lava.ConnectedNodes.Values.First();
            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("You must specify a voice channel."));
                return;
            }

            // 檢查是否連結頻道
            var conn = node.GetGuildConnection(channel.Guild);
            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("Lavalink is not connected."));
                return;
            }

            // 離開頻道，返回訊息
            await conn.DisconnectAsync();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent($"Left {channel.Name}!"));
        }

        /// <summary>
        ///  播放音樂
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [SlashCommand("play", "Play a song")]
        public async Task Play(InteractionContext ctx, [Option("url", "youtube url", true)] string search)
        {
            // 檢查用戶是否在語音頻道
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("You are not in a voice channel."));
                return;
            }

            // 檢查是否已經連結用戶的語音頻道
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("Lavalink is not connected."));
                return;
            }

            // 搜尋URI
            var loadResult = await node.Rest.GetTracksAsync(search);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent($"Track search failed for {search}."));
                return;
            }

            // 撥放音頻
            var track = loadResult.Tracks.First();
            await conn.PlayAsync(track);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent($"Now playing {track.Title}!"));

        }

        [SlashCommand("pause","Pause voice")]
        public async Task Pause(InteractionContext ctx)
        {
            // 檢查用戶是否在語音頻道
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("You are not in a voice channel."));
                return;
            }

            // 檢查是否已經連結用戶的語音頻道
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("Lavalink is not connected."));
                return;
            }

            // 檢查是否正在撥放音樂
            var track = conn.CurrentState.CurrentTrack;
            if (track == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent("There are no tracks loaded."));
                return;
            }

            await conn.PauseAsync();

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent($"Now pause {track.Title}!"));
        }

    }
}