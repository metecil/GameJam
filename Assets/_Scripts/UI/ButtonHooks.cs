using UnityEngine;

public class ButtonHooks : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;
    public void LoadNextScene()
    {
        SceneHandler.Instance.LoadNextScene();
    }

    public void ExitToMenu()
    {
        SceneHandler.Instance.LoadMenuScene();
    }

    public void DisplayHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }
}
