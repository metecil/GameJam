using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    private float screenWidth;
    private float screenHeight;
    private Camera mainCamera;
    private bool isVisible = false; 

    private void Start()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;
        Invoke(nameof(EnableVisibilityCheck), 1f);
    }

    private void EnableVisibilityCheck()
    {
        isVisible = true;
    }

    private void Update()
    {
        if (!isVisible) return; 

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
        {
            Destroy(gameObject);
        }
    }
}
