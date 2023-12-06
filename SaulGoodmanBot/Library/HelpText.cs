namespace SaulGoodmanBot.Library;

public static class HelpText {
    public static readonly Dictionary<string, string> Setup = new() {
        {"It's time for a real lawyer",
            "### Who's up slippin they Jimmy?\n" +
            "Thanks for adding the Saul Goodman Bot! This bot provides many general features, such as:\n\n" +
            "* Fun commands, like wheel picker, coin flip, and magic 8ball\n" +
            "* Birthday notifications\n" +
            "* Levelling system\n" +
            "* Role assignment and management\n" +
            "* Schedule inputting to help planning\n" +
            "* Minecraft commands for saving waypoints\n" +
            "* Secret Santa management for the holidays\n\n" +
            $"For any problems or questions, please DM {Program.Client!.GetUserAsync(263070689559445504).Result.Mention}, the creator of the bot"},
        {"Bot setup and customization",
            "### Custom messages\n" +
                "Set all custom messages using </config messages:1134335362681032806>\n" +
                "`Welcome message`: message for members that join the server\n" +
                "`Leave message`: message to notify the server when someone has left\n" +
                "`Birthday message`: custom birthday message\n" +
                "`Level up message`: custom message when somebody levels up, if enabled\n\n" +
            "### Config\n" +
                "</config levels:1134335362681032806>: enable or disable levels **(disabled by default)**\n" +
                "</config defaultchannel:1134335362681032806>: set the channel for bot messages to appear\n" +
                "</config birthday_notifications:1134335362681032806>: enable or disable birthday notifications **(enabled by default)**\n\n" +
            "### Other Setup\n" +
                "</role setup:1137537906966286386>: setup assignable roles. More info in </help role:1181749629088436336>\n" +
                "</mc save:1146296828782989365>: setup info for a Minecraft server. More info in </help minecraft:1181749629088436336>"},
        {"Other help commands",
            "### </help wheel:1181749629088436336>\n" +
            "### </help birthday:1181749629088436336>\n" +
            "### </help levels:1181749629088436336>\n" +
            "### </help schedule:1181749629088436336>\n" +
            "### </help secret_santa:1181749629088436336>\n" +
            "### </help misc:1181749629088436336>"}
    };

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
            "### </birthday list:1132924580349083718> - View all the birthdays in the server\n\n" +
            "Alternatively, you can `[right-click]` somebody, go to `Apps`, and select `Birthday`."},
        {"Setting a custom birthday message",
            "The default birthday message is \"Happy birthday! `@user`\"\n\n" +
            "This can be changed to whatever message you want by an admin using </config messages:1134335362681032806>. You can use emojis in the message too!"}
    };

    public static readonly string Levels =
        "Levels can be enabled with </config levels:1134335362681032806>\n\n" +
        "Once enabled, you gain `1 EXP` per message sent, with a cooldown of `1 minute`, so you cannot level up by spamming messages\n\n" +
        "You can check your own or somebody else's level using </level:1139784458619334796>, or `[right-click]` a user, going to `Apps`, and selecting `Level`\n\n" +
        "To check the leaderboard, use </leaderboard:1139784458619334797> to see who is the biggest no-life";

    public static readonly Dictionary<string, string> Schedule = new() {
        {"About",
            "The schedule commands allow you to save your work or school schedule for your friends to see. It is helpful for planning group events around everyones schedule.\n\n" +
            "### 1. Creating a schedule\n" +
            "### 2. Changing your schedule\n" +
            "### 3. Viewing others' schedules\n"},
        {"Creating a schedule",
            "### To save your schedule, use </schedule save:1160992042076356688>\n" +
            "* `recurring` - true if your schedule doesn't change, false if it changes weekly\n" +
            "* `sunday` - `saturday` - describe your schedule for the days you work. For the days that you're off, you can leave blank\n" +
            "* `picture` (optional) - if you would rather upload a picture of your schedule rather than typing it\n\n" +
            "### To change others' schedules, admins can use </schedule override:1160992042076356688>"},
        {"Changing your schedule",
            "You can change your entire schedule using </schedule save:1160992042076356688> again, which will override the current schedule you have set.\n\n" +
            "Alternatively, you can change your schedule for specific days using </schedule edit:1160992042076356688> or change your schedule picture with </schedule picture:1160992042076356688>.\n\n" +
            "To clear your schedule completely, use </schedule clear:1160992042076356688>."},
        {"Viewing others' schedules",
            "### Checking today\n" +
            "You can check who is busy today using </schedule today:1160992042076356688>\n\n" +
            "### Checking people's schedules\n" +
            "There are 2 different ways to check somebody's schedule. Either use </schedule check:1160992042076356688>, or `[right-click]` a user, go to `Apps`, and select `Schedule`."}
    };

    public static readonly string Misc = 
        "### </flip:1130848702471352332> - flips a coin\n" +
        "### </8ball:1130853100576583800> - ask the magic 8ball a question\n" +
        "### </poll:1181736173270478920> - create a poll for the server\n" +
        "### </dicksize:1134943696496902165> - this one speaks for itself";
}