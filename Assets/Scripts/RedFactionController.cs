using UnityEngine;

public class RedFactionController : MonoBehaviour
{
    private float actionTimer;
    private float actionTimerMax = 5.0f;

    private IRedFactionStrategy strategy;

    private void Awake()
    {
        strategy = RedFactionStrategyFactory.Create();
    }

    private void Update()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= actionTimerMax)
        {
            actionTimer = 0;
            strategy.ChooseAction().Execute();
        }
    }
}
