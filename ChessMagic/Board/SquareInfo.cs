using ChessMagic.Util;

namespace ChessMagic.Board;

public class SquareInfo
{
    public Position PositionUpdated { get; }
    public PieceType PieceType { get; }
    public PieceColor PieceColor { get; }

    private readonly bool _isPieceIn = true;

    public SquareInfo(Position positionUpdated, PieceType pieceType, PieceColor pieceColor)
    {
        this.PositionUpdated = positionUpdated;
        PieceType = pieceType;
        PieceColor = pieceColor;
    }

    public SquareInfo(Position positionUpdated)
    {
        this.PositionUpdated = positionUpdated;
        _isPieceIn = false;
    }

    public bool HasPiece()
    {
        return _isPieceIn;
    }
}