using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NavAgentHandler), typeof(Animator))]
public class Worker : MonoBehaviour
{
    public event EventHandler OnDeath;
    private const int k_MaxHealth = 100;
    public int Health { get; private set; } = k_MaxHealth;
    public bool IsEnemy = false;

    [SerializeField] private float attackSpeed = 1.0f;
    private float attackTimer;

    private AttackableTile attackingTile;

    private NavAgentHandler navAgentHandler;

    private Action onFinishedAttacking;

    [SerializeField] private GameObject healthBarCanvas;
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        navAgentHandler = GetComponent<NavAgentHandler>();
        healthBarCanvas.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        healthBar.fillAmount = (float)Health / k_MaxHealth;
        healthBarCanvas.SetActive(true);

        if (Health <= 0)
            OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void Attack(AttackableTile tile, Action onFinished)
    {
        attackingTile = tile;
        onFinishedAttacking = onFinished;
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
        {
            attackTimer = 0;
            onFinishedAttacking?.Invoke();
            onFinishedAttacking = null;
        }
    }
}
