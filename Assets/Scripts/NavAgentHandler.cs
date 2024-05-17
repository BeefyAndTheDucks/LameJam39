using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentHandler : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeedMultiplier = 5f;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        agent.SetDestination(target.position);

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
    }

    public void SetTarget(Vector3 targetPosition)
    {
        target.position = targetPosition;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
