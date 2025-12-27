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
        bool choosingMainMenuOption = true;
        while (choosingMainMenuOption)
        {
            ClearConsole();
            Console.WriteLine("=== Mastermind The Game ===\n");
            Console.WriteLine("1. New Game (this will delete your save file!)");
            Console.WriteLine("2. Continue");
            Console.WriteLine("3. Quit");
            
            string input = Console.ReadLine();
            ClearConsole();
            switch (input)
            {
                case "1":
                    bool choosingGameVariant = true;
                    while (choosingGameVariant)
                    {
                        Console.WriteLine("=== Select variant ===\n");
                        Console.WriteLine("1. Normal");
                        Console.WriteLine("   - Colors: 6");
                        Console.WriteLine("   - Code length: 4");
                        Console.WriteLine("   - Rounds: 9\n");
                        Console.WriteLine("2. Liar Mode");
                        Console.WriteLine("   - The game will try to trick you by giving you wrong answers");
                        Console.WriteLine("   - There is 33% that the answer you got is wrong, this can happen twice");
                        Console.WriteLine("   - Other parameters are the same as in normal mode\n");
                        Console.WriteLine("3. Digits Mode");
                        Console.WriteLine("   - Use digits [0-9] instead of colors");
                        Console.WriteLine("   - Code length: 4");
                        Console.WriteLine("   - Rounds: 12\n");
                        Console.WriteLine("4. Custom");
                        Console.WriteLine("   - Configure all options to your liking\n");
                        Console.WriteLine("5. Go back");
                        
                        string input2 = Console.ReadLine();
                        ClearConsole();
                        
                        Game.DeleteSavedGame();
                        switch (input2)
                        {
                            case "1":
                                PlayGame(new Game(symbolsCount: 6));
                                choosingGameVariant = false;
                                break;
                            
                            case "2":
                                PlayGame(new Game(symbolsCount: 6, liesCount: 2));
                                choosingGameVariant = false;
                                break;
                            
                            case "3":
                                PlayGame(new Game(symbolsCount: 10, rounds: 12, useColors: false));
                                choosingGameVariant = false;
                                break;

                            // TODO: Add Digits Mode integration 
                            case "4":
                                Console.WriteLine("=== Configure your game ===\n");
                                ShowCustomGameSettings();
                                int colorsCount = 0;
                                int codeLength = 0;
                                int roundsCount = 0;
                                int liesCount = 0;
                                bool configuringGame = true;
                                while (configuringGame)
                                {
                                    Console.Write("Colors (default is 6, pick between 6 and 8): ");
                                    string inputColors = Console.ReadLine();
                                    if (int.TryParse(inputColors, out colorsCount))
                                    {
                                        if (colorsCount is >= 6 and <= 8)
                                        {
                                            break;
                                        }
                                    }

                                    ClearConsoleOneLine();
                                }
                                while (configuringGame)
                                {
                                    Console.Write($"Code length (default is 4, pick between 4 and {colorsCount}): ");
                                    string inputCodeLength = Console.ReadLine();
                                    if (int.TryParse(inputCodeLength, out codeLength))
                                    {
                                        if (codeLength >= 4 && codeLength <= colorsCount)
                                        {
                                            break;
                                        }
                                    }

                                    ClearConsoleOneLine();
                                }
                                while (configuringGame)
                                {
                                    Console.Write("Number of rounds (default is 9): ");
                                    string roundsInput = Console.ReadLine();
                                    if (int.TryParse(roundsInput, out roundsCount))
                                    {
                                        if (roundsCount >= 9)
                                        {
                                            break;
                                        }
                                    }

                                    ClearConsoleOneLine();
                                }

                                while (configuringGame)
                                {
                                    Console.Write("How many times the game should try to trick you? (0 is disabled): ");
                                    string liesInput = Console.ReadLine();
                                    if (int.TryParse(liesInput, out liesCount))
                                    {
                                        if (liesCount >= 0)
                                        {
                                            break;
                                        }
                                    }

                                    ClearConsoleOneLine();
                                }

                                ClearConsole();
                                
                                PlayGame(new Game(roundsCount, codeLength, colorsCount, liesCount));
                                choosingGameVariant = false;
                                break;
                            
                            case "5":
                                choosingGameVariant = false;
                                break;
                        }
                    }
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
                    choosingMainMenuOption = false;
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
            // CHEAT: uncomment method below to see generated code
            // ShowInColor(game.GetSecretCode()); 

            Console.WriteLine(">-------------------------------------------------------------------<");
            if (game.IsUsingColors)
            {
                Console.Write($"Round: {game.CurrentRound} \t\tAvailable colors: ");
                ShowEssentialValues(game.GetAvailableEssentialValues());
            }
            else
            {
                Console.Write($"Round: {game.CurrentRound} \t\tAvailable digits: ");
                ShowEssentialValues(game.GetAvailableEssentialValues());
            }
            
            string guess = Console.ReadLine();
            if (guess.ToLower().Trim() == "q" || guess.ToLower().Trim() == "quit")
            {
                game.Surrender();
                if (game.CurrentRound != 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Game progress is saved, you can close the game");
                }
                WaitForResponse();
                continue;
            }
            
            List<string> cleanedGuess = guess.Split([' '], StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.ToString().Trim().ToLower())
                                             .ToList();
            try
            {
                AttemptResult attempt = game.GetAttemptFeedback(cleanedGuess);
                ShowInputInColor(cleanedGuess);
                Console.WriteLine();
                ShowAttemptResults(attempt);
            }
            catch (ArgumentException)
            {
                if (game.IsUsingColors)
                {
                    Console.WriteLine($"You need to enter {game.CodeLength} valid colors");
                }
                else
                {
                    Console.WriteLine($"You need to enter {game.CodeLength} valid digits");
                }
                
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

    static void ClearConsoleOneLine()
    {
        Console.CursorTop--;
        Console.WriteLine("\x1b[2K");
        Console.CursorTop--;
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
            ShowInputInColor(oldAttempt.GuessedValues);
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
        ShowEssentialValues(game.GetSecretCode());
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
        if (game.IsUsingColors)
        {
            Console.WriteLine($"- Try to guess the secret code consisting of {game.CodeLength} colors");
            Console.WriteLine($"- You have {game.AllRounds} rounds to do so");
            Console.WriteLine("- Separate colors with a single space, for example: \"g g g g\" \"c y r m\" etc.");
        }
        else
        {
            Console.WriteLine($"- Try to guess the secret code consisting of {game.CodeLength} digits");
            Console.WriteLine($"- You have {game.AllRounds} rounds to do so");
            Console.WriteLine("- Separate colors with a single space, for example: \"1 2 3 4\" \"9 0 1 2\" etc.");
        }
        
        Console.WriteLine("- Progress is saved automatically after each valid guess");
        Console.WriteLine("- If you want to quit, type in: \"q\" or \"quit\"\n");
        if (game.InitialLiesCount > 0)
        {
            Console.WriteLine("- Lying mode is enabled. The game will try to trick you.");
        }
        Console.WriteLine();
    }

    static void ShowCustomGameSettings()
    {
        Console.WriteLine("Colors (default is 6, pick between 6 and 8): ");
        Console.WriteLine("Code length (default is 4): ");
        Console.WriteLine("Number of rounds (default is 9): ");
        Console.WriteLine("How many times the game should try to trick you? (0 is disabled): ");
        Console.CursorTop--;
        Console.CursorTop--;
        Console.CursorTop--;
        ClearConsoleOneLine();
    }

    static void ShowEssentialValues(List<string> list)
    {
        foreach (string n in list)
        {
            switch (n)
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
                case "w":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[w]");
                    Console.ResetColor();
                    break;
                case "dg":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("[dg]");
                    Console.ResetColor();
                    break;
                default:
                    Console.ResetColor();
                    Console.Write(n + " ");
                    Console.ResetColor();
                    break;
            }
        }

        Console.WriteLine();
    }

    static void ShowInputInColor(List<string> guess)
    {
        Console.CursorTop--;
        ShowEssentialValues(guess);
    }
}