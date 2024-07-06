using ChessMagic.Board;
using ChessMagic.Event;
using ChessMagic.Event.Arguments;
using ChessMagic.Ledger;
using ChessMagic.Util;

namespace ChessMagic;


public class Game : GameEventHandler
{
    private PieceColor _nextPlayer;
    private ChessBoard _board;
    private MoveHistory _ledger;
    private bool _gameOn = true;

    public Game()
    {
    }

    public async Task InitializeGame()
    {
        await Task.Run(() =>
        {
            _nextPlayer = PieceColor.White;
            _board = new ChessBoard();
            _ledger = new MoveHistory();
            OnGameStateChangedEvent(GameState.Ready);
        });
    }

    private void ToggleNextPlayer()
    {
        if (_nextPlayer == PieceColor.Black) _nextPlayer = PieceColor.White;
        else _nextPlayer = PieceColor.Black;
        
        OnNextPlayerEvent(_nextPlayer);
    }
    
    public Position[] GetPossibleMoves(int x, int y)
    {
        if (!_gameOn)
            return [];
        return _board.GetMovesFor(_nextPlayer, _board.ConvertToPosition(x, y));
    }

    public SpecialMove[] GetSpecialMoves(int x, int y)
    {
        if (!_gameOn)
            return [];
        return _board.GetSpecialMovesFor(_nextPlayer, _board.ConvertToPosition(x, y));
    }

    public void PerformMove(Position moveFrom, Position move)
    {
        if (!_gameOn)
            return;
        string notation = _board.PerformMove(moveFrom, move);
        OnPieceMovedEvent(new PieceMovedEventArgs(moveFrom, move));
        
        PostMoveCalculation(notation);
    }

    private void PlayerWon(PieceColor winningColor)
    {
        OnGameStateChangedEvent(winningColor == PieceColor.Black ? GameState.BlackWin : GameState.WhiteWin);
        OnNotationUpdate(winningColor == PieceColor.Black ? "0-1" : "1-0");
    }

    private void PostMoveCalculation(string moveNotation)
    {
        ToggleNextPlayer();
        _board.CalculateMoves();
        bool isMate = !_board.HasLegalMoves(_nextPlayer) && _board.IsPositionThreatened(_board.FindKing(_nextPlayer));
        bool isStaleMate = !_board.HasLegalMoves(_nextPlayer) &&
                           !_board.IsPositionThreatened(_board.FindKing(_nextPlayer));
        bool isChecked = _board.HasLegalMoves(_nextPlayer) && _board.IsPositionThreatened(_board.FindKing(_nextPlayer));

        if (isMate)
        {
            moveNotation += '#';
            _gameOn = false;
        }
        else if (isStaleMate)
        { 
            OnGameStateChangedEvent(GameState.Stalemate);
            _gameOn = false;
        }
        else if (isChecked)
        {
            moveNotation += '+';
        }
        
        _ledger.PushSnapshot(_board.CreateSnapshot(_nextPlayer), moveNotation);
        OnNotationUpdate(moveNotation);
        if (isMate)
        {
            switch (_nextPlayer)
            {
                case PieceColor.White: PlayerWon(PieceColor.Black);
                    break;
                case PieceColor.Black: PlayerWon(PieceColor.White);
                    break;
            }
        }
        else if(isStaleMate)
            OnNotationUpdate("\u00bd–\u00bd"); // ½–½
    }
}
