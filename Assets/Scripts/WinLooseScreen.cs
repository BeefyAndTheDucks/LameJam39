using TMPro;
using UnityEngine;

public class WinLooseScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text winLooseText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Win()
    {
        gameObject.SetActive(true);
        winLooseText.text = "You Won!";
    }

    public void Lose()
    {
        gameObject.SetActive(true);
        winLooseText.text = "You Lost!";
    }

    public void Quit() => Application.Quit();
}
