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
    public int ColorsCount { get; private set; }

    [JsonInclude]
    public int CurrentRound { get; private set; } = 1;

    [JsonInclude]
    public bool IsGameOver { get; private set; }

    [JsonInclude]
    public bool IsSurrendered { get; private set; }

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
    
    private static readonly List<string> _allColors = ["r", "y", "g", "b", "m", "c", "w", "dg"];
    
    private const string SaveFileName = "savedgamedata.json";

    public Game(int rounds = 9, int codeLength = 4, int colorsCount = 6, int liesCount = 0)
    {
        AllRounds = rounds;
        CodeLength = codeLength;
        ColorsCount = colorsCount;
        LiesCount = liesCount;
        InitialLiesCount = liesCount;

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
            int x = rnd.Next(0, ColorsCount);
            SecretCode.Add(_allColors[x]);
        }
    }
    
    public List<string> GetSecretCode()
    {
        return SecretCode;
    }

    public List<string> GetAvailableColors(Game game)
    {
        List<string> availableColors = new List<string>();
        for (int i = 0; i < game.ColorsCount; i++)
        {
            availableColors.Add(_allColors[i]);
        }
        return availableColors;
    }

    public void Surrender()
    {
        IsSurrendered = true;
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
        attempt.GuessedColors = new List<string>(guess);
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
    public List<string> GuessedColors { get; set; } = new List<string>();
    public int AccurateAnswer { get; set; }
    public int NotAccurateAnswer { get; set; }
}