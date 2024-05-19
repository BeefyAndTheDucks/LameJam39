using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Workers
{
    private static readonly List<Worker> workers = new();
    private static Queue<Worker> workerQueue = new();

    private static int WorkerLimit = 1;

    public static bool TryRequestWorker(out Worker worker)
    {
        return workerQueue.TryDequeue(out worker);
    }

    public static void ReturnWorker(Worker worker)
    {
        workerQueue.Enqueue(worker);
        worker.Goto(FactionCentreTile.Instance.worldPosition);
    }

    public static Worker CreateWorker(Vector3 position)
    {
        if (workers.Count >= WorkerLimit)
            return null;

        Worker createdWorker = Object.Instantiate(GameManager.Instance.workerPrefab, GameManager.Instance.workerParent);
        createdWorker.transform.position = position;
        createdWorker.Goto(position);
        workers.Add(createdWorker);
        workerQueue.Enqueue(createdWorker);

        createdWorker.OnDeath += OnDeath;

        return createdWorker;
    }

    public static bool HasAvailableWorkers() => workerQueue.Count > 0;
    public static int GetAvailableWorkers() => workerQueue.Count;

    private static void OnDeath(object sender, System.EventArgs e)
    {
        Worker worker = sender as Worker;

        RemoveWorker(worker);
    }

    private static void RemoveWorker(Worker worker)
    {
        workerQueue = new Queue<Worker>(workerQueue.Where(s => s != worker));
        workers.Remove(worker);
        Object.Destroy(worker.gameObject);
    }

    public static void DecreaseWorkerLimit(int by)
    {
        WorkerLimit -= by;
        while (workers.Count > WorkerLimit)
        {
            Worker workerToDestroy = workerQueue.Dequeue();
            RemoveWorker(workerToDestroy);
        }
    }

    public static void IncreaseWorkerLimit(int by)
    {
        WorkerLimit += by;
    }
}
