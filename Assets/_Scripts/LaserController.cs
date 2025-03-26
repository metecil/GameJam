using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private float speed;

    private float totalDistanceTraveled = 0f;
    private Camera mainCamera;
    private float maxDistance;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Calculate screen width and height in world units for an orthographic camera.
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;
        maxDistance = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);
        
    }

    private void Update()
    {
        // Calculate the distance traveled this frame and add it to the total.
        float deltaDistance = speed * Time.deltaTime;
        totalDistanceTraveled += deltaDistance;
        transform.Translate(Vector3.up * deltaDistance);


        if (totalDistanceTraveled > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            return;

        if (other.gameObject.CompareTag("Asteroid2"))
            GameManager.instance.AddScore(1);
        if (other.gameObject.CompareTag("Asteroid1"))
            GameManager.instance.AddScore(2);
        
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
