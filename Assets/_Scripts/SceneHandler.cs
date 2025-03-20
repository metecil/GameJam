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
    private CanvasGroup canvasGroup; // For fading effect
    private bool preloadDone = false; // To prevent duplicate execution

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
            initXPosition = transitionCanvas.transform.localPosition.x;
            SceneManager.sceneLoaded += OnSceneLoad;

            // Get or add a CanvasGroup for fade effects
            canvasGroup = transitionCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = transitionCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1; // Ensure it starts fully visible
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

    // Wait 5 seconds in the preload scene then load the main menu.
    private IEnumerator ShowPreloadThenLoadMenu()
    {
        Debug.Log("Preload Scene Active. Waiting for .5 seconds...");
        yield return new WaitForSeconds(.5f);

        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene(menuScene); // Load the Main Menu scene
        // The preload panel remains as an overlay during the scene load.
    }

    // When a new scene is loaded, if it's the main menu, start the fade-out.
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuScene)
        {
            Debug.Log("Main Menu loaded. Starting fade-out of overlay panel...");
            StartCoroutine(FadeOutOverlay());
        }
    }

    // Fade out the overlay panel and then destroy it.
    private IEnumerator FadeOutOverlay()
    {
        canvasGroup.DOFade(0, animationDuration).SetEase(animationType);
        yield return new WaitForSeconds(animationDuration);
        Destroy(gameObject); // Remove the overlay object once fade-out is complete.
    }

    public void LoadNextScene()
    {
        if (nextLevelIndex >= levels.Count)
        {
            LoadMenuScene();
        }
        else
        {
            transitionCanvas.DOLocalMoveX(initXPosition + transitionCanvas.rect.width, animationDuration)
                .SetEase(animationType);
            StartCoroutine(LoadSceneAfterTransition(levels[nextLevelIndex]));
            nextLevelIndex++;
        }
    }

    public void LoadMenuScene()
    {
        transitionCanvas.DOLocalMoveX(initXPosition + transitionCanvas.rect.width, animationDuration)
            .SetEase(animationType);
        StartCoroutine(LoadSceneAfterTransition(menuScene));
        nextLevelIndex = 0;
    }

    private IEnumerator LoadSceneAfterTransition(string sceneName)
    {
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene(sceneName);
    }
}
