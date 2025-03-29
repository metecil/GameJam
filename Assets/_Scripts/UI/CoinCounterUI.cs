using UnityEngine;
using TMPro;

public class CoinCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void OnEnable()
    {
        GameManager.Instance.OnCoinsUpdated += UpdateCoinUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCoinsUpdated -= UpdateCoinUI;
    }

    private void UpdateCoinUI(int coinCount)
    {
        coinText.text = "x" + coinCount;
    }
}
