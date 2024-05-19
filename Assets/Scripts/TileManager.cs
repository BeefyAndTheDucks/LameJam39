using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Grid grid;

    public static TileManager Instance { get; private set; }
    public static bool HasInstance { get; private set; }

    public event System.Action OnUpdate;
    public static event System.Action OnReady;

    private void Awake()
    {
        Instance = this;
        HasInstance = true;

        OnReady?.Invoke();
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }

    public Vector3 CellToWorld(Vector3Int cell) => grid.CellToWorld(cell);
}