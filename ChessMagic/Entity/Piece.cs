using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

/// <summary>
/// Represents one singular piece on the board
/// </summary>
public abstract class Piece
{
    private PieceColor _color;
    private bool _firstMove = true;
    private bool _previouslyThreatened;

    private Position[] _possibleMoves = [];
    private SpecialMove[] _possibleSpecialMoves = [];

    public PieceColor Color => _color;

    public virtual PieceType Type => PieceType.Pawn;

    public Position[] PossibleMoves => _possibleMoves;

    public SpecialMove[] PossibleSpecialMoves => _possibleSpecialMoves;

    public Piece(PieceColor color)
    {
        _color = color;
    }

    /// <summary>
    /// Generates and stores possible moves in the <see cref="_possibleMoves"/> variable
    /// </summary>
    /// <param name="board">The board where the piece is</param>
    /// <param name="pos">The position of the piece</param>
    public void GeneratePossibleMoves(ChessBoard board, Position pos)
    {
        _possibleMoves = GetPossibleMoves(pos, board, _color);
    }

    /// <summary>
    /// Generates and stores possible special moves in the <see cref="_possibleSpecialMoves"/> variable
    /// </summary>
    /// <param name="board">The board where the piece is</param>
    /// <param name="pos">The position of the piece</param>
    public void GeneratePossibleSpecialMoves(ChessBoard board, Position pos)
    {
        _possibleSpecialMoves = GetPossibleSpecialMoves(pos, board, _firstMove, _previouslyThreatened, _color);
        
    }
    
    /// <summary>
    /// Generates all possible moves on the board from the given position
    /// </summary>
    /// <param name="positionFrom">The position where the piece should be</param>
    /// <param name="board">The board the piece is on</param>
    /// <param name="color">The color of the piece</param>
    /// <returns>A list of each position to move to</returns>
    public abstract Position[] GetPossibleMoves(Position positionFrom, ChessBoard board, PieceColor color);

    /// <summary>
    /// Generates all possible special moves on the board from the given position<br/>
    /// Examples of special moves: Castling, en passant, double first move on pawn
    /// </summary>
    /// <param name="positionFrom">The position where the piece should be</param>
    /// <param name="board">The board the piece is on</param>
    /// <param name="firstMove">Indicates if this is the first move of the piece</param>
    /// <param name="previouslyThreatened">Indicates if the piece has been threatened</param>
    /// <param name="color">The color of the piece</param>
    /// <returns>A list of each possible special move</returns>
    public virtual SpecialMove[] GetPossibleSpecialMoves(Position positionFrom, ChessBoard board, bool firstMove,
        bool previouslyThreatened, PieceColor color)
    {
        return [];
    }

    /// <summary>
    /// Removes every invalid move from the moves list in-place
    /// </summary>
    /// <param name="positionFrom">The position where the piece is</param>
    /// <param name="moves">The move list</param>
    /// <param name="board">The chessboard where the piece is</param>
    /// <param name="threateningPositions">All positions that threaten the piece in question</param>
    /// <param name="kingPosition">The piece's king, if the king is caller, it will receive itself in this</param>
    /// <param name="kingThreatenedFrom">The king's threatening squares</param>
    /// <returns>the new length of the array where the moves are valid</returns>
    public virtual int RemoveInvalidMoves(Position positionFrom, ref Position[] moves, ChessBoard board,
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
                    Position move = moves[i];
                    if (!move.Equals(position) && !kingThreatenedSquare.Occupant.CanAttack(position, kingPosition, board, move, 0))
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
        
        
        // handle piece pinning
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
                    Position move = moves[i];
                    if (!threateningSquare.Occupant.CanAttack(threateningPosition, kingPosition, board, move, 1))
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

    /// <summary>
    /// Removes every invalid special move from the moves list in-place
    /// </summary>
    /// <param name="positionFrom">The position where the piece is</param>
    /// <param name="moves">The special move list</param>
    /// <param name="board">The chessboard where the piece is</param>
    /// <param name="threateningPositions">All positions that threaten the piece in question</param>
    /// <param name="kingPosition">The piece's king, if the king is caller, it will receive itself in this</param>
    /// <param name="kingThreatenedFrom">The king's threatening squares</param>
    /// <returns>the new length of the array where the special moves are valid</returns>
    public virtual int RemoveInvalidSpecialMoves(Position positionFrom, ref SpecialMove[] moves, ChessBoard board,
        List<Position> threateningPositions, Position kingPosition, List<Position>? kingThreatenedFrom = null)
    {
        return moves.Length;
    }


    /// <summary>
    /// Determines if the piece can attack a position, given a depth
    /// </summary>
    /// <param name="positionFrom">The position from where the piece may attack</param>
    /// <param name="positionTo">The position the piece tries to attack</param>
    /// <param name="board">The chessboard the piece is on</param>
    /// <param name="attackGoThrough">The attack must go though this position to be valid. if the position is set to null,
    /// the function will most likely return false.
    /// If you intend to not set a go-through position, set this to the starting position</param>
    /// <param name="depth">The depth of the attack, indicates how many pieces can the attack cut through</param>
    /// <returns></returns>
    public abstract bool CanAttack(Position positionFrom, Position positionTo, ChessBoard board, Position? attackGoThrough, int depth);

    /// <summary>
    /// Performs the special move
    /// </summary>
    /// <param name="positionFrom">The position where the square should be</param>
    /// <param name="move">The move to perform</param>
    /// <param name="board">The board where the piece should be</param>
    public virtual void PerformSpecialMove(Position positionFrom, SpecialMove move, ChessBoard board)
    {
    }
}