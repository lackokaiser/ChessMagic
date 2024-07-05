namespace ChessMagic.Util;

/// <summary>
/// Contains a position determined in x,y coordinates
/// </summary>
/// <param name="x">The x coordinate</param>
/// <param name="y">The y coordinate</param>
public class Position(int x, int y)
{
    public int X => x;

    public int Y => y;

    public Position Offset(int xOffset, int yOffset)
    {
        return new Position(X + xOffset, Y + yOffset);
    }
    
    public string AlgebraicNotation()
    {
        char xPos = (char)(97 + x);
        string res = xPos.ToString() + (y + 1);

        return res;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Position pos)
        {
            return pos.X == X && pos.Y == Y;
        }

        return base.Equals(obj);
    }
}