using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

public class QueenPiece : Piece
{
    public QueenPiece(PieceColor color) : base(color)
    {
    }
    
    public override PieceType Type => PieceType.Queen;
    public override Position[] GetPossibleMoves(Position positionFrom, ChessBoard board)
    {
        //horizontal, vertical
        Position upPosition = positionFrom.Offset(0, 1);
        Position downPosition = positionFrom.Offset(0, -1);
        Position rightPosition = positionFrom.Offset(1, 0);
        Position leftPosition = positionFrom.Offset(-1, 0);
        Square? upSquare = board.ConvertToSquare(upPosition);
        Square? downSquare = board.ConvertToSquare(downPosition);
        Square? rightSquare = board.ConvertToSquare(rightPosition);
        Square? leftSquare = board.ConvertToSquare(leftPosition);
        
        // diagonals
        Position upLeftPosition = positionFrom.Offset(1, 1);
        Position downLeftPosition = positionFrom.Offset(1, -1);
        Position downRightPosition = positionFrom.Offset(-1, -1);
        Position upRightPosition = positionFrom.Offset(-1, 1);
        Square? upLeftSquare = board.ConvertToSquare(upLeftPosition);
        Square? downLeftSquare = board.ConvertToSquare(downLeftPosition);
        Square? downRightSquare = board.ConvertToSquare(downRightPosition);
        Square? upRightSquare = board.ConvertToSquare(upRightPosition);
        
        List<Position> moves = new();
        while (upSquare != null && (!upSquare.IsOccupied() || upSquare.Occupant?.Color != Color))
        {
            moves.Add(upPosition);
            upPosition = upPosition.Offset(0, 1);
            if (upSquare.IsOccupied())
                break;
            upSquare = board.ConvertToSquare(upPosition);
        }
        while (downSquare != null && (!downSquare.IsOccupied() || downSquare.Occupant?.Color != Color))
        {
            moves.Add(downPosition);
            downPosition = downPosition.Offset(0, -1);
            if (downSquare.IsOccupied())
                break;
            downSquare = board.ConvertToSquare(downPosition);
        }
        while (rightSquare != null && (!rightSquare.IsOccupied() || rightSquare.Occupant?.Color != Color))
        {
            moves.Add(rightPosition);
            rightPosition = rightPosition.Offset(1, 0);
            if (rightSquare.IsOccupied())
                break;
            rightSquare = board.ConvertToSquare(rightPosition);
        }
        while (leftSquare != null && (!leftSquare.IsOccupied() || leftSquare.Occupant?.Color != Color))
        {
            moves.Add(leftPosition);
            leftPosition = leftPosition.Offset(-1, 0);
            if (leftSquare.IsOccupied())
                break;
            leftSquare = board.ConvertToSquare(leftPosition);
        }
        
        while (upLeftSquare != null && (!upLeftSquare.IsOccupied() || upLeftSquare.Occupant?.Color != Color))
        {
            moves.Add(upLeftPosition);
            upLeftPosition = upLeftPosition.Offset(1, 1);
            if (upLeftSquare.IsOccupied())
                break;
            upLeftSquare = board.ConvertToSquare(upLeftPosition);
        }
        while (downLeftSquare != null && (!downLeftSquare.IsOccupied() || downLeftSquare.Occupant?.Color != Color))
        {
            moves.Add(downLeftPosition);
            downLeftPosition = downLeftPosition.Offset(1, -1);
            if (downLeftSquare.IsOccupied())
                break;
            downLeftSquare = board.ConvertToSquare(downLeftPosition);
        }
        while (downRightSquare != null && (!downRightSquare.IsOccupied() || downRightSquare.Occupant?.Color != Color))
        {
            moves.Add(downRightPosition);
            downRightPosition = downRightPosition.Offset(-1, -1);
            if (downRightSquare.IsOccupied())
                break;
            downRightSquare = board.ConvertToSquare(downRightPosition);
        }
        while (upRightSquare != null && (!upRightSquare.IsOccupied() || upRightSquare.Occupant?.Color != Color))
        {
            moves.Add(upRightPosition);
            upRightPosition = upRightPosition.Offset(-1, 1);
            if (upRightSquare.IsOccupied())
                break;
            upRightSquare = board.ConvertToSquare(upRightPosition);
        }

        return moves.ToArray();
    }

    public override bool CanAttack(Position positionFrom, Position positionTo, ChessBoard board, Position? attackGoThrough, int depth)
    {
        if (attackGoThrough == null)
            return false;
        int xOffset = positionTo.X - positionFrom.X;
        int yOffset = positionTo.Y - positionFrom.Y;
        
        if (Math.Abs(xOffset) != Math.Abs(yOffset) || xOffset != 0 && yOffset != 0)
            return false;
        if (Math.Abs(positionFrom.X - attackGoThrough.X) != Math.Abs(positionFrom.Y - attackGoThrough.Y) 
            || positionTo.X - attackGoThrough.X != 0 && positionTo.Y - attackGoThrough.Y != 0)
            return false;
        if (xOffset == 0 && yOffset == 0)
            return positionFrom.Equals(attackGoThrough);
        
        xOffset = xOffset == 0 ? 0 : xOffset / Math.Abs(xOffset);
        yOffset = yOffset == 0 ? 0 : yOffset / Math.Abs(yOffset);
        
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