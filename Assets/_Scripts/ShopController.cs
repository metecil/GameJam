using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopUIController : MonoBehaviour
{
    [Header("Assign your Shop Canvas GameObject from the scene (initially inactive)")]
    [SerializeField] private GameObject shopCanvas;

    private bool shopOpen = false;
    private float previousTimeScale = 1f;

    // Target resolution for WebGL (960x540)
    private Vector2 targetResolution = new Vector2(960, 540);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleShop();
        }
    }

    void ToggleShop()
    {
        if (!shopOpen)
        {
            // Pause game
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            // Unlock and show the mouse cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Enable the shop canvas
            shopCanvas.SetActive(true);

            // (Optional) Adjust the Canvas Scaler to use the target resolution.
            // If your shop canvas already has these settings in the editor, this block might be redundant.
            CanvasScaler scaler = shopCanvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                // If the Canvas Scaler is on a child object, try to find it.
                scaler = shopCanvas.GetComponentInChildren<CanvasScaler>();
            }
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = targetResolution;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            // Force a layout rebuild to ensure margins and sizes update immediately.
            RectTransform rect = shopCanvas.GetComponent<RectTransform>();
            if (rect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }

            // Ensure an EventSystem exists in the scene (for button clicks)
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            shopOpen = true;
        }
        else
        {
            // Resume game
            Time.timeScale = previousTimeScale;

            // Lock and hide the cursor again for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Disable the shop canvas
            shopCanvas.SetActive(false);

            shopOpen = false;
        }
    }
}
