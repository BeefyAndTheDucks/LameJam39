using System;
using UnityEngine;

[RequireComponent(typeof(NavAgentHandler))]
public class Worker : MonoBehaviour
{
    public event EventHandler OnDeath;
    public int Health { get; private set; } = 100;
    public bool IsEnemy = false;

    [SerializeField] private float attackSpeed = 1.0f;
    private float attackTimer;

    private AttackableTile attackingTile;

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

    public void Attack(AttackableTile tile, Action onFinished)
    {
        attackingTile = tile;
    }

    public void Goto(Vector3 position, Action onArrivedAction = null)
    {
        navAgentHandler.SetTarget(position, onArrivedAction);
    }

    private void Update()
    {
        if (attackingTile != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpeed)
            {
                attackingTile.TakeDamage(5);
                attackTimer = 0;
            }
        }
        else
            attackTimer = 0;
    }
}
