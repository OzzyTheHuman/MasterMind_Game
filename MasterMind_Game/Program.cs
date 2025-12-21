using System.Reflection.Metadata.Ecma335;

namespace MasterMind_Game;
class Program
{
    
    static void Main(string[] args)
    {
        bool gameRunning = true;
        
        Console.Clear();
        Console.WriteLine("                                                                                                                    ");
        Console.Clear();
        
        while (gameRunning)
        {
            Console.Clear();
            Console.WriteLine("=== Mastermind The Game ===");
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
        var game = new Game();
        ShowAvailableColors(game);
        while (true)
        {
            Console.WriteLine(ParseSecretCodeToString(game.GetSecretCode()));
            
            string guess = Console.ReadLine();
            
            if (guess.ToLower() == "q" || guess.ToLower() == "quit")
            {
                Console.WriteLine($"Correct answer was : {ParseSecretCodeToString(game.GetSecretCode())}");
                Console.WriteLine("Press any button to continue ...");
                Console.ReadKey();
                Console.Clear();
                break;
            }
            
            List<string> cleanedGuess = guess.Select(x => x.ToString().Trim().ToLower()).ToList();
            AttemptResult attempt = game.GetAttemptFeedback(cleanedGuess);
            
            if (attempt.IsVictory)
            {
                Console.WriteLine("Congratulations! You won!");
                Console.WriteLine($"Correct answer was : {ParseSecretCodeToString(game.GetSecretCode())}");
                Console.WriteLine();
                Console.WriteLine("Press any button to continue ...");
                Console.ReadKey();
                break;
            }
            
            Console.WriteLine();
            Console.WriteLine($"Accurate answers: {attempt.AccurateAnswer}");
            Console.WriteLine($"Not accurate answers: {attempt.NotAccurateAnswer}");
            Console.WriteLine();
        }
    }

    static string ParseSecretCodeToString(List<string> list)
    {
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
}