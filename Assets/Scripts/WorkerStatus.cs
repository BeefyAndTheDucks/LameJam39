using TMPro;
using UnityEngine;

public class WorkerStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private void Update()
    {
        statusText.text = $"{Workers.GetWorkerCount()} / {Workers.GetWorkerLimit()} ({Workers.GetAvailableWorkers()} available)";
    }
}
