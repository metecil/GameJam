using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class ScoreCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI toUpdateScoreText;
    [SerializeField] private Transform scoreTextContainer;
    [SerializeField] private float duration;

    private float containerInitPosition;
    private float moveAmount;

    private void Awake()
    {
        containerInitPosition = scoreTextContainer.localPosition.y;
        moveAmount = currentScoreText.rectTransform.rect.height;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnScoreUpdated += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnScoreUpdated -= UpdateScore;
    }

    private void UpdateScore(int score)
    {
        toUpdateScoreText.SetText($"{score}");
        scoreTextContainer.DOLocalMoveY(containerInitPosition + moveAmount, duration);

        StartCoroutine(ResetScoreContainer(score));
    }

    private IEnumerator ResetScoreContainer(int score)
    {
        yield return new WaitForSeconds(duration);
        currentScoreText.SetText($"{score}");
        scoreTextContainer.localPosition = new Vector3(scoreTextContainer.localPosition.x, containerInitPosition, scoreTextContainer.localPosition.z);
    }
}
