using ChessMagic.Entity;
using ChessMagic.Util;

namespace ChessMagic.Ledger;

public class GameSnapshot
{
    public char[] Snapshot { get; } = new char[64]; // missing information, the first move indicator is not present in this save.
    public PieceColor PlayerNext { get; }

    public GameSnapshot(PieceColor playerNext)
    {
        PlayerNext = playerNext;
    }

    public char this[int index]
    {
        get => Snapshot[index];
        set => Snapshot[index] = value;
    }

    public char EncodePiece(Piece? piece)
    {
        if(piece == null)
            return 'x';
        string pType = piece.AlgebraicNotation;
        if (string.IsNullOrEmpty(pType))
            pType += 'p';
        
        if (piece.Color == PieceColor.White)
        {
            pType = pType.ToLower();
        }

        return pType[0];
    }

    public Piece? DecodeCharacter(char pieceChar)
    {
        switch (pieceChar)
        {
            case 'x': return null;
            case 'r': return new RookPiece(PieceColor.White);
            case 'R': return new RookPiece(PieceColor.Black);
            case 'n': return new KnightPiece(PieceColor.White);
            case 'N': return new KnightPiece(PieceColor.Black);
            case 'b': return new BishopPiece(PieceColor.White);
            case 'B': return new BishopPiece(PieceColor.Black);
            case 'q': return new QueenPiece(PieceColor.White);
            case 'Q': return new QueenPiece(PieceColor.Black);
            case 'k': return new KingPiece(PieceColor.White);
            case 'K': return new KingPiece(PieceColor.Black);
            case 'p': return new PawnPiece(PieceColor.White);
            case 'P': return new PawnPiece(PieceColor.Black);
            
            default: throw new ArgumentException("Invalid character");
        }
    }
}