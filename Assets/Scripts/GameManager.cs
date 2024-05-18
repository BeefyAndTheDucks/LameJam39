using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform player;

    public event Action<Vector2> OnChooseMove;
    public event Action<Vector2> OnShowContextMenu;

    private Camera _cachedCameraMain;

    private void Awake()
    {
        Instance = this;
        _cachedCameraMain = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickedPosition = _cachedCameraMain.ScreenToWorldPoint(Input.mousePosition);
            Action<Vector2> evnt = (Vector2.Distance(player.position, clickedPosition) > 2.0f) ? OnChooseMove : OnShowContextMenu;
            evnt?.Invoke(clickedPosition);
        }
    }
}
