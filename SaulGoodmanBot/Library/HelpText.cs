namespace SaulGoodmanBot.Library;

public static class HelpText {
    public static readonly Dictionary<string, string> WheelPicker = new() {
        {"About", "The wheel picker commands allow you to create a wheel and spin it to get a random value.\n\n### 1. Creating a wheel\n### 2. Spinning your wheel\n### 3. Removing from a wheel"},
        {"Creating a wheel", "To create a new wheel, use </wheel create:1131501212961886208>\n`[name]`: Name of the new wheel\n`[first_option]`: First item to add to the new wheel\n`[image]` (*optional*): Picture that you want to represent the wheel\n\nOnce your wheel is created, you can add more options to it using </wheel add:1131501212961886208>. You can add options one at a time, or add multiple in one message with values separated by a new line (`[shift + enter]`). There is no limit to the size of a wheel\n\nYou can have a maximum of 25 wheels in your server."},
        {"Spinning your wheel", "Once you've added options to choose for your wheel, spin it using </wheel spin:1131501212961886208> to get a random value.\n\nAfter spinning, you can either spin or again or **temporarily remove** the option and spin again. Temporarily removing allows you to spin again without that option appearing without permanently deleting it from the wheel.\n\nTo reload the wheel and restore temporarily removed options, use </wheel reload:1131501212961886208>"},
        {"Removing from a wheel", "To remove an option from a wheel, use </wheel delete:1131501212961886208>.\n\nAfter selecting a wheel, you can either remove an option, or permanently delete the **entire** wheel."},
    };
}