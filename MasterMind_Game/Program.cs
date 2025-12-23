using System.Reflection.Metadata.Ecma335;

namespace MasterMind_Game;
class Program
{
    // The CLI is bugged in Rider when opening in an external window,
    // To see it properly, scroll up the window and rerun the app,
    // Default Windows CMD handles this better
    static void Main(string[] args)
    {
        ClearConsole();
        bool gameRunning = true;
        while (gameRunning)
        {
            ClearConsole();
            Console.WriteLine("=== Mastermind The Game ===\n");
            Console.WriteLine("1. New Game (this will delete your saved games!)");
            Console.WriteLine("2. Continue");
            Console.WriteLine("3. Quit");
            
            string input = Console.ReadLine();
            ClearConsole();
            switch (input)
            {
                case "1":
                    Game.DeleteSavedGame();
                    PlayGame(new Game(colorsCount:6));
                    break;
                case "2":
                    if (Game.HasSavedGame())
                    {
                        Game savedGame = Game.LoadGame();
                        PlayGame(savedGame);
                    }
                    else 
                    {
                        Console.WriteLine("You dont have any saved games");
                        WaitForResponse();
                    }
                    break;
                case "3":
                    gameRunning = false;
                    break;
                
            }
        }

        ClearConsole();
        Console.WriteLine("Thanks for playing !");
    }

    static void PlayGame(Game game)
    {
        ShowRules(game);
        ShowSavedGame(game);
        
        while(!game.IsGameOver)
        {
            //ShowInColor(game.GetSecretCode());

            Console.WriteLine(">-------------------------------------------------------------------<");
            Console.Write($"Round: {game.CurrentRound} \t\tAvailable colors: ");
            ShowInColor(game.GetAvailableColors());
            
            string guess = Console.ReadLine();
            if (guess.ToLower().Trim() == "q" || guess.ToLower().Trim() == "quit")
            {
                game.Surrender();
                Console.WriteLine();
                Console.WriteLine("Game progress is saved, you can close the game");
                WaitForResponse();
                continue;
            }
            
            List<string> cleanedGuess = guess.Select(x => x.ToString().Trim().ToLower()).ToList();
            try
            {
                AttemptResult attempt = game.GetAttemptFeedback(cleanedGuess);
                ShowInputInColor(cleanedGuess);
                Console.WriteLine();
                ShowAttemptResults(attempt);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"You need to enter {game.CodeLength} valid colors");
            }

            if (game.IsSurrendered)
            {
                Console.WriteLine("Game progress is saved, you can close the game");
                WaitForResponse();
            }
            else if (game.IsVictory)
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

    static void ClearConsole()
    {
        Console.Clear();
        Console.WriteLine("\x1b[2J");
        Console.WriteLine("\x1b[3J");
        Console.Clear();
    }
    static void ShowSavedGame(Game game)
    {
        if (game.History.Capacity > 0)
        {
            Console.WriteLine(">-------------------------------------------------------------------<");
            Console.WriteLine("Saved game: ");
            Console.WriteLine();
        }

        foreach (var oldAttempt in game.History)
        {
            Console.WriteLine();
            ShowInputInColor(oldAttempt.GuessedColors);
            Console.WriteLine();
            ShowAttemptResults(oldAttempt);
            Console.WriteLine();
        }
    }
    static void WaitForResponse()
    {
        Console.WriteLine();
        Console.WriteLine("Press any button to continue ...");
        Console.ReadKey();
        ClearConsole();
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
        Console.WriteLine("- If you want to save and exit, type in: \"q\" or \"quit\"");
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
                    Console.Write(color);
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