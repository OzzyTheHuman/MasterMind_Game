using System.Reflection.Metadata.Ecma335;

namespace MasterMind_Game;
class Program
{
    // The CLI is bugged in Rider when opening in an external window,
    // To see it properly, scroll up the window and rerun the app,
    // Visual Studio handles this better
    static void Main(string[] args)
    {
        bool gameRunning = true;
        
        Console.Clear();
        Console.WriteLine("                                                                                                                    ");
        Console.Clear();
        
        while (gameRunning)
        {
            Console.Clear();
            Console.WriteLine("=== Mastermind The Game ===\n");
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Quit");
            
            string input = Console.ReadLine();
            Console.Clear();
            switch (input)
            {
                case "1":
                    PlayGame();
                    break;
                case "2":
                    gameRunning = false;
                    break;
            }
        }

        Console.Clear();
        Console.WriteLine("Thanks for playing !");
    }

    static void PlayGame()
    {
        Game game = new Game();
        AttemptResult attempt = new AttemptResult();
        
        ShowRules(game);
        while(!game.IsGameOver)
        {
            //ShowListInColor(game.GetSecretCode());
            Console.WriteLine(">-------------------------------------------------------------------<");
            Console.Write($"Round: {game.CurrentRound} \t\tAvailable colors: ");
            ShowInColor(game.GetAvailableColors());
            
            string guess = Console.ReadLine();
            List<string> cleanedGuess = guess.Select(x => x.ToString().Trim().ToLower()).ToList();
            ShowInputInColor(cleanedGuess);
            try
            {
                attempt = game.GetAttemptFeedback(cleanedGuess);
                Console.WriteLine();
                ShowAttemptResults(attempt);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"You need to enter {game.CodeLength} letters");
            }

            if (attempt.IsSurrendered)
            {
                Console.WriteLine();
                ShowSecretCodeAndWait(game);
            }
            else if (attempt.IsVictory)
            {
                Console.WriteLine();
                Console.WriteLine("Congratulations! You won!");
                WaitForResponse();
            }
            else if (game.IsGameOver)
            {
                Console.WriteLine();
                Console.WriteLine("Time's up! Sadly you didn't guess the code in time");
                ShowSecretCodeAndWait(game);
            }
        }
    }

    static string ParseSecretCodeToString(Game game)
    {
        List<string> list = game.GetSecretCode();
        string result = "";
        foreach (var n in list)
        {
            result += "[" + n + "]";
        }

        return result;
    }

    static void ShowAvailableColors(Game game)
    {
        List<string> allColors = game.GetAvailableColors();
        Console.WriteLine("Available colors to choose from:");
        foreach (string color in allColors)
        {
            switch (color)
            {
                case "r":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("r - red");
                    Console.ResetColor();
                    break;
                case "y":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("y - yellow");
                    Console.ResetColor();
                    break;
                case "g":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("g - green");
                    Console.ResetColor();
                    break;
                case "b":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("b - blue");
                    Console.ResetColor();
                    break;
                case "m":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("m - magneta");
                    Console.ResetColor();
                    break;
                case "c":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("c - cyan");
                    Console.ResetColor();
                    break;
                default:
                    Console.ResetColor();
                    Console.WriteLine(color);
                    Console.ResetColor();
                    break;
            }
        }

        Console.WriteLine();
    }

    static void WaitForResponse()
    {
        Console.WriteLine();
        Console.WriteLine("Press any button to continue ...");
        Console.ReadKey();
        Console.Clear();
    }
    
    static void ShowSecretCodeAndWait(Game game)
    {
        Console.WriteLine("Correct answer was:");
        ShowInColor(game.GetSecretCode());
        WaitForResponse();
    }

    static void ShowAttemptResults(AttemptResult attempt)
    {
        Console.WriteLine($"Accurate answers: {attempt.AccurateAnswer}");
        Console.WriteLine($"Not accurate answers: {attempt.NotAccurateAnswer}");
    }

    static void ShowRules(Game game)
    {
        Console.WriteLine("Rules:");
        Console.WriteLine($"- Try to guess the secret code consisting of {game.CodeLength} colors,");
        Console.WriteLine($"- You have {game.AllRounds} rounds to do so,");
        Console.WriteLine("- Enter only the first letters of the colors, for example: gggg, cyrm etc.");
        Console.WriteLine("- If you want to give up, type in: \"q\" or \"quit\"");
        Console.WriteLine();
    }

    static void ShowInColor(List<string> list)
    {
        foreach (string color in list)
        {
            switch (color)
            {
                case "r":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[r]");
                    Console.ResetColor();
                    break;
                case "y":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[y]");
                    Console.ResetColor();
                    break;
                case "g":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[g]");
                    Console.ResetColor();
                    break;
                case "b":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[b]");
                    Console.ResetColor();
                    break;
                case "m":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("[m]");
                    Console.ResetColor();
                    break;
                case "c":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("[c]");
                    Console.ResetColor();
                    break;
                default:
                    Console.ResetColor();
                    Console.Write("[" + color + "]");
                    Console.ResetColor();
                    break;
            }
        }

        Console.WriteLine();
    }

    static void ShowInputInColor(List<string> guess)
    {
        Console.CursorTop--;
        ShowInColor(guess);
    }
}