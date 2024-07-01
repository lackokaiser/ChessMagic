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

    public Position Offset(int x, int y)
    {
        return new Position(X + x, Y + y);
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