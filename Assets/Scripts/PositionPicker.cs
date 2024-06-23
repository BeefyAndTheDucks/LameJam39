using UnityEngine;

public static class PositionPicker
{
    private const int k_MaxArea = 10;

    private static int currentXOffset = -k_MaxArea - 2;
    private static int currentYOffset = -k_MaxArea;

    public static bool TryPickPosition(out Vector3Int gridPosition)
    {
        currentXOffset += 2;
        if (currentXOffset >= k_MaxArea)
        {
            currentYOffset += 2;
            currentXOffset = -k_MaxArea;
        }

        if (currentYOffset >= k_MaxArea)
        {
            Debug.LogError("No more area.");

            gridPosition = FactionCentreTile.EnemyInstance.gridPosition;

            return false;
        }

        // Avoid already placed buildings
        if (currentXOffset == 0 && currentYOffset == 0)
            currentXOffset = 4;

        gridPosition = FactionCentreTile.EnemyInstance.gridPosition + new Vector3Int(currentXOffset, currentYOffset, 0);

        return true;
    }
}