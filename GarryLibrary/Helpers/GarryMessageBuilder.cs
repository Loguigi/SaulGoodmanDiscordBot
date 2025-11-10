using DSharpPlus.Entities;
using GarryLibrary.Models;

namespace GarryLibrary.Helpers;

public class GarryMessageBuilder
{
    private DiscordMessageBuilder _builder = new();
    private DiscordEmbedBuilder _embed = new();

    public DiscordMessageBuilder Build() => _builder.AddEmbed(_embed);
    public DiscordEmbedBuilder ToEmbed() => _embed;

    public GarryMessageBuilder WithContent(string content)
    {
        _builder.WithContent(content);
        return this;
    }

    public GarryMessageBuilder WithTitle(string title)
    {
        _embed.WithTitle(title);
        return this;
    }

    public GarryMessageBuilder WithDescription(string description)
    {
        _embed.WithDescription(description);
        return this;
    }

    public GarryMessageBuilder WithThumbnail(string url)
    {
        _embed.WithThumbnail(url);
        return this;
    }

    public GarryMessageBuilder WithColor(DiscordColor color)
    {
        _embed.WithColor(color);
        return this;
    }

    public GarryMessageBuilder WithTimestamp()
    {
        _embed.WithTimestamp(DateTime.Now);
        return this;
    }

    public GarryMessageBuilder WithUserMention(DiscordUser user)
    {
        _builder.WithContent(user.Mention).AddMention(new UserMention(user));
        return this;
    }

    public GarryMessageBuilder WithRoleMention(DiscordRole role)
    {
        _builder.WithContent(role.Mention).AddMention(new RoleMention(role));
        return this;
    }

    public GarryMessageBuilder WithEveryoneMention()
    {
        _builder.WithContent("@everyone").AddMention(new EveryoneMention());
        return this;
    }

    public GarryMessageBuilder WithField(string name, string value, bool inline = false)
    {
        _embed.AddField(name, value, inline);
        return this;
    }

    public GarryMessageBuilder WithFields(Dictionary<string, string> fields, bool inline = false)
    {
        foreach (var field in fields)
        {
            _embed.AddField(field.Key, field.Value, inline);
        }

        return this;
    }
    
    public GarryMessageBuilder WithFooter(string footer)
    {
        _embed.WithFooter(footer);
        return this;
    }
    
    public GarryMessageBuilder WithGuildBranding(DiscordGuild guild)
    {
        _embed.WithAuthor(guild.Name, "", guild.IconUrl);
        return this;
    }

    public GarryMessageBuilder WithUserBranding(ServerMember member)
    {
        _embed.WithAuthor(member.DisplayName, "", member.User.AvatarUrl);
        return this;
    }

    public GarryMessageBuilder WithPagination(PageContext<ServerMember> pageContext)
    {
        _embed.WithDescription(pageContext.GetPageText())
            .WithFooter(pageContext.PageStatus);
        _builder.AddComponents(pageContext.GetPageButtons());
        return this;
    }

    public GarryMessageBuilder WithUserFocus(ServerMember member, DiscordGuild guild, string prefix = "")
    {
        WithGuildBranding(guild);
        if (string.IsNullOrWhiteSpace(prefix))
        {
            _embed.WithTitle($"{member.DisplayName}")
                .WithThumbnail(member.User.AvatarUrl);
        }
        else
        {
            _embed.WithTitle($"# {prefix} {member.Name ?? member.User.GlobalName}")
                .WithThumbnail(member.User.AvatarUrl);
        }

        return this;
    }

    public GarryMessageBuilder WithButtons(params DiscordComponent[] buttons)
    {
        _builder.AddComponents(buttons);
        return this;
    }

    public GarryMessageBuilder WithTheme(EmbedTheme theme)
    {
        _embed.WithColor(theme switch
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