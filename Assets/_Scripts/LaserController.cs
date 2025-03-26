using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject coinPrefab; // For coin drop

    // Distance tracking for screen bounds (for orthographic cameras)
    private float totalDistanceTraveled = 0f;
    private Camera mainCamera;
    private float maxDistance;

    private void Start()
    {
        mainCamera = Camera.main;
        // Calculate the diagonal screen distance for an orthographic camera.
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;
        maxDistance = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);
    }

    private void Update()
    {
        // Move the laser upward and accumulate distance traveled.
        float deltaDistance = speed * Time.deltaTime;
        totalDistanceTraveled += deltaDistance;
        transform.Translate(Vector3.up * deltaDistance);

        // Destroy the laser if it has traveled beyond the screen.
        if (totalDistanceTraveled > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with the player.
        if (other.gameObject.CompareTag("Player"))
            return;

        bool handled = false;

        // Normal asteroids (use tags "Asteroid1" and "Asteroid2")
        if (other.gameObject.CompareTag("Asteroid1") || other.gameObject.CompareTag("Asteroid2"))
        {
            if (other.gameObject.CompareTag("Asteroid2"))
            {
                GameManager.instance.AddScore(1);
            }
            else if (other.gameObject.CompareTag("Asteroid1"))
            {
                GameManager.instance.AddScore(2);
            }
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, other.transform.position, Quaternion.identity);
            }
            handled = true;
        }
        // Big asteroids with explosion effects (use tags "Asteroid1Big" and "Asteroid2Big")
        else if (other.gameObject.CompareTag("Asteroid1Big") || other.gameObject.CompareTag("Asteroid2Big"))
        {
            // Trigger explosion effect if available.
            AsteroidController asteroidCtrl = other.gameObject.GetComponent<AsteroidController>();
            if (asteroidCtrl != null)
            {
                asteroidCtrl.TriggerExplosion();
            }
            
            if (other.gameObject.CompareTag("Asteroid2Big"))
            {
                GameManager.instance.AddScore(1);
            }
            else if (other.gameObject.CompareTag("Asteroid1Big"))
            {
                GameManager.instance.AddScore(2);
            }
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, other.transform.position, Quaternion.identity);
            }
            handled = true;
        }

        // If one of our conditions was met, destroy the asteroid.
        if (handled)
        {
            Destroy(other.gameObject);
        }
        // Always destroy the laser after collision.
        Destroy(gameObject);
    }
}
