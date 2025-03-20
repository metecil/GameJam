using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : SingletonMonoBehavior<SceneHandler>
{
    [Header("Scene Data")]
    [SerializeField] private List<string> levels;
    [SerializeField] private string menuScene;

    [Header("Transition Animation Data")]
    [SerializeField] private Ease animationType;
    [SerializeField] private float animationDuration;
    [SerializeField] private RectTransform transitionCanvas;
    
    private int nextLevelIndex;
    private float initXPosition;
    private CanvasGroup canvasGroup; // For fade effect
    private bool preloadDone = false; // Prevents duplicate execution

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
            initXPosition = transitionCanvas.transform.localPosition.x;
            SceneManager.sceneLoaded += OnSceneLoad;

            // Get or add a CanvasGroup for fade effects on the overlay panel.
            canvasGroup = transitionCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = transitionCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1; // Start fully visible.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!preloadDone)
        {
            preloadDone = true;
            StartCoroutine(ShowPreloadThenLoadMenu());
        }
    }

    // Waits in the preload scene, then loads the main menu.
    private IEnumerator ShowPreloadThenLoadMenu()
    {
        yield return new WaitForSeconds(.5f);
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene(menuScene); // Load the Main Menu scene.
    }
    // Called when a new scene is loaded.
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuScene)
        {
            nextLevelIndex = 0; // Reset the index so clicking Play loads level 1.
            Debug.Log("Main Menu loaded. nextLevelIndex reset to 0. Starting fade-out of overlay panel...");
            StartCoroutine(FadeOutOverlay());
        }
    }

    // Fade out the overlay panel and then destroy just the overlay.
    private IEnumerator FadeOutOverlay()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, animationDuration).SetEase(animationType);
            yield return new WaitForSeconds(animationDuration);
        }
        if (transitionCanvas != null)
        {
            Destroy(transitionCanvas.gameObject);
            transitionCanvas = null; // Clear the reference.
        }
        yield break;
    }

    public void LoadNextScene()
    {
        if (nextLevelIndex >= levels.Count)
        {
            LoadMenuScene();
        }
        else
        {
            // Only animate the overlay if it still exists (for scene 0).
            if (transitionCanvas != null)
            {
                transitionCanvas.DOLocalMoveX(initXPosition + transitionCanvas.rect.width, animationDuration)
                    .SetEase(animationType);
            }
            StartCoroutine(LoadSceneAfterTransition(levels[nextLevelIndex]));
            nextLevelIndex++;
        }
    }

    public void LoadMenuScene()
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.DOLocalMoveX(initXPosition + transitionCanvas.rect.width, animationDuration)
                .SetEase(animationType);
        }
        StartCoroutine(LoadSceneAfterTransition(menuScene));
        nextLevelIndex = 0;
    }

    private IEnumerator LoadSceneAfterTransition(string sceneName)
    {
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene(sceneName);
    }
}
