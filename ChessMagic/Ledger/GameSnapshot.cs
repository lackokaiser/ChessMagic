using ChessMagic.Util;

namespace ChessMagic.Ledger;

public class GameSnapshot
{
    public char[] Snapshot { get; } = new char[64];
    public PieceColor PlayerNext { get; }

    public GameSnapshot(PieceColor playerNext)
    {
        PlayerNext = playerNext;
    }

    public char this[int index]
    {
        get => Snapshot[index];
        set => Snapshot[index] = value;
    }
}