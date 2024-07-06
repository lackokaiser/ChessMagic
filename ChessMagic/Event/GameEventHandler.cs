using ChessMagic.Event.Arguments;
using ChessMagic.Util;

namespace ChessMagic.Event;

public delegate void AlgebraicNotationUpdate(object sender, string notation);
public delegate void NextPlayer(object sender, PieceColor color);
public delegate void PieceMoved(object sender, PieceMovedEventArgs args);
public delegate void GameStateChanged(object sender, GameState state);

public abstract class GameEventHandler
{
    public event NextPlayer? NextPlayerEvent;
    public event AlgebraicNotationUpdate? NotationUpdateEvent;
    public event PieceMoved? PieceMovedEvent;
    public event GameStateChanged? GameStateChangedEvent;

    protected void OnNotationUpdate(string notation)
    {
        NotationUpdateEvent?.Invoke(this, notation);
    }

    protected void OnNextPlayerEvent(PieceColor color)
    {
        NextPlayerEvent?.Invoke(this, color);
    }

    protected void OnPieceMovedEvent(PieceMovedEventArgs args)
    {
        PieceMovedEvent?.Invoke(this, args);
    }

    protected void OnGameStateChangedEvent(GameState state)
    {
        GameStateChangedEvent?.Invoke(this, state);
    }
}