using ChessMagic.Entity;
using ChessMagic.Util;

namespace ChessMagic.Board;

/// <summary>
/// Represents the chess board itself, handles operations, moves, etc..
/// </summary>
public class ChessBoard
{
    private Square[] _squares;

    public ChessBoard()
    {
        
    }

    public Position ConvertToPosition(int x, int y)
    {
        return new Position(x, y);
    }

    public Square? ConvertToSquare(Position position)
    {
        if (position.X < 0 || position.X > 7 || position.Y < 0 || position.Y > 7)
            return null;
        return _squares[position.Y * 8 + position.X];
    }

    /// <summary>
    /// Finds and returns the square occupied by a king with a given color
    /// </summary>
    /// <param name="color">The color given</param>
    /// <exception cref="ApplicationException">If the king is not on the board</exception>
    /// <returns>The square for a king</returns>
    public Square FindKing(PieceColor color)
    {
        foreach (var square in _squares)
        {
            if(square.Occupant == null)
                continue;
            if (square.Occupant.Color == color && square.Occupant.Type == PieceType.King)
                return square;
        }

        throw new ApplicationException("The king should always exist on the board!");
    }

    /// <summary>
    /// Returns every possible move performable from a location for a specific color
    /// </summary>
    /// <param name="color">The color of the piece</param>
    /// <param name="location">The location of the supposed piece</param>
    /// <returns>The array of possible moves</returns>
    public Position[] GetMovesFor(PieceColor color, Position location)
    {
        Square? square = ConvertToSquare(location);
        if (square == null)
            return [];

        if (square.Occupant?.Color == color)
        {
            return square.PossibleMoves;
        }

        return [];
    }

    /// <summary>
    /// Returns every possible special move performable from a location for a specific color
    /// </summary>
    /// <param name="color">The color of the piece</param>
    /// <param name="location">The location of the piece</param>
    /// <returns></returns>
    public SpecialMove[] GetSpecialMovesFor(PieceColor color, Position location)
    {
        Square? square = ConvertToSquare(location);
        if (square == null)
            return [];

        if (square.Occupant?.Color == color)
        {
            return square.PossibleSpecialMoves;
        }

        return [];
    }

    /// <summary>
    /// Determines if the given color has any legal moves or not
    /// </summary>
    /// <param name="color">The color of the player</param>
    /// <returns>True if there are any moves</returns>
    public bool HasLegalMoves(PieceColor color)
    {
        foreach (var square in _squares)
        {
            if (square.Occupant?.Color == color && square.PossibleMoves.Length != 0 || square.PossibleSpecialMoves.Length != 0)
            {
                return true;
            }
        }

        return false;
    }

    public void CalculateMoves()
    {
        foreach (var square in _squares)
        {
            square.ThreateningSquares.Clear();
        }
        
        
    }

    public void ThreatenSquare(Square location, Position threatenFrom)
    {
        Square? threateningSquare = ConvertToSquare(threatenFrom);
        
        if (threateningSquare == null) throw new ArgumentException("Threatening square is invalid");

        location.ThreateningSquares.Add(threatenFrom);
    }

    public Piece? PerformMove(Position from, Position to, PieceColor color)
    {
        Square? fromSquare = ConvertToSquare(from);
        Square? toSquare = ConvertToSquare(to);

        if (fromSquare == null || toSquare == null)
            throw new ArgumentException("Position invalid");

        if (!fromSquare.IsOccupied() || fromSquare.Occupant?.Color != color)
            throw new ArgumentException("Invalid occupant of the square");

        if (toSquare.Occupant?.Color == color)
            throw new ArgumentException("Invalid move for square");

        Piece? takenPiece = toSquare.Occupant;

        toSquare.Occupant = fromSquare.Occupant;

        fromSquare.Occupant = null;

        return takenPiece;
    }

    public Piece? RemovePiece(Position from)
    {
        Square? square = ConvertToSquare(from);
        if (square == null)
            throw new ApplicationException("Invalid square position");

        Piece? piece = square.Occupant;
        square.Occupant = null;
        return piece;
    }
}