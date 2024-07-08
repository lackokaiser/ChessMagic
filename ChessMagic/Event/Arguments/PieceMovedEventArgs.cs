using ChessMagic.Util;

namespace ChessMagic.Event.Arguments;

public class PieceMovedEventArgs : EventArgs
{
    public Position From { get; }
    public Position To { get; }

    public PieceMovedEventArgs(Position from, Position to)
    {
        From = from;
        To = to;
    }
}