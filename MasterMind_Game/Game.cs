using System.Security.AccessControl;
using System.Text;

namespace MasterMind_Game;

public class Game
{
    public int AllRounds { get; private set; }
    public int CodeLength { get; private set; }
    public int ColorsCount { get; private set; }
    public int CurrentRound { get; private set; } = 1;
    public bool IsGameOver { get; private set; }
    public bool IsSurrendered { get; private set; }
    
    private List<string> _secretCode = new List<string>();
    private static readonly List<string> _allColors = ["r", "y", "g", "b", "m", "c"];

    public Game(int rounds = 9, int codeLength = 4, int colorsCount = 6)
    {
        AllRounds = rounds;
        CodeLength = codeLength;
        ColorsCount = colorsCount;

        GenerateSecretCode();
    }
    
    private void GenerateSecretCode()
    {
        Random rnd = new Random();
        for (int i = 0; i < CodeLength; i++)
        {
            int x = rnd.Next(0, ColorsCount);
            _secretCode.Add(_allColors[x]);
        }
    }
    
    public List<string> GetSecretCode()
    {
        return _secretCode;
    }

    public List<string> GetAvailableColors()
    {
        return _allColors;
    }

    public void Surrender()
    {
        IsSurrendered = true;
        IsGameOver = true;
    }
    
    public AttemptResult GetAttemptFeedback(List<string> guess)
    {
        AttemptResult attempt = new AttemptResult();

        if (guess.Count != CodeLength)
            throw new ArgumentException();
        
        List<string> secretCodeCopy = _secretCode.ToList();
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
        
        for (int i = 0; i < secretCodeCopy.Count; i++)
        {
            if (secretCodeCopy.Contains(guessCopy[i]))
            {
                attempt.NotAccurateAnswer++;
                secretCodeCopy.Remove(guessCopy[i]);
            }
        }
        
        if (attempt.AccurateAnswer == CodeLength)
        {
            attempt.IsVictory = true;
            IsGameOver = true;
        }

        if (CurrentRound == AllRounds)
        {
            IsGameOver = true;
        }

        CurrentRound++;
        return attempt;
    }
}

// Data Transfer Object
public class AttemptResult
{
    public int AccurateAnswer { get; set; }
    public int NotAccurateAnswer { get; set; }
    public bool IsVictory { get; set; }
    /*
    public AttemptResult(int accurateAnswer, int notAccurateAnswer, bool isVictory)
    {
        AccurateAnswer = accurateAnswer;
        NotAccurateAnswer = notAccurateAnswer;
        IsVictory = isVictory;
    }
    */
}