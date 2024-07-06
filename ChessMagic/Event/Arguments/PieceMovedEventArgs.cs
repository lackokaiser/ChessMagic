using ChessMagic.Util;

namespace ChessMagic.Event.Arguments;

public class PieceMovedEventArgs : EventArgs
{
    public Position From { get; set; }
    public Position To { get; set; }

    public PieceMovedEventArgs(Position from, Position to)
    {
        From = from;
        To = to;
    }
}