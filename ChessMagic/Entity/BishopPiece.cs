using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

public class BishopPiece : Piece
{
    public BishopPiece(PieceColor color) : base(color)
    {
    }
    
    public override PieceType Type => PieceType.Bishop;
    public override Position[] GetPossibleMoves(Position positionFrom, ChessBoard board, PieceColor color)
    {
        Position upPosition = positionFrom.Offset(1, 1);
        Position downPosition = positionFrom.Offset(1, -1);
        Position rightPosition = positionFrom.Offset(-1, -1);
        Position leftPosition = positionFrom.Offset(-1, 1);
        Square? upSquare = board.ConvertToSquare(upPosition);
        Square? downSquare = board.ConvertToSquare(downPosition);
        Square? rightSquare = board.ConvertToSquare(rightPosition);
        Square? leftSquare = board.ConvertToSquare(leftPosition);
        
        List<Position> moves = new();
        while (upSquare != null && (!upSquare.IsOccupied() || upSquare.Occupant?.Color != color))
        {
            moves.Add(upPosition);
            upPosition = upPosition.Offset(1, 1);
            if (upSquare.IsOccupied())
                break;
            upSquare = board.ConvertToSquare(upPosition);
        }
        while (downSquare != null && (!downSquare.IsOccupied() || downSquare.Occupant?.Color != color))
        {
            moves.Add(downPosition);
            downPosition = downPosition.Offset(1, -1);
            if (downSquare.IsOccupied())
                break;
            downSquare = board.ConvertToSquare(downPosition);
        }
        while (rightSquare != null && (!rightSquare.IsOccupied() || rightSquare.Occupant?.Color != color))
        {
            moves.Add(rightPosition);
            rightPosition = rightPosition.Offset(-1, -1);
            if (rightSquare.IsOccupied())
                break;
            rightSquare = board.ConvertToSquare(rightPosition);
        }
        while (leftSquare != null && (!leftSquare.IsOccupied() || leftSquare.Occupant?.Color != color))
        {
            moves.Add(leftPosition);
            leftPosition = leftPosition.Offset(-1, 1);
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

        if (Math.Abs(xOffset) != Math.Abs(yOffset))
            return false;
        if (Math.Abs(positionFrom.X - attackGoThrough.X) != Math.Abs(positionFrom.Y - attackGoThrough.Y))
            return false;
        if (xOffset == 0 && yOffset == 0)
            return positionFrom.Equals(attackGoThrough);
        
        xOffset /= Math.Abs(xOffset);
        yOffset /= Math.Abs(yOffset);

        Position checkedPosition = positionFrom.Offset(xOffset, yOffset);
        Square? checkedSquare = board.ConvertToSquare(checkedPosition);

        bool goneThroughPosition = positionFrom.Equals(attackGoThrough);
        while (checkedSquare != null && !checkedPosition.Equals(positionTo) && depth >= 0)
        {
            if (checkedPosition.Equals(attackGoThrough))
                goneThroughPosition = true;
            checkedPosition = checkedPosition.Offset(xOffset, yOffset);
            if (checkedSquare.IsOccupied())
                depth--;
            checkedSquare = board.ConvertToSquare(checkedPosition);
        }

        return goneThroughPosition && checkedPosition.Equals(positionTo) && depth >= 0;
    }
}