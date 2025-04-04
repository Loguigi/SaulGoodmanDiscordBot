// using DSharpPlus;
// using DSharpPlus.Entities;
// using DSharpPlus.EventArgs;
// using SaulGoodmanBot.Library;
// using SaulGoodmanBot.Helpers;
//
// namespace SaulGoodmanBot.Handlers;
//
// public static class HelpHandler {
//     public static async Task HandleSetupHelp(DiscordClient s, ComponentInteractionCreateEventArgs e) {
//         if (e.Id != IDHelper.Help.SETUP) {
//             await Task.CompletedTask;
//             return;
//         }
//
//         var page = e.Values.First();
//
//         var embed = new DiscordEmbedBuilder()
//             .WithAuthor("Saul Goodman", "", ImageHelper.Images["Heisenberg"])
//             .WithTitle(page)
//             .WithDescription(HelpText.Setup[page])
//             .WithThumbnail(ImageHelper.Images["Saul"])
//             .WithColor(DiscordColor.Orange);
//         
//         var pages = HelpText.Setup.Keys.Select(p => new DiscordSelectComponentOption(p, p)).ToList();
//         var dropdown = new DiscordSelectComponent(IDHelper.Help.SETUP, "Select a page...", pages);
//
//         await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));
//     }
//
//     public static async Task HandleWheelPickerHelp(DiscordClient s, ComponentInteractionCreateEventArgs e) {
//         if (e.Id != IDHelper.Help.WHEELPICKER) {
//             await Task.CompletedTask;
//             return;
//         }
//
//         var page = e.Values.First();
//
//         var embed = new DiscordEmbedBuilder()
//             .WithAuthor("Wheel Picker", "", ImageHelper.Images["PS2Jesse"])
//             .WithTitle(page)
//             .WithDescription(HelpText.WheelPicker[page])
//             .WithColor(DiscordColor.Gold);
//         
//         var pages = new List<DiscordSelectComponentOption>();
//         foreach (var p in HelpText.WheelPicker.Keys) {
//             pages.Add(new DiscordSelectComponentOption(p, p));
//         }
//         var dropdown = new DiscordSelectComponent(IDHelper.Help.WHEELPICKER, "Select a page...", pages);
//
//         await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));
//     }
//
//     public static async Task HandleBirthdayHelp(DiscordClient s, ComponentInteractionCreateEventArgs e) {
//         if (e.Id != IDHelper.Help.BIRTHDAY) {
//             await Task.CompletedTask;
//             return;
//         }
//
//         var page = e.Values.First();
//
//         var embed = new DiscordEmbedBuilder()
//             .WithAuthor("Birthdays", "", ImageHelper.Images["SmilingGus"])
//             .WithTitle(page)
//             .WithDescription(HelpText.Birthday[page])
//             .WithColor(DiscordColor.HotPink);
//         
//         var pages = new List<DiscordSelectComponentOption>();
//         foreach (var p in HelpText.Birthday.Keys) {
//             pages.Add(new DiscordSelectComponentOption(p, p));
//         }
//         var dropdown = new DiscordSelectComponent(IDHelper.Help.BIRTHDAY, "Select a page...", pages);
//
//         await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));
//     }
//
//     public static async Task HandleScheduleHelp(DiscordClient s, ComponentInteractionCreateEventArgs e) {
//         if (e.Id != IDHelper.Help.SCHEDULE) {
//             await Task.CompletedTask;
//             return;
//         }
//
//         var page = e.Values.First();
//
//         var embed = new DiscordEmbedBuilder()
//             .WithAuthor("Schedules", "", ImageHelper.Images["Finger"])
//             .WithTitle(page)
//             .WithDescription(HelpText.Schedule[page])
//             .WithColor(DiscordColor.DarkBlue);
//         
//         var pages = new List<DiscordSelectComponentOption>();
//         foreach (var p in HelpText.Schedule.Keys) {
//             pages.Add(new DiscordSelectComponentOption(p, p));
//         }
//         var dropdown = new DiscordSelectComponent(IDHelper.Help.SCHEDULE, "Select a page...", pages);
//
//         await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));
//     }
// }