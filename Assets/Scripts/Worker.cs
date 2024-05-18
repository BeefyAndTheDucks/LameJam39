using System;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public event EventHandler OnDeath;
    public int Health { get; private set; } = 100;

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            OnDeath?.Invoke(this, EventArgs.Empty);
    }
}
