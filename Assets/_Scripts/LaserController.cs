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
        
        // Calculate screen dimensions (for an orthographic camera).
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;
        maxDistance = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);
    }

    private void Update()
    {
        // Move the laser and accumulate the traveled distance.
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
        // Ignore collisions with the player.
        if (other.gameObject.CompareTag("Player"))
            return;

        // If the laser hits an asteroid, trigger explosion effects.
        if (other.gameObject.CompareTag("Asteroid1Big") || other.gameObject.CompareTag("Asteroid2Big"))
        {
            // Try to get the AsteroidController and call TriggerExplosion.
            AsteroidController asteroidCtrl = other.gameObject.GetComponent<AsteroidController>();
            if (asteroidCtrl != null)
            {
                asteroidCtrl.TriggerExplosion();
            }
            
            // Update score based on asteroid type.
            if (other.gameObject.CompareTag("Asteroid2Big"))
                GameManager.instance.AddScore(1);
            else if (other.gameObject.CompareTag("Asteroid1Big"))
                GameManager.instance.AddScore(2);
        }

        // Destroy the asteroid and the laser.
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}