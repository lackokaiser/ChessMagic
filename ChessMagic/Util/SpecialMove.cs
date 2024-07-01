namespace ChessMagic.Util;

public class SpecialMove(Position involvingPosition, Position posTo)
{
    public Position? InvolvingPosition => involvingPosition;

    public Position PosTo => posTo;
}