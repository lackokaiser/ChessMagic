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

    public List<Position> ThreateningSquares { get; set; } = new ();

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
    /// <param name="pos">The position of the square</param>
    public void GeneratePossibleMoves(ChessBoard board, Position pos)
    {
        _occupant?.GeneratePossibleMoves(board, pos);
        _occupant?.GeneratePossibleSpecialMoves(board, pos);
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