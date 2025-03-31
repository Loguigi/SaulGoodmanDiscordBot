namespace SaulGoodmanLibrary.Helpers;

public static class IDHelper 
{
    public static string GetId(string id, int index) => id.Split('\\')[index];
    public static class WheelPicker 
    {
        public const string Add = "ADDOPTIONS";
        public const string List = "LISTWHEELS";
        public const string Spin = "SPINWHEEL";
        public const string DeleteWheel = "DELETEWHEELSELECTION";
        public const string DeleteOption = "DELETEOPTIONSELECTION";
        public const string ReloadWheel = "WHEELRELOAD";
    }

    public static class Birthdays 
    {
        public const string LIST = "LISTBIRTHDAYS";
    }

    public static class Roles 
    {
        public const string MENU = "ROLEMENU";
        public const string ASSIGN = "ROLEASSIGN";
        public const string REMOVE = "REMOVEROLE";
    }

    public static class Misc 
    {
        public const string FLIP = "COINFLIP";
    }

    public static class Levels 
    {
        public const string LEADERBOARD = "LEVELLEADERBOARD";
    }

    public static class Santa 
    {
        public const string PARTICIPANTS = "SANTAPARTICIPANTS";
        public const string GIFTSTATUSES = "SANTAGIFTSTATUSES";
        public const string WISHLISTREMOVE = "SANTAWISHLISTREMOVE";
    }

    public static class Help 
    {
        public const string SETUP = "SETUPHELP";
        public const string WHEELPICKER = "HELPWHEELPICKER";
        public const string BIRTHDAY = "HELPBIRTHDAYS";
        public const string SCHEDULE = "HELPSCHEDULE";
    }
}