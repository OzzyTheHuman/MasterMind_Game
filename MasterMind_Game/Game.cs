using System.Security.AccessControl;

namespace MasterMind_Game;

public class Game
{
    public int Rounds { get; private set; }
    public int CodeLength { get; private set; }
    public int ColorsCount { get; private set; }

    private List<string> _secretCode = new List<string>();
    private static readonly List<string> _allColors = ["red", "yellow", "green", "blue", "magenta", "cyan"];

    public Game(int rounds = 9, int codeLength = 4, int colorsCount = 6)
    {
        Rounds = rounds;
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
    
    public string GetSecretCodeAsString()
    {
        return string.Join(", ", _secretCode);
    }
}