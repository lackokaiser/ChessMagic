using ChessMagic.Board;
using ChessMagic.Util;

namespace ChessMagic.Entity;

/// <summary>
/// Represents one singular piece on the board
/// </summary>
public abstract class Piece
{
    private PieceColor _color;

    protected Position[] _possibleMoves = [];
    protected SpecialMove[] _possibleSpecialMoves = [];
    public bool FirstMove { get; private set; } = true;
    public PieceColor Color => _color;
    public virtual string AlgebraicNotation => "";

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
    /// <param name="position">The position of the piece</param>
    /// <param name="board">The board where the piece is</param>
    public void GeneratePossibleMoves(Position position, ChessBoard board)
    {
        _possibleMoves = GetPossibleMoves(position, board);
    }

    /// <summary>
    /// Generates and stores possible special moves in the <see cref="_possibleSpecialMoves"/> variable
    /// </summary>
    /// <param name="position">The position of the piece</param>
    /// <param name="board">The board where the piece is</param>
    public void GeneratePossibleSpecialMoves(Position position, ChessBoard board)
    {
        _possibleSpecialMoves = GetPossibleSpecialMoves(position, board);
        
    }

    /// <summary>
    /// Generates all possible moves on the board from the given position
    /// </summary>
    /// <param name="positionFrom">The position where the piece should be</param>
    /// <param name="board">The board the piece is on</param>
    /// <returns>A list of each position to move to</returns>
    public abstract Position[] GetPossibleMoves(Position positionFrom, ChessBoard board);

    /// <summary>
    /// Generates all possible special moves on the board from the given position<br/>
    /// Examples of special moves: Castling, en passant, double first move on pawn
    /// </summary>
    /// <param name="positionFrom">The position where the piece should be</param>
    /// <param name="board">The board the piece is on</param>
    /// <returns>A list of each possible special move</returns>
    public virtual SpecialMove[] GetPossibleSpecialMoves(Position positionFrom, ChessBoard board)
    {
        return [];
    }

    public void TrimMoves(int moveCount, int specialMoveCount)
    {
        if (moveCount != PossibleMoves.Length)
        {
            Position[] newMoves = new Position[moveCount];

            for (int i = 0; i < moveCount; i++)
            {
                newMoves[i] = PossibleMoves[i];
            }

            _possibleMoves = newMoves;
        }
        
        if (specialMoveCount != PossibleSpecialMoves.Length)
        {
            SpecialMove[] newMoves = new SpecialMove[specialMoveCount];

            for (int i = 0; i < specialMoveCount; i++)
            {
                newMoves[i] = PossibleSpecialMoves[i];
            }

            _possibleSpecialMoves = newMoves;
        }
    }

    /// <summary>
    /// Removes every invalid move from the moves list in-place
    /// </summary>
    /// <param name="positionFrom">The position where the piece is</param>
    /// <param name="board">The chessboard where the piece is</param>
    /// <param name="threateningPositions">All positions that threaten the piece in question</param>
    /// <param name="kingPosition">The piece's king, if the king is caller, it will receive itself in this</param>
    /// <param name="kingThreatenedFrom">The king's threatening squares</param>
    /// <returns>the new length of the array where the moves are valid</returns>
    public virtual int RemoveInvalidMoves(Position positionFrom, ChessBoard board, List<Position> threateningPositions, 
        Position kingPosition, List<Position>? kingThreatenedFrom = null)
    {
        int k = _possibleMoves.Length;
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
                    Position move = _possibleMoves[i];
                    if (!move.Equals(position) && !kingThreatenedSquare.Occupant.CanAttack(position, kingPosition, board, move, 0))
                    {
                        // king attack cannot be stopped through this move, remove it from valid moves
                        _possibleMoves[i] = _possibleMoves[k - 1];
                        _possibleMoves[k - 1] = move;
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
                    Position move = _possibleMoves[i];
                    if (!threateningSquare.Occupant.CanAttack(threateningPosition, kingPosition, board, move, 1))
                    {
                        _possibleMoves[i] = _possibleMoves[k - 1];
                        _possibleMoves[k - 1] = move;
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
    /// <param name="board">The chessboard where the piece is</param>
    /// <param name="threateningPositions">All positions that threaten the piece in question</param>
    /// <param name="kingPosition">The piece's king, if the king is caller, it will receive itself in this</param>
    /// <param name="kingThreatenedFrom">The king's threatening squares</param>
    /// <returns>the new length of the array where the special moves are valid</returns>
    public virtual int RemoveInvalidSpecialMoves(Position positionFrom, ChessBoard board,
        List<Position> threateningPositions, Position kingPosition, List<Position>? kingThreatenedFrom = null)
    {
        return _possibleSpecialMoves.Length;
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
    /// <returns>The position of the other piece in the interaction</returns>
    public virtual Position PerformSpecialMove(Position positionFrom, SpecialMove move, ChessBoard board)
    {
        return Position.Invalid;
    }

    public virtual string SpecialMoveAlgebraicNotation(Position from, SpecialMove move)
    {
        return "";
    }
}