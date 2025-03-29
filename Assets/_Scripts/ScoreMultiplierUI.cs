using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System;

public class ScoreMultiplierUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreMultiplierUI;
    private bool shouldFlash = false;
    private float flashSpeed = 4f;
    private float flashThreshold = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Update()
    {
        if (shouldFlash)
        {
            float alpha = Mathf.Lerp(0.3f, 1f, Mathf.PingPong(Time.time * flashSpeed, 1f));
            scoreMultiplierUI.alpha = alpha;
        }
        else
        {
            scoreMultiplierUI.alpha = 1f;
        }
    }


    private void OnEnable()
    {
        GameManager.Instance.OnCurrentStreakUpdated += SetMultiplier; 

    }

    private void OnDisable()
    {
        GameManager.Instance.OnCurrentStreakUpdated -= SetMultiplier;
    }

    private void SetMultiplier(int multiplier, float timeElapsed)
    {
        if (multiplier < 2)
        {
            scoreMultiplierUI.SetText("");
            shouldFlash = false;
            return;
        }

        float timeRemaining = GameManager.Instance.GetStreakDuration() - timeElapsed;
        shouldFlash = timeRemaining <= flashThreshold;

        scoreMultiplierUI.SetText($"{multiplier}x");

        int maxStreakForFullRed = 10; // You can adjust this
        float t = Mathf.InverseLerp(2, maxStreakForFullRed, multiplier);
        Color streakColor = Color.Lerp(Color.white, Color.red, t);

        scoreMultiplierUI.color = streakColor;
    }


}
