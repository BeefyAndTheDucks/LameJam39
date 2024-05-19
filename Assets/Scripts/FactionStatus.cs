using TMPro;
using UnityEngine;

public class FactionStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private void Update()
    {
        statusText.text = $"FACTION HEALTH: {FactionCentreTile.Instance.Health}/500\nENEMY HEALTH: {FactionCentreTile.EnemyInstance.Health}/500";
    }
}
