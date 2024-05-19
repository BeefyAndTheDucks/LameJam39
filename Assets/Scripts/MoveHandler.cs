using UnityEngine;

[RequireComponent(typeof(NavAgentHandler))]
public class MoveHandler : MonoBehaviour
{
    private NavAgentHandler navAgentHandler;

    private void Start()
    {
        GameManager.Instance.OnChooseMove += OnChooseMove;
    }

    private void Awake()
    {
        navAgentHandler = GetComponent<NavAgentHandler>();
    }

    private void OnChooseMove(Vector2 clickedPosition)
    {
        navAgentHandler.SetTarget(clickedPosition);
    }
}
