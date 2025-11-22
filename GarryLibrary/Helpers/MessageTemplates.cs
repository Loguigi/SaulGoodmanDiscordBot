using DSharpPlus.Entities;
using GarryLibrary.Models;

namespace GarryLibrary.Helpers;

public static class MessageTemplates
{
    // Constants for image URLs
    private static class ImageUrls
    {
        public const string Success = "https://i.pinimg.com/736x/96/19/ef/9619ef6bc6d40637c670f9147f9d0515.jpg";
        public const string Error = "https://images.steamusercontent.com/ugc/595876698379580434/C8B1E0CACE0600024FA2D3B3C575174BDA462CEC/?imw=268&imh=268&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=true";
    }

    #region Status Messages

    public static DiscordInteractionResponseBuilder CreateSuccess(string message)
    {
        return new GarryMessageBuilder()
            .WithTitle("✅ Success")
            .WithDescription(message)
            .WithThumbnail(ImageUrls.Success)
            .WithTheme(EmbedTheme.Success)
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateError(string message)
    {
        return new GarryMessageBuilder()
            .WithTitle("❌ Error")
            .WithDescription(message)
            .WithThumbnail(ImageUrls.Error)
            .WithTheme(EmbedTheme.Error)
            .Build();
    }

    #endregion

    #region Level System

    public static DiscordInteractionResponseBuilder CreateLeaderboard(List<ServerMember> members, DiscordGuild guild, string page)
    {
        return new GarryMessageBuilder()
            .WithTitle("Leaderboard")
            .WithGuildBranding(guild)
            .WithTheme(EmbedTheme.Level)
            .WithPagination(new PageContext<ServerMember>(members, 10, page, IDHelper.Levels.LEADERBOARD))
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateLevelCard(ServerMember member, DiscordGuild guild)
    {
        var prefix = GetRankEmoji(member.Rank);
        var fields = new Dictionary<string, string>
        {
            { "Rank", $"#{member.Rank}" },
            { "Level", member.Level.ToString() },
            { "Experience", $"{member.Experience} / {member.ExpNeededForNextLevel} XP" },
        };

        return new GarryMessageBuilder()
            .WithUserFocus(member, guild, prefix)
            .WithTheme(EmbedTheme.Level)
            .WithFields(fields, true)
            .Build();
    }

    public static DiscordMessageBuilder CreateLevelUpNotification(ServerMember member, string message)
    {
        return new GarryMessageBuilder()
            .WithDescription($"### ⬆️ {member.User.Mention} {message}")
            .WithFooter($"Level {member.Level - 1} ➡️ Level {member.Level}")
            .WithTheme(EmbedTheme.Wheel)
            .BuildMessage();
    }

    #endregion
    
    #region Wheel System

    public static DiscordInteractionResponseBuilder CreateWheelSpin(ServerMember member, WheelPicker wheel, SpinData spinData)
    {
        var spinAgainData = spinData with { ShouldRemoveLastOption = false };
        var spinAgainButton = new DiscordButtonComponent(
            DiscordButtonStyle.Primary,
            spinAgainData.ToButtonId(),
            "Spin Again",
            false,
            new DiscordComponentEmoji("🎡")
        );

        var spinAndRemoveData = spinData with { ShouldRemoveLastOption = true };
        var spinAndRemoveButton = new DiscordButtonComponent(
            DiscordButtonStyle.Danger,
            spinAndRemoveData.ToButtonId(),
            "Remove & Spin",
            false,
            new DiscordComponentEmoji("❌")
        );

        var buttons = new[] { spinAgainButton, spinAndRemoveButton };
    
        // Format the previous spin display
        string previousSpinDisplay;
        if (spinData.PreviousOptionSpun == null)
        {
            previousSpinDisplay = "------";
        }
        else if (spinData.ShouldRemoveLastOption)
        {
            previousSpinDisplay = $"❌ ~~{spinData.PreviousOptionSpun}~~ ❌";
        }
        else
        {
            previousSpinDisplay = spinData.PreviousOptionSpun;
        }
    
        return new GarryMessageBuilder()
            .WithGuildBranding(member.Guild!)
            .WithTitle($"🌀 {wheel.Name} 🌀")
            .WithDescription($"# {spinData.LastOptionSpun}")
            .WithThumbnail(wheel.ImageUrl ?? "")
            .WithField("Spin Count", spinData.SpinCount.ToString(), true)
            .WithField("Total Options", $"{wheel.AvailableOptions.Count} / {wheel.WheelOptions.Count}", true)
            .WithField("Previous Spin", previousSpinDisplay, true)
            .WithFooter($"Spun by {member.DisplayName}", member.User.AvatarUrl)
            .WithTheme(EmbedTheme.Wheel)
            .WithButtons(buttons)
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateWheelOptionList(WheelPicker wheel, DiscordGuild guild, string page)
    {
        return new GarryMessageBuilder()
            .WithGuildBranding(guild)
            .WithTitle($"{wheel.Name} - {wheel.WheelOptions.Count} Options")
            .WithThumbnail(wheel.ImageUrl ?? "")
            .WithColor(DiscordColor.Gold)
            .WithPagination(new PageContext<WheelOption>(wheel.WheelOptions, 20, page, $"{IDHelper.WheelPicker.List}\\{wheel.Id}"))
            .Build();
    }
    #endregion

    #region Birthday System

    public static DiscordInteractionResponseBuilder CreateBirthdayList(DiscordGuild guild, IEnumerable<ServerMember> members, string page)
    {
        var sortedMembers = members.OrderBy(x => x.NextBirthday).ToList();
        
        return new GarryMessageBuilder()
            .WithTitle("Birthdays")
            .WithGuildBranding(guild)
            .WithTheme(EmbedTheme.Birthday)
            .WithPagination(new PageContext<ServerMember>(sortedMembers, 10, page, IDHelper.Birthdays.LIST))
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateBirthdayCard(ServerMember member)
    {
        return new GarryMessageBuilder()
            .WithTitle("Birthday")
            .WithDescription($"# {member.FormattedBirthday}")
            .WithUserBranding(member)
            .WithTheme(EmbedTheme.Birthday)
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateNextBirthday(ServerMember member, DiscordGuild guild)
    {
        var daysUntil = member.DaysUntilBirthday?.Days ?? 0;
        var daysText = daysUntil switch
        {
            0 => "🎉 Today!",
            1 => "Tomorrow",
            _ => $"In {daysUntil} days"
        };

        return new GarryMessageBuilder()
            .WithTitle("Next Birthday")
            .WithDescription($"# {member.DisplayMention}\n\n{member.FormattedNextBirthday}\n**{daysText}**")
            .WithGuildBranding(guild)
            .WithThumbnail(member.User.AvatarUrl)
            .WithTheme(EmbedTheme.Birthday)
            .Build();
    }

    public static DiscordMessageBuilder CreateBirthdayNotification(ServerMember member, string message, bool isBirthday = false)
    {
        var builder = new GarryMessageBuilder()
            .WithTheme(EmbedTheme.Birthday)
            .WithDescription($"# {message}")
            .WithFooter($"🎂 {(isBirthday ? DateTime.Now.ToString("D") : member.NextBirthday!.Value.ToString("D"))}");

        if (isBirthday) builder.WithEveryoneMention();
        
        return builder.BuildMessage();
    }

    #endregion

    #region Miscellaneous

    public static DiscordInteractionResponseBuilder CreateEggCounter(DiscordGuild guild, IEnumerable<ServerMember> members, string page)
    {
        var sortedMembers = members.OrderByDescending(x => x.EggCount).ToList();
        
        return new GarryMessageBuilder()
            .WithTitle("🥚 ROTTEN EGGS 🥚")
            .WithGuildBranding(guild)
            .WithTheme(EmbedTheme.Egg)
            .WithPagination(new PageContext<ServerMember>(sortedMembers, 10, page, IDHelper.Misc.EGG))
            .Build();
    }

    public static DiscordMessageBuilder CreateEggNotification(ServerMember member, string gif)
    {
        return new GarryMessageBuilder()
            .WithTheme(EmbedTheme.Egg)
            .WithDescription($"# 🥚 Egg {member.User.Mention} 🥚")
            .WithImage(gif)
            .WithFooter($"you have 🐣    {member.EggCount}    🐣 eggs")
            .BuildMessage();
    }

    public static DiscordInteractionResponseBuilder CreateCoinFlip(ServerMember member, FlipData flipData)
    {
        var button = new DiscordButtonComponent(
            DiscordButtonStyle.Success, 
            flipData.ToButtonId(), 
            "Flip Again", 
            false, 
            new DiscordComponentEmoji("🪙")
        );
        
        return new GarryMessageBuilder()
            .WithTitle("Coin Flip")
            .WithDescription($"# {flipData.LastFlip}")
            .WithUserBranding(member)
            .WithField("Heads", flipData.HeadCount.ToString(), true)
            .WithField("Tails", flipData.TailsCount.ToString(), true)
            .WithTimestamp()
            .WithButtons(button)
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateIdentityDisplay(DiscordGuild guild, List<ServerMember> members, string page)
    {
        return new GarryMessageBuilder()
            .WithTitle("Who are these people?")
            .WithGuildBranding(guild)
            .WithPagination(new PageContext<ServerMember>(members, 10, page, IDHelper.Misc.WHO))
            .WithColor(DiscordColor.Azure)
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateIdentityCard(ServerMember member)
    {
        return new GarryMessageBuilder()
            .WithTitle("Who are you?")
            .WithUserBranding(member)
            .WithDescription($"# **{member.Name!}**")
            .WithColor(DiscordColor.Aquamarine)
            .Build();
    }

    public static DiscordInteractionResponseBuilder CreateRandomNumber(int result, int min, int max)
    {
        return new GarryMessageBuilder()
            .WithTitle("Random Number")
            .WithDescription($"# `{result}`")
            .WithField("Min", min.ToString(), true)
            .WithField("Max", max.ToString(), true)
            .WithColor(DiscordColor.Lilac)
            .Build();
    }

    public static DiscordMessageBuilder CreateGuildEventMessage(string message, string eventUrl)
    {
        return new GarryMessageBuilder()
            .WithDescription($"### {message}")
            .WithContent(eventUrl)
            .BuildMessage();
    }

    #endregion

    #region Helper Methods

    private static string GetRankEmoji(int rank) => rank switch
    {
        1 => "🥇",
        2 => "🥈",
        3 => "🥉",
        _ => ""
    };

    #endregion
}