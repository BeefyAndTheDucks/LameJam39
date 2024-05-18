using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    [SerializeField] private Transform playerTargetTransform;

    private void Start()
    {
        GameManager.Instance.OnChooseMove += OnChooseMove;
    }

    private void OnChooseMove(Vector2 clickedPosition)
    {
        if (Vector2.Distance(playerTargetTransform.position, clickedPosition) > 2.0f)
            playerTargetTransform.position = clickedPosition;
    }
}
