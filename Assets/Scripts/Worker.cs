using System;
using UnityEngine;

[RequireComponent(typeof(NavAgentHandler))]
public class Worker : MonoBehaviour
{
    public event EventHandler OnDeath;
    public int Health { get; private set; } = 100;
    public bool IsEnemy = false;

    private NavAgentHandler navAgentHandler;

    private void Awake()
    {
        navAgentHandler = GetComponent<NavAgentHandler>();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void Goto(Vector3 position, Action onArrivedAction = null)
    {
        navAgentHandler.SetTarget(position, onArrivedAction);
    }
}
