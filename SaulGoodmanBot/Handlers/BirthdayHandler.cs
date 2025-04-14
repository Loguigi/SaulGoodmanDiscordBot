using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System.Reflection;
using System.Timers;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class BirthdayHandler 
{
    public static async Task HandleBirthdayMessage(object source, ElapsedEventArgs e) 
    {
        foreach (var guild in DiscordHelper.ServerConfigs) 
        {
            guild.Value.Load();
            if (!guild.Value.BirthdayNotifications)
                continue;
            
            if (DateTime.Now.Hour != guild.Value.BirthdayTimerHour)
                continue;

            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.HotPink);

            var birthdays = new Birthdays(guild.Key);

            foreach (var birthday in birthdays.Members) 
            {
                switch (birthday.DaysUntilBirthday.Days)
                {
                    case 0:
                        embed.WithDescription($"# {DiscordEmoji.FromName(DiscordHelper.Client, ":birthday:", false)} {guild.Value.BirthdayMessage} {birthday.User.Mention} ({birthday.Age})");
                        await guild.Value.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed));
                        break;
                    case 1:
                        embed.WithDescription($"# {birthday.User.Mention}'s birthday is **__tomorrow__**!").WithFooter($"{DiscordEmoji.FromName(DiscordHelper.Client, ":birthday:", false)} {birthday.NextBirthday:D} {DiscordEmoji.FromName(DiscordHelper.Client, ":birthday:", false)}");
                        await guild.Value.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed));
                        break;
                    case 3:
                    case 5:
                    case 7:
                        embed.WithDescription($"# {birthday.User.Mention}'s birthday is in **__{birthday.DaysUntilBirthday.Days}__** {(birthday.DaysUntilBirthday.Days == 1 ? "day" : "days")}!").WithFooter($"{DiscordEmoji.FromName(DiscordHelper.Client, ":birthday:", false)} {birthday.NextBirthday:D} {DiscordEmoji.FromName(DiscordHelper.Client, ":birthday:", false)}");
                        await guild.Value.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed));
                        break;
                }
            }
        }
    }

    public static async Task HandleBirthdayList(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Birthdays.LIST)) 
        {
            await Task.CompletedTask;
            return;
        }

        try 
        {
            var birthdays = new Birthdays(e.Guild).GetInteractivity(IDHelper.GetId(e.Id, PAGE_INDEX));

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
                .WithTitle("Birthdays")
                .WithDescription(birthdays.IsEmpty())
                .WithColor(DiscordColor.Magenta)
                .WithFooter(birthdays.PageStatus);

            foreach (var birthday in birthdays)
            {
                embed.Description += $"### {birthday.User.Mention}: {birthday.Birthday:MMMM d} `({birthday.Age + 1})`\n";
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(birthdays.AddPageButtons().AddEmbed(embed)));
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private const int PAGE_INDEX = 1;
}