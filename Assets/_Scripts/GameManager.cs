using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;



public class GameManager : SingletonMonoBehavior<GameManager>
{
    public static GameManager instance;

    private int bestScore = 0; 
    private int lives = 3;
    private int score = 0;
    private int coins = 0;

    public event Action<int> OnScoreUpdated;
    public event Action<int> OnLivesUpdated;
    public event Action<int> OnCoinsUpdated;
    public event Action OnShopToggleRequested;


    protected override void Awake()
    {
        if (instance == null) { instance = this; }
        base.Awake();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B pressed");
            // Fire the event instead of directly calling ToggleShop
            OnShopToggleRequested?.Invoke();
        }
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
            coins = 0;
            OnScoreUpdated?.Invoke(score);
            OnLivesUpdated?.Invoke(lives);
            OnCoinsUpdated?.Invoke(coins);
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

    public int GetCoins() => coins;

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsUpdated?.Invoke(coins);
    }

    public void ResetCoins()
    {
        coins = 0;
        OnCoinsUpdated?.Invoke(coins);
    }
}
