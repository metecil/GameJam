using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;



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
    public event Action<int, float> OnCurrentStreakUpdated;


    // --- Gravity Switch Variables ---
    public bool gravityActive = false;
    public float gravityConstant = 5f; // Adjust to set the overall strength of gravity

    // --- Countdown Variables ---
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to a TextMeshProUGUI element for countdown
    [SerializeField] private int countdownStart = 5;          // Countdown starting number

    // --- Score multiplier variables ---
    private float lastDestroyTime = -10f;
    private int currentStreak = 0;
    [SerializeField] private float streakDuration = 4f;
    private int maxStreak = 5;

    // --- Background Music ---
    [SerializeField] private AudioClip backgroundMusic;       // Background music clip
    private AudioSource audioSource;                          // AudioSource for background music

    // --- Game Over Sound ---
    [SerializeField] private AudioClip gameOverSound;         // Sound to play when game is over

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        base.Awake();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Set up the AudioSource for background music.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f; // Adjust volume as needed.
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B pressed");
            // Fire the event instead of directly calling ToggleShop
            OnShopToggleRequested?.Invoke();
        }

        // Reset current streak to 0 if the player has not destroyed an asteroid in the last streakDuration amount of time. 
        if (Time.time - lastDestroyTime > streakDuration)
        {
            currentStreak = 0;
        }
        OnCurrentStreakUpdated?.Invoke(currentStreak, Time.time - lastDestroyTime);

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
            currentStreak = 0;
            lastDestroyTime = -10f;
            OnScoreUpdated?.Invoke(score);
            OnLivesUpdated?.Invoke(lives);
            OnCurrentStreakUpdated?.Invoke(currentStreak, Time.time - lastDestroyTime);
            
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
                // Optionally, display "Gravity Switch!!" briefly.
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

    public int GetCurrentStreak() => currentStreak;
    public void SetLastDestroyTime(float time)
    {
        lastDestroyTime = time;
    }
    public void IncrementStreak()
    {
        currentStreak = Math.Min(currentStreak + 1, maxStreak); // Cap streak at maxStreak
        OnCurrentStreakUpdated?.Invoke(currentStreak, Time.time - lastDestroyTime);
    }
    public float GetStreakDuration() => streakDuration;
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

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            if (gameOverSound != null)
            {
                // Play game over sound at the camera's position.
                AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
            }
            // Optional: trigger additional game over logic (e.g., load Game Over scene).
        }
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
