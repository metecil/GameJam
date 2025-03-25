using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class LivesCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI current;
    [SerializeField] private TextMeshProUGUI toUpdate;
    [SerializeField] private Transform livesTextContainer;
    [SerializeField] private float duration;

    private float containerInitPosition;
    private float moveAmount;

    private void Awake()
    {
        containerInitPosition = livesTextContainer.localPosition.y;
        moveAmount = current.rectTransform.rect.height;
       

    }

    private void OnEnable()
    {
        GameManager.Instance.OnLivesUpdated += UpdateLives;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnLivesUpdated -= UpdateLives;
    }

    private void UpdateLives(int lives)
    {
        toUpdate.SetText($"{lives}");

        livesTextContainer.DOLocalMoveY(containerInitPosition + moveAmount, duration);

        StartCoroutine(ResetLivesContainer(lives));
    }

    private IEnumerator ResetLivesContainer(int lives)
    {
        yield return new WaitForSeconds(duration);
        current.SetText($"{lives}");
        livesTextContainer.localPosition = new Vector3(livesTextContainer.localPosition.x, containerInitPosition, livesTextContainer.localPosition.z);
    }
}
