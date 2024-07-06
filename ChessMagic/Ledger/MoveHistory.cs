namespace ChessMagic.Ledger;

public class MoveHistory
{
    private Stack<GameSnapshot> _snapshots;
    private Stack<string> _notations;

    public void PushSnapshot(GameSnapshot snapshot, string notation)
    {
        _snapshots.Push(snapshot);
        _notations.Push(notation);
    }

    public GameSnapshot? Rollback(uint times = 1)
    {
        GameSnapshot? snapshot = null;
        while (times > 0 && _snapshots.TryPop(out snapshot))
        {
            _notations.Pop();
            times--;
        }

        return snapshot;
    }
}