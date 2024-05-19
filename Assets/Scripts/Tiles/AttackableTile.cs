using UnityEngine.Tilemaps;

public abstract class AttackableTile : Tile
{
    public abstract void TakeDamage(int damage);
}