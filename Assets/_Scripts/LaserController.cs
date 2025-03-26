using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject coinPrefab; // Assign your coin drop prefab here.

    private float totalDistanceTraveled = 0f;
    private Camera mainCamera;
    private float maxDistance;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Calculate screen dimensions (for an orthographic camera).
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;
        maxDistance = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);
    }

    private void Update()
    {
        // Move the laser upward and track the total distance traveled.
        float deltaDistance = speed * Time.deltaTime;
        totalDistanceTraveled += deltaDistance;
        transform.Translate(Vector3.up * deltaDistance);

        // Destroy the laser if it travels beyond the maximum distance.
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

        // Check if the laser hit an asteroid.
        if (other.gameObject.CompareTag("Asteroid1") || other.gameObject.CompareTag("Asteroid2"))
        {
            // Trigger explosion on the asteroid if possible.
            AsteroidController asteroidCtrl = other.gameObject.GetComponent<AsteroidController>();
            if (asteroidCtrl != null)
            {
                asteroidCtrl.TriggerExplosion();
            }

            // Update score based on the asteroid type.
            if (other.gameObject.CompareTag("Asteroid2"))
            {
                GameManager.instance.AddScore(1);
            }
            else if (other.gameObject.CompareTag("Asteroid1"))
            {
                GameManager.instance.AddScore(2);
            }
            
            // Instantiate a coin drop at the asteroid's position.
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, other.transform.position, Quaternion.identity);
            }
        }

        // Destroy both the asteroid (or other collided object) and the laser.
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
