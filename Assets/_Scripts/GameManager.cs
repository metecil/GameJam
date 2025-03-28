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


    // --- Gravity Switch Variables ---
    public bool gravityActive = false;
    public float gravityConstant = 5f; // Adjust to set the overall strength of gravity

    // --- Countdown Variables ---
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to a TextMeshProUGUI element for countdown
    [SerializeField] private int countdownStart = 5;          // Countdown starting number

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
            
            // If this is the Level scene, reassign the coinText reference.
            if (scene.name == "Level")
            {
                GameObject coinTextObject = GameObject.Find("CoinText");
                if (coinTextObject != null)
                {
                    coinText = coinTextObject.GetComponent<TextMeshProUGUI>();
                    UpdateCoinUI();
                }
                else
                {
                    Debug.LogWarning("CoinText object not found. Check that your UI element is named 'CoinText'.");
                }
            }
            
            // Start gravity switch routine when the level loads.
            StartCoroutine(GravitySwitchRoutine());
        }
    }

    private IEnumerator GravitySwitchRoutine()
    {
        while (true)
        {
            // Wait for a random time before activating gravity.
            float waitBefore = UnityEngine.Random.Range(20f, 30f);
            yield return new WaitForSeconds(waitBefore);

            // Countdown from countdownStart to 1.
            if (countdownText != null)
            {
                for (int i = countdownStart; i > 0; i--)
                {
                    countdownText.text = i.ToString();
                    yield return new WaitForSeconds(1f);
                }
                // Optionally, display "GO!" or clear the text.
                countdownText.text = "Gravity Switch!!";
                yield return new WaitForSeconds(0.5f);
                countdownText.text = "";
            }

            gravityActive = true;
            Debug.Log("Gravity activated!");
            
            // Gravity remains active for a random duration.
            float activeDuration = UnityEngine.Random.Range(5f, 10f);
            yield return new WaitForSeconds(activeDuration);

            gravityActive = false;
            Debug.Log("Gravity deactivated!");
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
