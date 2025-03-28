using System;
using UnityEngine;
using UnityEngine.Events;

public class InputHandler : SingletonMonoBehavior<InputHandler>
{
    public static event Action OnShopToggleRequested;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B pressed");
            // Fire the event instead of directly calling ToggleShop
            OnShopToggleRequested?.Invoke();
        }
    }
}
