using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentHandler : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private float rotationSpeedMultiplier = 5f;
    [SerializeField] private float destinationDistanceThreshold = 1f;

    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    private bool useLineRenderer;

    private System.Action onArrived;

    private bool hasArrived;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        useLineRenderer = TryGetComponent(out lineRenderer);
    }

    private void Update()
    {
        if (!agent.pathPending && agent.destination != target && agent.destination != target)
        {
            target = new Vector3(target.x, target.y, transform.position.z);
            bool success = agent.SetDestination(target);
            if (!success)
            {
                Debug.LogWarning("Couldn't get path.");
                return;
            }

            hasArrived = false;
        }

        Vector3 velocity = agent.velocity;

        if (velocity.magnitude > 0.1f)
        {
            /*
            Quaternion rotation = Quaternion.LookRotation(velocity);
            Quaternion rot = Quaternion.RotateTowards(transform.rotation, rotation, agent.angularSpeed * Time.deltaTime * rotationSpeedMultiplier);
            transform.rotation = Quaternion.Euler(0, 0, rot.eulerAngles.z);*/

            // Calculate the angle required to face the direction of movement
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

            // Apply the rotation to the agent's transform
            Quaternion desiredRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime * rotationSpeedMultiplier);


        }

        if (agent.remainingDistance < destinationDistanceThreshold && !hasArrived && !agent.pathPending)
        {
            onArrived?.Invoke();
            onArrived = null;
            hasArrived = true;
        }

        if (useLineRenderer)
        {
            lineRenderer.positionCount = agent.path.corners.Length;
            lineRenderer.SetPositions(agent.path.corners);
        }
    }

    public void SetTarget(Vector3 targetPosition, System.Action onArrival = null)
    {
        target = targetPosition;
        onArrived = onArrival;
    }
}
