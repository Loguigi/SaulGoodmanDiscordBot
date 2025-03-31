namespace SaulGoodmanBot.Library;

public class Magic8Ball {
    private static readonly List<string> Answers = 
    [
        // Affirmitave answers
        "It is certain",
        "It is decidedly so",
        "Without a doubt",
        "Yes definitely",
        "You may rely on it",
        "As I see it, yes",
        "Most likely",
        "Outlook good",
        "Yes",
        "Signs point to yes",
        "Yeah bitch!! Magnets!",

        // Non-committal answers
        "Reply hazy, try again",
        "Ask again later",
        "Better not tell you now",
        "Cannot predict now",
        "Concentrate and ask again",
        "I'M A BLOWFISH! BLOWFISH! YEEEAAAH! BLOWFISHIN' THIS UP!",

        // Negative answers
        "Don't count on it",
        "My reply is no",
        "My sources say no",
        "Outlook not so good",
        "Very doubtful",
        "Nah come on… man, some straight like you giant stick up his ass all of a sudden at age what 60 he's just going to break bad?"
    ];

    private static readonly Random random = new();

    public static string GetAnswer() {
        int r = random.Next(Answers.Count);
        return Answers[r];
    }
}