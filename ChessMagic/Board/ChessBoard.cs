using ChessMagic.Entity;
using ChessMagic.Ledger;
using ChessMagic.Util;

namespace ChessMagic.Board;

/// <summary>
/// Represents the chess board itself, handles operations, moves, etc..
/// </summary>
public class ChessBoard
{
    private readonly Square[] _squares;

    public ChessBoard()
    {
        _squares = new Square[64];
        InitializeBoard();
        
        CalculateMoves();
    }

    public void InitializeBoard()
    {
        for (int i = 0; i < _squares.Length; i++)
        {
            Piece? p = null;
            if (i is >= 8 and < 16 or >= 48 and < 56)
                p = new PawnPiece(i >= 16 ? PieceColor.Black : PieceColor.White);
            else if (i is 0 or 7 or 56 or 63)
                p = new RookPiece(i >= 9 ? PieceColor.Black : PieceColor.White);
            else if (i is 1 or 6 or 57 or 62)
                p = new KnightPiece(i >= 9 ? PieceColor.Black : PieceColor.White);
            else if (i is 2 or 5 or 58 or 61)
                p = new BishopPiece(i >= 9 ? PieceColor.Black : PieceColor.White);
            else if (i is 3 or 59)
                p = new QueenPiece(i >= 9 ? PieceColor.Black : PieceColor.White);
            else if (i is 4 or 60)
                p = new KingPiece(i >= 9 ? PieceColor.Black : PieceColor.White);
            _squares[i] = new Square(p);
        }
    }

    public void InitializeBoard(GameSnapshot snapshotFor)
    {
        
    }

    /// <summary>
    /// Creates a snapshot of the current game. Aims to be as memory sparing as possible
    /// </summary>
    /// <param name="nextPlayer">The player's color who will move next</param>
    /// <returns>The constructed snapshot of the board</returns>
    /// <exception cref="ApplicationException">If one piece does not exist when it should</exception>
    public GameSnapshot CreateSnapshot(PieceColor nextPlayer)
    {
        GameSnapshot snapshot = new GameSnapshot(nextPlayer);
        for (int i = 0; i < _squares.Length; i++)
        {
            if(!_squares[i].IsOccupied())
                snapshot[i] = 'x';
            else
            {
                Piece? p = _squares[i].Occupant;
                if (p == null)
                    throw new ApplicationException("Wrong square");

                string pType = p.AlgebraicNotation;

                if (p.Color == PieceColor.White)
                {
                    pType = pType.ToLower();
                }

                snapshot[i] = pType[0];
            }
        }

        return snapshot;
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

    public bool IsPositionThreatened(Position position)
    {
        Square? square = ConvertToSquare(position);

        if (square == null)
            throw new ApplicationException("Wrong position");

        return square.ThreateningPositions.Count > 0;
    }

    /// <summary>
    /// Finds and returns the square occupied by a king with a given color
    /// </summary>
    /// <param name="color">The color given</param>
    /// <exception cref="ApplicationException">If the king is not on the board or one of the squares are invalid</exception>
    /// <returns>The position of the king</returns>
    public Position FindKing(PieceColor color)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Position pos = new Position(x, y);
                Square? square = ConvertToSquare(pos);
                if (square == null)
                    throw new ApplicationException("invalid square");
                if(!square.IsOccupied())
                    continue;
                if (square.Occupant!.Color == color && square.Occupant.Type == PieceType.King)
                    return pos;
            }
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
        List<(Position, Square)> foundPoses = new();
        
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Position pos = ConvertToPosition(x, y);
                Square? square = ConvertToSquare(pos);
                
                square?.ThreateningPositions.Clear();
                if (square == null)
                    throw new ApplicationException("Invalid square");
                square.GeneratePossibleMoves(pos, this);

                if (square.IsOccupied())
                {
                    foreach (var move in square.Occupant!.PossibleMoves)
                    {
                        Square? sqr = ConvertToSquare(move);
                        if (sqr == null)
                            throw new ApplicationException("Invalid square");
                        ThreatenSquare(sqr, pos);
                    }
                    foundPoses.Add((pos, square));
                }
                    
            }
        }

        Position whiteKingPosition = FindKing(PieceColor.White);
        Position blackKingPosition = FindKing(PieceColor.Black);
        Square? whiteKingSquare = ConvertToSquare(whiteKingPosition);
        Square? blackKingSquare = ConvertToSquare(blackKingPosition);

        if (whiteKingSquare == null || blackKingSquare == null)
            throw new ApplicationException("Invalid king square");
        
        foreach (var foundPose in foundPoses)
        {
            bool isWhite = foundPose.Item2.Occupant?.Color == PieceColor.White;
            foundPose.Item2.FilterPossibleMoves(foundPose.Item1, this, 
                isWhite ? whiteKingPosition : blackKingPosition, isWhite ? whiteKingSquare.ThreateningPositions : blackKingSquare.ThreateningPositions);
        }
    }

    public void ThreatenSquare(Square location, Position threatenFrom)
    {
        Square? threateningSquare = ConvertToSquare(threatenFrom);
        
        if (threateningSquare == null) throw new ArgumentException("Threatening square is invalid");

        location.ThreateningPositions.Add(threatenFrom);
    }

    public (string, Position) PerformMove(Position from, SpecialMove move)
    {
        Square? fromSquare = ConvertToSquare(from);

        if (fromSquare == null || !fromSquare.IsOccupied())
            throw new ApplicationException("No piece was found");
        
        Position involvingTarget = fromSquare.Occupant!.PerformSpecialMove(from, move, this);

        return (fromSquare.Occupant.SpecialMoveAlgebraicNotation(from, move), involvingTarget);
    }

    /// <summary>
    /// Transforms a piece to another location
    /// </summary>
    /// <param name="from">The location of the piece to move</param>
    /// <param name="to">The location the piece must move to</param>
    /// <returns>The algebraic notation of the move</returns>
    /// <exception cref="ArgumentException">If there are no piece at the source location</exception>
    public string PerformMove(Position from, Position to)
    {
        var piece = RemovePiece(from);
        
        if (piece == null)
            throw new ArgumentException("No piece was found from removing position");

        PlacePiece(to, piece);
        
        
        return piece.AlgebraicNotation + to.AlgebraicNotation();
    }

    /// <summary>
    /// Places a piece at a specific location
    /// </summary>
    /// <param name="at">The location to place the piece</param>
    /// <param name="pieceToPlace">The piece to place</param>
    /// <returns>The piece that was replaced</returns>
    /// <exception cref="ApplicationException">If the position is invalid</exception>
    public Piece? PlacePiece(Position at, Piece pieceToPlace)
    {
        Piece? removed = RemovePiece(at);

        Square? atSquare = ConvertToSquare(at);

        if (atSquare == null)
            throw new ApplicationException("Invalid square position");

        atSquare.Occupant = pieceToPlace;

        return removed;
    }

    /// <summary>
    /// Removes a piece from the board at a specific location
    /// </summary>
    /// <param name="from">The location to remove</param>
    /// <returns>The piece that was removed</returns>
    /// <exception cref="ApplicationException">If the position is invalid</exception>
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