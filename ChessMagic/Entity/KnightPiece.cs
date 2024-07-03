using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

public class KnightPiece : Piece
{
    public KnightPiece(PieceColor color) : base(color)
    {
    }
    
    public override PieceType Type => PieceType.Knight;
    public override Position[] GetPossibleMoves(Position positionFrom, ChessBoard board)
    {
        List<Position> moves = new();
        CheckPosition(1, 2, ref moves, positionFrom, board, Color);
        CheckPosition(-1, 2, ref moves, positionFrom, board, Color);
        CheckPosition(2, 1, ref moves, positionFrom, board, Color);
        CheckPosition(2, -1, ref moves, positionFrom, board, Color);
        CheckPosition(1, -2, ref moves, positionFrom, board, Color);
        CheckPosition(-1, -2, ref moves, positionFrom, board, Color);
        CheckPosition(-2, 1, ref moves, positionFrom, board, Color);
        CheckPosition(-2, -1, ref moves, positionFrom, board, Color);

        return moves.ToArray();
    }

    private void CheckPosition(int xOffset, int yOffset, ref List<Position> moves, Position from, ChessBoard board, PieceColor color)
    {
        Position position = from.Offset(xOffset, yOffset);
        Square? square = board.ConvertToSquare(position);
        if(square != null && (!square.IsOccupied() || square.Occupant?.Color != color))
            moves.Add(position);
    }

    public override bool CanAttack(Position positionFrom, Position positionTo, ChessBoard board, Position? attackGoThrough, int depth)
    {
        return Math.Abs(positionFrom.X - positionTo.X) == 1 && Math.Abs(positionFrom.Y - positionTo.Y) == 2
               || Math.Abs(positionFrom.X - positionTo.X) == 2 && Math.Abs(positionFrom.Y - positionTo.Y) == 1;
    }
}