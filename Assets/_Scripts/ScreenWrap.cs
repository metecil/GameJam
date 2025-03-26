using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool wrapped = false;

        if (viewportPos.x < 0f)
        {
            viewportPos.x = 1f;
            wrapped = true;
        }
        else if (viewportPos.x > 1f)
        {
            viewportPos.x = 0f;
            wrapped = true;
        }
        if (viewportPos.y < 0f)
        {
            viewportPos.y = 1f;
            wrapped = true;
        }
        else if (viewportPos.y > 1f)
        {
            viewportPos.y = 0f;
            wrapped = true;
        }

        if (wrapped)
        {
            Vector3 newWorldPos = mainCamera.ViewportToWorldPoint(viewportPos);
            newWorldPos.z = 0f;  
            transform.position = newWorldPos;
        }
    }
}
