using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;

namespace SaulGoodmanBot.Library;

public class WheelPickers {
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        var data = WheelPickerProcessor.LoadAllWheels(Guild.Id);

        foreach (var row in data) {
            if (Wheels.ContainsKey(row.WheelName)) {
                if (row.TempRemoved == 1) {
                    Wheels[row.WheelName].RemovedOptions.Add(row.WheelOption);
                } else {
                    Wheels[row.WheelName].Options.Add(row.WheelOption);
                }
                Wheels[row.WheelName].Image = row.ImageUrl;
            } else {
                if (row.TempRemoved == 1) {
                    Wheels[row.WheelName] = new Wheel(Guild, row.WheelName, new List<string>(), new List<string>(){row.WheelOption}, row.ImageUrl);
                } else {
                    Wheels[row.WheelName] = new Wheel(Guild, row.WheelName, new List<string>(){row.WheelOption}, new List<string>(), row.ImageUrl);
                }
            }
        }
    }

    public void AddWheel(string name, string first_option, string? imgurl) {
        WheelPickerProcessor.AddWheelOption(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = name,
            WheelOption = first_option,
            ImageUrl = imgurl
        });
    }

    public bool Contains(string name) {
        return Wheels.ContainsKey(name);
    }

    public void DeleteWheel(Wheel wheel) {
        WheelPickerProcessor.DeleteWheel(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = wheel.Name,
        });
    }

    public bool IsFull() {
        return Wheels.Count == WHEEL_LIMIT;
    }

    private DiscordGuild Guild { get; set; }
    public Dictionary<string, Wheel> Wheels = new();
    private const int WHEEL_LIMIT = 25;

    public class Wheel {
        public Wheel(DiscordGuild guild, string name, List<string> options, List<string> removedOptions, string? imgurl=null) {
            Guild = guild;
            Name = name;
            Options = options;
            RemovedOptions = removedOptions;
            Image = imgurl;
        }

        public string Spin() {
            var random = new Random();
            var i = random.Next(Options.Count);
            return Options[i];
        }

        public void AddOption(string option) {
            WheelPickerProcessor.AddWheelOption(new WheelPickerModel() {
                GuildId = (long)Guild.Id,
                WheelName = Name,
                WheelOption = option,
                ImageUrl = Image
            });
        }

        public void DeleteOption(string option) {
            WheelPickerProcessor.DeleteWheelOption(new WheelPickerModel() {
                GuildId = (long)Guild.Id,
                WheelName = Name,
                WheelOption = option
            });
        }

        public void TemporarilyRemoveOption(string option) {
            WheelPickerProcessor.TemporarilyRemoveOption(new WheelPickerModel() {
                GuildId = (long)Guild.Id,
                WheelName = Name,
                WheelOption = option,
                TempRemoved = 1
            });
            Options.Remove(option);
            RemovedOptions.Add(option);
        }

        public void Reload() {
            WheelPickerProcessor.ReloadWheel(new WheelPickerModel() {
                GuildId = (long)Guild.Id,
                WheelName = Name,
                TempRemoved = 0
            });
        }

        public List<string> GetAllOptions() {
            return Options.Concat(RemovedOptions).ToList();
        }

        private DiscordGuild Guild { get; set; }
        public string Name { get; private set; }
        public List<string> Options { get; private set; }
        public List<string> RemovedOptions { get; private set; }
        public string? Image { get; set; } = null;
    }
}
