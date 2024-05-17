using System;
using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    [SerializeField] private Transform playerTargetTransform;

    private void Start()
    {
        GameManager.Instance.OnLeftClick += OnLeftClick;
    }

    private void OnLeftClick()
    {
        playerTargetTransform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
