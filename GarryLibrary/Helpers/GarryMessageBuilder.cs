using DSharpPlus.Entities;
using GarryLibrary.Models;

namespace GarryLibrary.Helpers;

public class GarryMessageBuilder
{
    private DiscordMessageBuilder _messageBuilder = new();
    private DiscordEmbedBuilder _embedBuilder = new();

    public DiscordInteractionResponseBuilder Build()
    {
        return new DiscordInteractionResponseBuilder(_messageBuilder.AddEmbed(_embedBuilder));
    }
    
    public DiscordMessageBuilder BuildMessage() => _messageBuilder.AddEmbed(_embedBuilder);

    public GarryMessageBuilder WithContent(string content)
    {
        _messageBuilder.WithContent(content);
        return this;
    }

    public GarryMessageBuilder WithTitle(string title)
    {
        _embedBuilder.WithTitle(title);
        return this;
    }

    public GarryMessageBuilder WithDescription(string description)
    {
        _embedBuilder.WithDescription(description);
        return this;
    }

    public GarryMessageBuilder WithThumbnail(string url)
    {
        _embedBuilder.WithThumbnail(url);
        return this;
    }

    public GarryMessageBuilder WithImage(string url)
    {
        _embedBuilder.WithImageUrl(url);
        return this;
    }

    public GarryMessageBuilder WithColor(DiscordColor color)
    {
        _embedBuilder.WithColor(color);
        return this;
    }

    public GarryMessageBuilder WithTimestamp()
    {
        _embedBuilder.WithTimestamp(DateTime.Now);
        return this;
    }

    public GarryMessageBuilder WithUserMention(DiscordUser user)
    {
        _messageBuilder.WithContent(user.Mention).AddMention(new UserMention(user));
        return this;
    }

    public GarryMessageBuilder WithRoleMention(DiscordRole role)
    {
        _messageBuilder.WithContent(role.Mention).AddMention(new RoleMention(role));
        return this;
    }

    public GarryMessageBuilder WithEveryoneMention()
    {
        _messageBuilder.WithContent("@everyone").AddMention(new EveryoneMention());
        return this;
    }

    public GarryMessageBuilder WithField(string name, string value, bool inline = false)
    {
        _embedBuilder.AddField(name, value, inline);
        return this;
    }

    public GarryMessageBuilder WithFields(Dictionary<string, string> fields, bool inline = false)
    {
        foreach (var field in fields)
        {
            _embedBuilder.AddField(field.Key, field.Value, inline);
        }

        return this;
    }
    
    public GarryMessageBuilder WithFooter(string footer, string iconUrl = "")
    {
        _embedBuilder.WithFooter(footer, iconUrl);
        return this;
    }
    
    public GarryMessageBuilder WithGuildBranding(DiscordGuild guild)
    {
        _embedBuilder.WithAuthor(guild.Name, "", guild.IconUrl);
        return this;
    }

    public GarryMessageBuilder WithUserBranding(ServerMember member)
    {
        _embedBuilder.WithAuthor(member.DisplayName, "", member.User.AvatarUrl);
        return this;
    }

    public GarryMessageBuilder WithPagination<T>(PageContext<T> pageContext)
    {
        _embedBuilder.WithDescription(pageContext.GetPageText())
            .WithFooter(pageContext.PageStatus);
        _messageBuilder.AddActionRowComponent(pageContext.GetPageButtons());
        return this;
    }

    public GarryMessageBuilder WithUserFocus(ServerMember member, DiscordGuild guild, string prefix = "")
    {
        WithGuildBranding(guild);
        if (string.IsNullOrWhiteSpace(prefix))
        {
            _embedBuilder.WithTitle($"{member.DisplayName}")
                .WithThumbnail(member.User.AvatarUrl);
        }
        else
        {
            _embedBuilder.WithTitle($"# {prefix} {member.Name ?? member.User.GlobalName}")
                .WithThumbnail(member.User.AvatarUrl);
        }

        return this;
    }

    public GarryMessageBuilder WithButtons(params DiscordButtonComponent[] buttons)
    {
        _messageBuilder.AddActionRowComponent(buttons);
        return this;
    }

    public GarryMessageBuilder WithTheme(EmbedTheme theme)
    {
        _embedBuilder.WithColor(theme switch
        {
            EmbedTheme.Success => DiscordColor.Green,
            EmbedTheme.Error => DiscordColor.Red,
            EmbedTheme.Warning => DiscordColor.Yellow,
            EmbedTheme.Info => DiscordColor.Blue,
            EmbedTheme.Birthday => DiscordColor.Magenta,
            EmbedTheme.Level => DiscordColor.Orange,
            EmbedTheme.Santa => DiscordColor.DarkRed,
            EmbedTheme.Wheel => DiscordColor.Cyan,
            EmbedTheme.Egg => DiscordColor.White,
            EmbedTheme.Heads => DiscordColor.Magenta,
            EmbedTheme.Tails => DiscordColor.Aquamarine,
            _ => DiscordColor.Gray
        });

        return this;
    }
}

public enum EmbedTheme
{
    Success,
    Error,
    Warning,
    Info,
    Birthday,
    Level,
    Santa,
    Wheel,
    Egg,
    Heads,
    Tails
}