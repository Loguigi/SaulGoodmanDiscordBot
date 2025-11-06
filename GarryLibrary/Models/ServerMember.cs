using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;

namespace GarryLibrary.Models;

public class ServerMember : IPageable
{
    #region Mapped Properties

    public int Id { get; set; }
    public long UserId { get; set; }
    public long GuildId { get; set; }
    public string? Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public DateTime? Birthday { get; set; }
    public int EggCount { get; set; }
    public bool Active { get; set; }
    public SantaParticipant? SantaParticipant { get; set; }
    #endregion
    
    #region Unmapped Properties
    [NotMapped] public DiscordUser User { get; set; } = null!;
    [NotMapped] public DiscordGuild? Guild { get; set; }
    [NotMapped] public string DisplayName => Name ?? User.Username;
    [NotMapped] public string DisplayMention => Name ?? User.Mention;
    [NotMapped] public int Rank { get; set; }
    [NotMapped] public int ExpNeededForNextLevel => 2 * (int)Math.Pow(Level + 1, 2) - 2;
    [NotMapped] public string? FormattedBirthday => Birthday?.ToString("MMMM d, yyyy");
    [NotMapped] public int Age 
    {
        get 
        {
            if (Birthday == null) return 0;
            
            var current = DateTime.Now;
            var age = current.Year - Birthday.Value.Year;

            if (current.Month == Birthday.Value.Month) 
            {
                // birthday is within month
                if (current.Day < Birthday.Value.Day) 
                {
                    // birthday hasn't occured yet
                    age--;
                }
            } 
            else if (current.Month < Birthday.Value.Month) 
            {
                // birthday month not reached yet
                age--;
            }

            return age;
        }
    }

    [NotMapped] public DateTime? NextBirthday => Birthday?.AddYears(Age + 1);
    [NotMapped] public bool? HasBirthdayToday => Birthday?.Month == DateTime.Now.Month && Birthday?.Day == DateTime.Now.Day;
    [NotMapped] public TimeSpan? DaysUntilBirthday => NextBirthday?.Date - DateTime.Today.Date;

    #endregion

    public string GetPageItemDisplay(string context) => context switch
    {
        IDHelper.Levels.LEADERBOARD => $"{GetRankText()} {DisplayMention} `LVL {Level}` `{Experience} XP`",
        IDHelper.Misc.WHO => $"### * {User.Mention} ➡️ {Name ?? "`?`"}",
        IDHelper.Birthdays.LIST => $"### {DisplayMention}: {Birthday:MMMM d} `({Age + 1})`",
        IDHelper.Misc.EGG => $"### 🥚 {DisplayMention}: `{EggCount}` 🥚",
        _ => ""
    };

    private string GetRankText() => Rank switch
    {
        1 => "### 🥇",
        2 => "### 🥈",
        3 => "### 🥉",
        _ => $"### **__#{Rank}__**"
    };
}