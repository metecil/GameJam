using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;
using System;

public class ShopUIController : MonoBehaviour
{
    // Reference to the shop panel (child GameObject) that is toggled.
    // This shop panel should be assigned in the Inspector and can be inactive by default.
    [SerializeField] private GameObject shopPanel;
    
    private bool shopOpen = false;
    private float previousTimeScale = 1f;
    private Vector2 targetResolution = new Vector2(960, 540);

    private void Awake()
    {
        // Ensure that this GameObject (the one with ShopUIController) is active.
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("ShopUIController is inactive. Make sure its GameObject is active so it can subscribe to events.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to the event from GameManager.
        GameManager.Instance.OnShopToggleRequested += ToggleShop;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnShopToggleRequested -= ToggleShop;
    }

    public void ToggleShop()
    {
        if (!shopOpen)
        {
            Debug.Log("Attempting to open shop...");
            if (shopPanel == null)
            {
                Debug.LogError("Shop Panel reference is missing!");
                return;
            }
            
            // Check if the game is already paused. If so, set a default time scale.
            previousTimeScale = Time.timeScale;
            if(previousTimeScale == 0f)
            {
                previousTimeScale = 1f; // or whatever your normal game time scale should be
            }
            
            // Pause the game and show the shop panel.
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            shopPanel.SetActive(true);
            Debug.Log("Shop panel active: " + shopPanel.activeSelf);
            AdjustCanvasScaler();

            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
            
            shopOpen = true;
        }
        else
        {
            Debug.Log("Closing shop...");
            // Resume the game and hide the shop panel.
            Time.timeScale = previousTimeScale;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            shopPanel.SetActive(false);
            Debug.Log("Shop panel active: " + shopPanel.activeSelf);
            shopOpen = false;
        }
    }

    private void AdjustCanvasScaler()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>() ?? GetComponentInChildren<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = targetResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}
