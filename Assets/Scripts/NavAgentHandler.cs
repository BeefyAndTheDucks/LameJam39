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
    private Animator animator;

    private bool useLineRenderer;
    private bool useAnimator;

    private System.Action onArrived;

    private bool hasArrived;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        useLineRenderer = TryGetComponent(out lineRenderer);
        useAnimator = TryGetComponent(out animator);
    }

    private void Update()
    {
        UpdatePath();
        UpdateRotation();
        UpdateArrival();
        UpdateLine();
        UpdateAnimator();
    }

    private void UpdatePath()
    {
        // Check there isn't already a path being calculated and the agents target isn't already the requested target.
        if (!agent.pathPending && agent.destination != target)
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
    }

    private void UpdateArrival()
    {
        // Check that we have arrived, havent already registered that we have and that we arent calculating a new path.
        if (agent.remainingDistance < destinationDistanceThreshold && !hasArrived && !agent.pathPending)
        {
            onArrived?.Invoke();
            onArrived = null;
            hasArrived = true;
        }
    }

    private void UpdateLine()
    {
        // Make sure we want to use a LineRenderer
        if (useLineRenderer)
        {
            lineRenderer.positionCount = agent.path.corners.Length;
            lineRenderer.SetPositions(agent.path.corners);
        }
    }

    private void UpdateAnimator()
    {
        // Make sure we want to use an Animator
        if (useAnimator)
            animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }

    private void UpdateRotation()
    {
        // Make sure we are walking
        if (agent.velocity.magnitude > 0.1f)
        {
            // Calculate the angle required to face the direction of movement
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;

            // Apply the rotation to the agent's transform
            Quaternion desiredRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, agent.angularSpeed * Time.deltaTime * rotationSpeedMultiplier);
        }
    }

    public void SetTarget(Vector3 targetPosition, System.Action onArrival = null)
    {
        target = targetPosition;
        onArrived = onArrival;
    }
}
