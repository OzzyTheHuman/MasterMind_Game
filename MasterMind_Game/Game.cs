namespace MasterMind_Game;

public class Game
{
    public int Rounds { get; private set; }
    public int CodeLength { get; private set; }
    public int ColorsCount { get; private set; }

    private List<int> _secretCode;
    private static readonly List<string> _allColors = ["red", "yellow", "green", "blue", "magenta", "cyan"];

    public Game(int rounds = 9, int codeLength = 4, int colorsCount = 6)
    {
        Rounds = rounds;
        CodeLength = codeLength;
        ColorsCount = colorsCount;
    }
}