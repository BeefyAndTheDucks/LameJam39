using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HiveMind
{
    private static readonly List<Worker> workers = new();
    private static Queue<Worker> workerQueue = new();

    public static bool TryRequestWorker(out Worker worker)
    {
        return workerQueue.TryDequeue(out worker);
    }

    public static void ReturnWorker(Worker worker)
    {
        workerQueue.Enqueue(worker);
    }

    public static Worker CreateWorker()
    {
        Worker createdWorker = Object.Instantiate(GameManager.Instance.workerPrefab, GameManager.Instance.workerParent);
        workers.Add(createdWorker);
        workerQueue.Enqueue(createdWorker);

        createdWorker.OnDeath += OnDeath;

        return createdWorker;
    }

    private static void OnDeath(object sender, System.EventArgs e)
    {
        Worker worker = sender as Worker;

        RemoveWorker(worker);
        Object.Destroy(worker.gameObject);
    }

    private static void RemoveWorker(Worker worker)
    {
        workerQueue = new Queue<Worker>(workerQueue.Where(s => s != worker));
        workers.Remove(worker);
    }
}
