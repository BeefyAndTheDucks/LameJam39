using UnityEngine;

public interface IRedFactionStrategy
{
    AIAction ChooseAction();
}

public static class RedFactionStrategyFactory
{
    public static IRedFactionStrategy Create() => new LegacyRedFactionStrategy();
}

public class LegacyRedFactionStrategy : IRedFactionStrategy
{
    private const int k_AttackWorkerCount = 5;

    public AIAction ChooseAction()
    {
        if (EnemyWorkers.HasAvailableWorkers(k_AttackWorkerCount + 1))
            return AIAction.Factory.CreateAttackAction(k_AttackWorkerCount);

        if (EnemyWorkers.GetAvailableWorkers() <= 2 || Random.Range(0, 10) <= 3)
            return AIAction.Factory.CreateConstructionAction(GameManager.Instance.enemyWorkerHousingTile);

        if (EnemyWorkers.GetWorkerCount() < EnemyWorkers.GetWorkerLimit() || Random.Range(0, 10) <= 1)
            return AIAction.Factory.CreateConstructionAction(GameManager.Instance.enemyTrainingCenterTile);

        // Create Turret
        return AIAction.Factory.CreateConstructionAction(GameManager.Instance.enemyTurretTile);
    }
}
