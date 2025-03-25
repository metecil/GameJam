using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] 

    private void OnEnable()
    {
        GameManager.Instance.OnLivesUpdated += CheckGameOver;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnLivesUpdated -= CheckGameOver;
    }

    private void CheckGameOver(int lives)
    {
        if (lives <= 0)
        {
            int score = GameManager.Instance.GetScore();
            int bestScore = GameManager.Instance.GetBestScore();
            if (score > bestScore)
            {
                GameManager.Instance.SetBestScore(score);
                bestScore = score;
            }
            scoreText.SetText($"Score:{score}");
            bestScoreText.SetText($"Best Score:{bestScore}");
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);


        StartCoroutine(DelayedFreeze());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator DelayedFreeze()
    {
        yield return new WaitForSeconds(2f); // 1 second delay (game is still running)
        Time.timeScale = 0f;
    }
}

