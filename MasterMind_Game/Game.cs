using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MasterMind_Game;

public class Game
{
    [JsonInclude]
    public int AllRounds { get; private set; }

    [JsonInclude]
    public int CodeLength { get; private set; }

    [JsonInclude]
    public int SymbolsCount { get; private set; }

    [JsonInclude]
    public int CurrentRound { get; private set; } = 1;

    [JsonInclude]
    public bool IsGameOver { get; private set; }

    [JsonInclude]
    public bool IsPaused { get; private set; }

    [JsonInclude]
    public bool IsVictory { get; private set; }

    [JsonInclude]
    public List<string> SecretCode { get; private set; } = new List<string>();

    [JsonInclude]
    public List<AttemptResult> History { get; private set; } = new List<AttemptResult>();
    
    [JsonInclude]
    public int LiesCount { get; private set; }

    [JsonInclude] 
    public int InitialLiesCount { get; private set; }
    
    [JsonInclude]
    public List<string> EssentialValuesSet { get; private set; }
    
    [JsonInclude]
    public bool IsUsingColors { get; private set; }
        
    public static readonly List<string> Colors = ["r", "y", "g", "b", "m", "c", "w", "dg"];

    public static readonly List<string> Numbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    
    private const string SaveFileName = "savedgamedata.json";

    public Game(int rounds = 9, int codeLength = 4, int symbolsCount = 6, int liesCount = 0, bool useColors = true)
    {
        AllRounds = rounds;
        CodeLength = codeLength;
        SymbolsCount = symbolsCount;
        LiesCount = liesCount;
        InitialLiesCount = liesCount;
        IsUsingColors = useColors;
        if (useColors)
        {
            EssentialValuesSet = Colors;
        }
        else
        {
            EssentialValuesSet = Numbers;
        }

        if (SymbolsCount > EssentialValuesSet.Count)
        {
            throw new ArgumentException($"You cannot choose {SymbolsCount} elements, because {EssentialValuesSet.Count} is the limit");
        }

        GenerateSecretCode();
    }

    [JsonConstructor]
    public Game()
    { }
    
    private void GenerateSecretCode()
    {
        if (SecretCode.Count > 0 ) return;
        
        Random rnd = new Random();
        for (int i = 0; i < CodeLength; i++)
        {
            int x = rnd.Next(0, SymbolsCount);
            SecretCode.Add(EssentialValuesSet[x]);
        }
    }
    
    public List<string> GetSecretCode()
    {
        return SecretCode;
    }

    public List<string> GetAvailableEssentialValues()
    {
        return EssentialValuesSet.Take(SymbolsCount).ToList();
    }

    public void Exit()
    {
        IsPaused = true;
        IsGameOver = true;
    }

    public static bool HasSavedGame()
    {
        if (File.Exists(SaveFileName))
        {
            return true;
        }

        return false;
    }

    public static void DeleteSavedGame()
    {
        if (File.Exists(SaveFileName))
        {
            File.Delete(SaveFileName);
        }
    }
    
    public void SaveGame()
    {
        string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SaveFileName, jsonString);
    }

    public static Game LoadGame()
    {
        if (!HasSavedGame()) 
            return null;

        string jsonString = File.ReadAllText(SaveFileName);
        Game savedGame = JsonSerializer.Deserialize<Game>(jsonString);
        return savedGame!;
    }
    
    public AttemptResult GetAttemptFeedback(List<string> guess)
    {
        if (guess.Count != CodeLength)
            throw new ArgumentException();
        
        AttemptResult attempt = new AttemptResult();
        attempt.GuessedValues = new List<string>(guess);
        List<string> secretCodeCopy = SecretCode.ToList();
        List<string> guessCopy = guess.ToList();
        
        for (int i = secretCodeCopy.Count - 1; i >= 0; i--)
        {
            if (secretCodeCopy[i] == guessCopy[i])
            {
                attempt.AccurateAnswer++;
                secretCodeCopy.RemoveAt(i);
                guessCopy.RemoveAt(i);
            }
        }
        
        for (int i = 0; i < guessCopy.Count; i++)
        {
            if (secretCodeCopy.Contains(guessCopy[i]))
            {
                attempt.NotAccurateAnswer++;
                secretCodeCopy.Remove(guessCopy[i]);
            }
        }

        if (attempt.AccurateAnswer == CodeLength)
        {
            DeleteSavedGame();
            IsVictory = true;
            IsGameOver = true;
            return attempt;
        }
        
        if (LiesCount > 0)
        {
            TryToLie(attempt);
        }
        
        if (CurrentRound >= AllRounds)
        {
            DeleteSavedGame();
            IsGameOver = true;
            return attempt;
        }
        
        CurrentRound++;
        History.Add(attempt);
        SaveGame();
        return attempt;

        void TryToLie(AttemptResult attempt)
        {
            Random rnd = new Random();

            if (rnd.Next(0, 3) != 1) return;

            bool changeAccurateAnswer = rnd.Next(0, 2) == 0;
            if (changeAccurateAnswer)
            {
                if (attempt.AccurateAnswer == 0) attempt.AccurateAnswer++;
                else if (CodeLength - attempt.AccurateAnswer == 1) attempt.AccurateAnswer--;
                else
                {
                    if (rnd.Next(0, 2) == 0)
                    {
                        attempt.AccurateAnswer--;
                    }
                    else
                    {
                        attempt.AccurateAnswer++;
                    }
                }
            }
            else
            {
                if (attempt.NotAccurateAnswer == 0) attempt.NotAccurateAnswer++;
                else if (attempt.NotAccurateAnswer == CodeLength) attempt.NotAccurateAnswer--;
                else
                {
                    if (rnd.Next(0, 2) == 0)
                    {
                        attempt.NotAccurateAnswer--;
                    }
                    else
                    {
                        attempt.NotAccurateAnswer++;
                    }
                }
            }

            LiesCount--;
        }
    }
}

// Data Transfer Object
public class AttemptResult
{
    public List<string> GuessedValues { get; set; } = new List<string>();
    public int AccurateAnswer { get; set; }
    public int NotAccurateAnswer { get; set; }
}