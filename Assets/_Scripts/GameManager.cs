using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public static GameManager instance; 

    private int lives;
    private int score;

    public event Action<int> OnScoreUpdated;
    public event Action<int> OnLivesUpdated;

    protected override void Awake()
    {
        if (instance == null) { instance = this; }
        base.Awake();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            score = 0;
            lives = 3;
            OnScoreUpdated?.Invoke(score);
            OnLivesUpdated?.Invoke(lives);
        }

    }

    public int GetScore() => score;
    public int GetLives() => lives;

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreUpdated?.Invoke(score);
    }

    public void RemoveLife()
    {
        lives--;
        OnLivesUpdated?.Invoke(lives);
    }


}
