using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

public class PawnPiece : Piece
{
    private bool performedDoubleMove = false;
    
    public PawnPiece(PieceColor color) : base(color)
    {
    }

    public override Position[] GetPossibleMoves(Position positionFrom, ChessBoard board, PieceColor color)
    {
        int moveDirection = color == PieceColor.Black ? -1 : 1;

        Position forward = positionFrom.Offset(0, moveDirection);
        Square? sqr = board.ConvertToSquare(forward);
        if (sqr == null)
            throw new ApplicationException("Promotion square already reached");
        
        List<Position> moves = [];
        if(!sqr.IsOccupied())
            moves.Add(forward);

        forward = forward.Offset(1, 0);

        sqr = board.ConvertToSquare(forward);
        if (sqr != null)
        {
            if (sqr.IsOccupied() && sqr.Occupant?.Color != color) 
            {
                moves.Add(forward);
                board.ThreatenSquare(sqr, positionFrom);
            }
        }

        forward = forward.Offset(-2, 0);
        sqr = board.ConvertToSquare(forward);
        if (sqr != null)
        {
            if (sqr.IsOccupied() && sqr.Occupant?.Color != color)
            {
                moves.Add(forward);
                board.ThreatenSquare(sqr, positionFrom);
            }
        }            
        

        return moves.ToArray();
    }

    public override SpecialMove[] GetPossibleSpecialMoves(Position positionFrom, ChessBoard board, bool firstMove,
        bool previouslyThreatened, PieceColor color)
    {
        List<SpecialMove> moves = [];
        int moveDirection = color == PieceColor.Black ? -1 : 1;
        
        // double move
        Position forward = positionFrom.Offset(0, moveDirection * 2);
        Square? squareForward = board.ConvertToSquare(forward.Offset(0, -moveDirection));
        Square? squareForwardTwice = board.ConvertToSquare(forward);
        if(squareForward != null && squareForwardTwice != null && firstMove && !squareForward.IsOccupied() && !squareForwardTwice.IsOccupied())
            moves.Add(new SpecialMove(new Position(-1, -1), forward));
        
        // anti-double move
        Position left = positionFrom.Offset(-1, 0);
        Position right = positionFrom.Offset(1, 0);

        Square? leftSquare = board.ConvertToSquare(left);
        Square? rightSquare = board.ConvertToSquare(right);

        if (leftSquare != null && leftSquare.IsOccupied())
        {
            if (leftSquare.Occupant.Color != color && leftSquare.Occupant is PawnPiece leftPawn)
            {
                if(leftPawn.performedDoubleMove)
                    moves.Add(new SpecialMove(left, left.Offset(0, moveDirection)));
            }
        }
        if (rightSquare != null && rightSquare.IsOccupied())
        {
            if (rightSquare.Occupant.Color != color && rightSquare.Occupant is PawnPiece rightPawn)
            {
                if(rightPawn.performedDoubleMove)
                    moves.Add(new SpecialMove(right, right.Offset(0, moveDirection)));
            }
        }

        return moves.ToArray();
    }

    public override int RemoveInvalidSpecialMoves(Position positionFrom, ref SpecialMove[] moves, ChessBoard board,
        List<Position> threateningPositions, Position kingPosition, List<Position>? kingThreatenedFrom = null)
    {
        int k = moves.Length;
        // handle king checks
        if (kingThreatenedFrom != null)
        {
            foreach (var position in kingThreatenedFrom)
            {
                Square? kingThreatenedSquare = board.ConvertToSquare(position);
                if (kingThreatenedFrom == null)
                    throw new ApplicationException("Invalid king threatening square");
            
                for(int i = 0; i < k; i++)
                {
                    SpecialMove move = moves[i];
                    if (!move.PosTo.Equals(position) && !kingThreatenedSquare.Occupant.CanAttack(position, kingPosition, board, move.PosTo, 0)
                        || kingThreatenedSquare.Occupant.CanAttack(position, kingPosition, board, move.InvolvingPosition, 1))
                    {
                        // king attack cannot be stopped through this move, remove it from valid moves
                        moves[i] = moves[k - 1];
                        moves[k - 1] = move;
                        k--;
                        i--;
                    }
                }    
            }
        }
        
        // pinning
        foreach (var threateningPosition in threateningPositions)
        {
            Square? threateningSquare = board.ConvertToSquare(threateningPosition);
            if (threateningSquare == null || !threateningSquare.IsOccupied())
                throw new ApplicationException("Invalid threaten source");

            if (threateningSquare.Occupant.CanAttack(threateningPosition, kingPosition, board, positionFrom, 1))
            {
                // piece is pinned
                for(int i = 0; i < k; i++)
                {
                    SpecialMove move = moves[i];
                    if (!threateningSquare.Occupant.CanAttack(threateningPosition, kingPosition, board, move.PosTo, 1))
                    {
                        moves[i] = moves[k - 1];
                        moves[k - 1] = move;
                        k--;
                        i--;
                    }
                }
            }
        }

        return k;
    }

    public override bool CanAttack(Position positionFrom, Position positionTo, ChessBoard board, Position? attackGoThrough, int depth)
    {
        int direction = Color == PieceColor.Black ? -1 : 1;
        return positionFrom.X - positionTo.X == direction && positionFrom.Y - positionTo.Y == direction; // diagonal distance is 1
    }

    public override void PerformSpecialMove(Position positionFrom, SpecialMove move, ChessBoard board)
    {
        board.PerformMove(positionFrom, move.PosTo, Color);
        if (move.InvolvingPosition != null) // en-peasant 
        {
            board.RemovePiece(move.InvolvingPosition);
        }
        else
            performedDoubleMove = true;
    }
}