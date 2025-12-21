using System.Security.AccessControl;
using System.Text;

namespace MasterMind_Game;

public class Game
{
    public int Rounds { get; private set; }
    public int CodeLength { get; private set; }
    public int ColorsCount { get; private set; }

    private List<string> _secretCode = new List<string>();
    private static readonly List<string> _allColors = ["r", "y", "g", "b", "m", "c"];

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
    
    public List<string> GetSecretCode()
    {
        return _secretCode;
    }

    // TODO: REFACTOR - PARSING TO STRING SHOULD BE IN UI
    // TODO: REFACTOR - NAME METHOD CHANGE, START WITH "GET"
    // TODO: REFACTOR - CHANGE ARGUMENT AND RETURN TYPE TO LIST
    // TODO: CURRENT METHOD FOR CHECKING FOR ANSWERS IS WRONG
    public string CheckForAnswers(string answer)
    {
        string result = "";
        for (int i = 0; i < answer.Length; i++)
        {
            if (_secretCode[i] == answer[i].ToString())
            {
                result += "[+]";
            }
            else if(_secretCode.Contains(answer[i].ToString()))
            {
                result += "[-]";
            }
            else
            {
                result += "[ ]";
            }
        }

        return result;
    }
}