using UnityEngine;
using UnityEngine.Tilemaps;

public class AIAction
{
    public int RequiredWorkers { get; internal set; }
    public System.Action<Worker[]> Behaviour { get; internal set; }

    public void Execute()
    {
        if (!EnemyWorkers.HasAvailableWorkers(RequiredWorkers))
        {
            Debug.LogWarning("Insufficient Workers!");
            return;
        }

        Worker[] workers = new Worker[RequiredWorkers];

        for (int i = 0; i < RequiredWorkers; i++)
            EnemyWorkers.TryRequestWorker(out workers[i]);

        Behaviour?.Invoke(workers);
    }

    public class Builder
    {
        private AIAction action;

        public Builder() => action = new AIAction();

        public Builder WithRequiredWorkers(int requiredWorkers)
        {
            action.RequiredWorkers = requiredWorkers;
            return this;
        }

        public Builder WithBehaviour(System.Action<Worker[]> behavior)
        {
            action.Behaviour = behavior;
            return this;
        }

        public AIAction Build()
        {
            return action;
        }
    }

    public static class Factory
    {
        public static AIAction CreateAttackAction(int workersToSend)
        {
            return new Builder()
                .WithRequiredWorkers(workersToSend)
                .WithBehaviour((workers) =>
                {
                    foreach (Worker worker in workers)
                        // Goto Faction Centre                                      // Attack Faction Centre                  // Send worker back.
                        worker.Goto(FactionCentreTile.Instance.worldPosition, () => worker.Attack(FactionCentreTile.Instance, () => EnemyWorkers.ReturnWorker(worker)));
                })
                .Build();
        }

        public static AIAction CreateConstructionAction(Tile tile)
        {
            return new Builder()
                .WithRequiredWorkers(1)
                .WithBehaviour((workers) =>
                {
                    // Get Position
                    if (!PositionPicker.TryPickPosition(out Vector3Int gridPosition))
                    {
                        Debug.LogWarning("Failed to find a valid position");
                        return;
                    }

                    Vector3 worldPosition = GameManager.Instance.grid.CellToWorld(gridPosition);

                    workers[0].Goto(worldPosition, () =>
                    {
                        GameManager.Instance.enemyTilemap.SetTile(gridPosition, tile);
                        EnemyWorkers.ReturnWorker(workers[0]);
                    });
                })
                .Build();
        }
    }
}