using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SaiyanBot.SlashCommands
{
    public class TestCommand : ApplicationCommandModule
    {
        [SlashCommand("test", "This is a test command")]
        public async Task TestCommandAsync(InteractionContext ctx)
        {
            ulong guildId = ctx.Guild.Id;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                          .WithContent($"Your Guild ID: {guildId}"));
        }
    }
}
