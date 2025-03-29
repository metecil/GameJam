using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System;

public class ScoreMultiplierUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreMultiplierUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        GameManager.Instance.OnCurrentStreakUpdated += SetMultiplier; 

    }

    private void OnDisable()
    {
        GameManager.Instance.OnCurrentStreakUpdated -= SetMultiplier;
    }

    private void SetMultiplier(int multiplier)
    {
        if (multiplier < 2)
        {
            scoreMultiplierUI.SetText("");
            return;
        }

        scoreMultiplierUI.SetText($"{multiplier}x");

        int maxStreakForFullRed = 10; // You can adjust this
        float t = Mathf.InverseLerp(2, maxStreakForFullRed, multiplier);
        Color streakColor = Color.Lerp(Color.white, Color.red, t);

        scoreMultiplierUI.color = streakColor;
    }


}
