using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

public class RookPiece : Piece
{
    public RookPiece(PieceColor color) : base(color)
    {
    }
    public override PieceType Type => PieceType.Rook;
    public override Position[] GetPossibleMoves(Position positionFrom, ChessBoard board, PieceColor color)
    {
        Position upPosition = positionFrom.Offset(0, 1);
        Position downPosition = positionFrom.Offset(0, -1);
        Position rightPosition = positionFrom.Offset(1, 0);
        Position leftPosition = positionFrom.Offset(-1, 0);
        Square? upSquare = board.ConvertToSquare(upPosition);
        Square? downSquare = board.ConvertToSquare(downPosition);
        Square? rightSquare = board.ConvertToSquare(rightPosition);
        Square? leftSquare = board.ConvertToSquare(leftPosition);
            
        List<Position> moves = new();
        while (upSquare != null && (!upSquare.IsOccupied() || upSquare.Occupant?.Color != color))
        {
            moves.Add(upPosition);
            upPosition = upPosition.Offset(0, 1);
            if (upSquare.IsOccupied())
                break;
            upSquare = board.ConvertToSquare(upPosition);
        }
        while (downSquare != null && (!downSquare.IsOccupied() || downSquare.Occupant?.Color != color))
        {
            moves.Add(downPosition);
            downPosition = downPosition.Offset(0, -1);
            if (downSquare.IsOccupied())
                break;
            downSquare = board.ConvertToSquare(downPosition);
        }
        while (rightSquare != null && (!rightSquare.IsOccupied() || rightSquare.Occupant?.Color != color))
        {
            moves.Add(rightPosition);
            rightPosition = rightPosition.Offset(1, 0);
            if (rightSquare.IsOccupied())
                break;
            rightSquare = board.ConvertToSquare(rightPosition);
        }
        while (leftSquare != null && (!leftSquare.IsOccupied() || leftSquare.Occupant?.Color != color))
        {
            moves.Add(leftPosition);
            leftPosition = leftPosition.Offset(-1, 0);
            if (leftSquare.IsOccupied())
                break;
            leftSquare = board.ConvertToSquare(leftPosition);
        }

        return moves.ToArray();
    }

    public override bool CanAttack(Position positionFrom, Position positionTo, ChessBoard board, Position? attackGoThrough, int depth)
    {
        if (attackGoThrough == null)
            return false;
        int xOffset = positionTo.X - positionFrom.X;
        int yOffset = positionTo.Y - positionFrom.Y;
        if (xOffset != 0 && yOffset != 0)
            return false;
        if (positionTo.X - attackGoThrough.X != 0 && positionTo.Y - attackGoThrough.Y != 0)
            return false;
        xOffset = xOffset == 0 ? 0 : 1;
        yOffset = yOffset == 0 ? 0 : 1;

        if (xOffset == 0 && yOffset == 0) // The position to go is the position the piece is standing on
            return attackGoThrough.Equals(positionFrom);
        
        Position checkedPosition = positionFrom.Offset(xOffset, yOffset);
        Square? checkedSquare = board.ConvertToSquare(checkedPosition);

        while (checkedSquare != null && !checkedPosition.Equals(positionTo) && depth >= 0)
        {
            checkedPosition = checkedPosition.Offset(xOffset, yOffset);
            if (checkedSquare.IsOccupied())
                depth--;
            checkedSquare = board.ConvertToSquare(checkedPosition);
        }

        return checkedPosition.Equals(positionTo) && depth >= 0;
    }
}