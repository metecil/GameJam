using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public static GameManager instance;

    private int bestScore = 0; 
    private int lives = 3;
    private int score = 0;

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
        if (scene.name == "MainMenu" || scene.name == "Level")
        {
            score = 0;
            lives = 3;
            OnScoreUpdated?.Invoke(score);
            OnLivesUpdated?.Invoke(lives);
        }

    }

    public int GetScore() => score;
    public int GetLives() => lives;

    public int GetBestScore() => bestScore;

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

    public void SetBestScore(int score)
    {
        this.bestScore = score;
    }

}
