using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

public class KingPiece : Piece
{
    public KingPiece(PieceColor color) : base(color)
    {
    }

    public override PieceType Type => PieceType.King;
    public override Position[] GetPossibleMoves(Position positionFrom, ChessBoard board, PieceColor color)
    {
        List<Position> moves = new();

        if(IsValidPosition(positionFrom.Offset(1, 0), board, out Position outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(1, 1), board, out outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(0, 1), board, out outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(-1, 1), board, out outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(-1, 0), board, out outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(-1, -1), board, out outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(0, -1), board, out outPos))
            moves.Add(outPos);
        if (IsValidPosition(positionFrom.Offset(1, -1), board, out outPos))
            moves.Add(outPos);

        return moves.ToArray();
    }

    private bool IsValidPosition(Position position, ChessBoard board, out Position outPosition)
    {
        outPosition = position;
        Square? square = board.ConvertToSquare(position);
        if (square != null && (!square.IsOccupied() || square.Occupant?.Color != Color))
            return true;
        return false;
    }

    public override int RemoveInvalidMoves(Position positionFrom, ref Position[] moves, ChessBoard board, List<Position> threateningPositions,
        Position kingPosition, List<Position>? kingThreatenedFrom = null)
    {
        int k = moves.Length;
        for (int i = 0; i < k; i++)
        {
            Square? square = board.ConvertToSquare(moves[i]);

            if (square == null)
                throw new ApplicationException("Invalid move");

            if (square.ThreateningSquares.Count > 0)
            {
                k--;
                moves[i] = moves[k];
                i--;
            }
        }

        return k;
    }

    public override SpecialMove[] GetPossibleSpecialMoves(Position positionFrom, ChessBoard board, bool firstMove,
        bool previouslyThreatened, PieceColor color)
    {
        if (!firstMove)
            return [];

        List<SpecialMove> moves = new();

        Position marchPos = positionFrom.Offset(1, 0);
        Square? marchSquare = board.ConvertToSquare(marchPos);


        while (marchSquare != null && !marchSquare.IsOccupied())
        {
            marchPos = marchPos.Offset(1, 0);
            marchSquare = board.ConvertToSquare(marchPos);
        }

        if (marchSquare is { Occupant.Type: PieceType.Rook })
        {
            Piece occupant = marchSquare.Occupant;
            if (occupant.Color == Color && occupant.FirstMove)
            {
                moves.Add(new SpecialMove(marchPos, positionFrom.Offset(2, 0)));
            }
        }

        marchPos = positionFrom.Offset(-1, 0);
        marchSquare = board.ConvertToSquare(marchPos);
        
        while (marchSquare != null && !marchSquare.IsOccupied())
        {
            marchPos = marchPos.Offset(-1, 0);
            marchSquare = board.ConvertToSquare(marchPos);
        }
        
        if (marchSquare is { Occupant.Type: PieceType.Rook })
        {
            Piece occupant = marchSquare.Occupant;
            if (occupant.Color == Color && occupant.FirstMove)
            {
                moves.Add(new SpecialMove(marchPos, positionFrom.Offset(-2, 0)));
            }
        }

        return moves.ToArray();
    }

    public override int RemoveInvalidSpecialMoves(Position positionFrom, ref SpecialMove[] moves, ChessBoard board,
        List<Position> threateningPositions, Position kingPosition, List<Position>? kingThreatenedFrom = null)
    {
        if (threateningPositions.Count > 0)
            return 0; // nothing can be added
        int k = moves.Length;
        for (int i = 0; i < k; i++)
        {
            Position pos = moves[0].PosTo;
            Square? square = board.ConvertToSquare(pos);

            int xOffset = positionFrom.X - pos.X;
            xOffset /= Math.Abs(xOffset);
            bool threatenedSquare = false;
            while (square != null && !pos.Equals(positionFrom) && !threatenedSquare)
            {
                pos = pos.Offset(xOffset, 0);

                if (square.ThreateningSquares.Count > 0)
                    threatenedSquare = true;
                square = board.ConvertToSquare(pos);
            }

            if (threatenedSquare)
            {
                k--;
                moves[i] = moves[k];
                i--;
            }
        }

        return k;
    }
    
    public override void PerformSpecialMove(Position positionFrom, SpecialMove move, ChessBoard board)
    {
        if (move.InvolvingPosition == null)
            throw new ApplicationException("Invalid special move for king");
        board.PerformMove(positionFrom, move.PosTo, Color);
        int xOffset = positionFrom.X - move.PosTo.X;
        xOffset /= Math.Abs(xOffset);
        board.PerformMove(move.InvolvingPosition, move.PosTo.Offset(xOffset, 0), Color);
    }

    public override bool CanAttack(Position positionFrom, Position positionTo, ChessBoard board, Position? attackGoThrough, int depth)
    {
        return Math.Abs(positionFrom.X - positionTo.X) < 1 && Math.Abs(positionFrom.Y - positionTo.Y) < 1;
    }
}