using ChessMagic.Entity;
using ChessMagic.Util;

namespace ChessMagic.Board;

/// <summary>
/// Represents one square of the board, the object does not contain its position, allowing free re-arrangements on the board
/// </summary>
public class Square
{
    private Piece? _occupant;

    public Piece? Occupant
    {
        get => _occupant;
        set => _occupant = value;
    }

    public List<Position> ThreateningPositions { get; set; } = new ();

    public Position[] PossibleMoves
    {
        get
        {
            if (_occupant != null) return _occupant.PossibleMoves ?? [];

            return [];
        }
    }

    public SpecialMove[] PossibleSpecialMoves
    {
        get
        {
            if (_occupant != null) return _occupant.PossibleSpecialMoves ?? [];

            return [];
        }
    }

    /// <summary>
    /// Initializes a new instance of the square
    /// </summary>
    /// <param name="occupant">The given piece as it's occupier, if any</param>
    public Square(Piece? occupant = null)
    {
        _occupant = occupant;
    }

    /// <summary>
    /// Calls the occupier, if any, to generate all possible moves for itself
    /// </summary>
    /// <param name="position">The position of the square</param>
    /// <param name="board">The chessboard where the square is</param>
    public void GeneratePossibleMoves(Position position, ChessBoard board)
    {
        _occupant?.GeneratePossibleMoves(position, board);
        _occupant?.GeneratePossibleSpecialMoves(position, board);
    }

    public void FilterPossibleMoves(Position position, ChessBoard board, Position kingPosition, List<Position> kingThreatenedFrom)
    {
        if (_occupant == null)
            return;
        int movesCount =
            _occupant.RemoveInvalidMoves(position, board, ThreateningPositions, kingPosition, kingThreatenedFrom);
        int specialMoveCount = _occupant.RemoveInvalidSpecialMoves(position, board, ThreateningPositions,
            kingPosition, kingThreatenedFrom);

        _occupant.TrimMoves(movesCount, specialMoveCount);
    }

    /// <summary>
    /// Determines if there is an occupier for this square
    /// </summary>
    /// <returns>True if the square has an occupier</returns>
    public bool IsOccupied()
    {
        return _occupant != null;
    }
}