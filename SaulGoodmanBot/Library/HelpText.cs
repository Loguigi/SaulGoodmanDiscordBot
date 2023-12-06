namespace SaulGoodmanBot.Library;

public static class HelpText {
    public static readonly Dictionary<string, string> WheelPicker = new() {
        {"About", 
            "The wheel picker commands allow you to create a wheel and spin it to get a random value.\n\n" + 
            "### 1. Creating a wheel\n" +
            "### 2. Spinning your wheel\n" +
            "### 3. Removing from a wheel"},
        {"Creating a wheel", 
            "To create a new wheel, use </wheel create:1131501212961886208>\n" +
            "`[name]`: Name of the new wheel\n" +
            "`[first_option]`: First item to add to the new wheel\n" +
            "`[image]` (*optional*): Picture that you want to represent the wheel\n\n" +
            "Once your wheel is created, you can add more options to it using </wheel add:1131501212961886208>. You can add options one at a time, or add multiple in one message with values separated by a new line (`[shift + enter]`). There is no limit to the size of a wheel\n\n" +
            "You can have a maximum of 25 wheels in your server."},
        {"Spinning your wheel", 
            "Once you've added options to choose for your wheel, spin it using </wheel spin:1131501212961886208> to get a random value.\n\n" +
            "After spinning, you can either spin or again or **temporarily remove** the option and spin again. Temporarily removing allows you to spin again without that option appearing without permanently deleting it from the wheel.\n\n" +
            "To reload the wheel and restore temporarily removed options, use </wheel reload:1131501212961886208>"},
        {"Removing from a wheel", 
            "To remove an option from a wheel, use </wheel delete:1131501212961886208>.\n\n" +
            "After selecting a wheel, you can either remove an option, or permanently delete the **entire** wheel."},
    };

    public static readonly Dictionary<string, string> Birthday = new() {
        {"About", 
            "The birthday commands allow you to keep track of birthdays in your server\n" +
            "There are 2 notifications for birthdays - one for 5 days before a birthday, and one for the day of a birthday. They are enabled by default, but an admin can disable them using </config birthday_notifications:1134335362681032806>.\n\n" +
            "### 1. Setting your birthday\n" +
            "### 2. Viewing others' birthdays\n" +
            "### 3. Setting a custom birthday message (Admins)"},
        {"Setting your birthday",
            "To set your birthday, use </birthday add:1132924580349083718>.\n\n" +
            "If birthday notifications are enabled, your friends will know when your birthday is coming up!\n\n" +
            "For admins, you can use </birthday change:1132924580349083718> to manually add somebody's birthday."},
        {"Viewing others' birthdays",
            "There are 3 different commands to view your friends' birthdays.\n\n" +
            "### </birthday next:1132924580349083718> - See who's birthday is coming next\n" +
            "### </birthday check:1132924580349083718> - Check somebody's birthday individually\n" +
            "### </birthday list:1132924580349083718> - View all the birthdays in the server"},
        {"Setting a custom birthday message",
            "The default birthday message is \"Happy birthday! `@user`\"\n\n" +
            "This can be changed to whatever message you want by an admin using </config messages:1134335362681032806>. You can use emojis in the message too!"}
    };
}