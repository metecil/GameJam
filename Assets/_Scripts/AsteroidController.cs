using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;   // Explosion visual effect prefab
    [SerializeField] private AudioClip explosionSound;       // Explosion sound effect
    [SerializeField] private float explosionLifetime = 2f;     // Duration after which the explosion is destroyed

    // Fields for splitting big asteroids into regular ones:
    [SerializeField] private GameObject asteroid1Prefab;       // Regular asteroid prefab for Asteroid1 (spawned from Asteroid1Big)
    [SerializeField] private GameObject asteroid2Prefab;       // Regular asteroid prefab for Asteroid2 (spawned from Asteroid2Big)
    [SerializeField] private float splitAsteroidMinSpeed = 2f;   // Minimum speed for split asteroids
    [SerializeField] private float splitAsteroidMaxSpeed = 5f;   // Maximum speed for split asteroids

    public void TriggerExplosion()
    {
        // Instantiate the explosion effect and schedule its destruction.
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosionInstance, explosionLifetime);
            Debug.Log("Explosion triggered at " + transform.position);
        }
        
        // Play the explosion sound effect.
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            Debug.Log("Explosion sound played at " + transform.position);
        }
    }

    private void SpawnSplitAsteroid(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("SpawnSplitAsteroid called with null prefab.");
            return;
        }

        Debug.Log("Spawning split asteroid from prefab: " + prefab.name);
        GameObject splitAsteroid = Instantiate(prefab, transform.position, Quaternion.identity);
        Rigidbody rb = splitAsteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = Random.Range(splitAsteroidMinSpeed, splitAsteroidMaxSpeed);
            // Generate a random direction in the XY plane.
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rb.linearVelocity = new Vector3(randomDir.x, randomDir.y, 0f) * speed;
            rb.angularVelocity = new Vector3(0, 0, Random.Range(-5f, 5f));
            Debug.Log("Split asteroid spawned with speed: " + speed + " and direction: " + randomDir);
        }
        else
        {
            Debug.LogWarning("The spawned split asteroid (" + prefab.name + ") does not have a Rigidbody component!");
        }
    }

    // Handles trigger collisions (for lasers).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            Debug.Log("Asteroid triggered collision with Laser.");
            TriggerExplosion();
            Destroy(other.gameObject); // Destroy the laser

            // Check if this is a big asteroid and spawn split asteroids.
            if (gameObject.CompareTag("Asteroid1Big"))
            {
                Debug.Log("Big Asteroid1Big hit. Spawning two regular Asteroid1.");
                SpawnSplitAsteroid(asteroid1Prefab);
                SpawnSplitAsteroid(asteroid1Prefab);
            }
            else if (gameObject.CompareTag("Asteroid2Big"))
            {
                Debug.Log("Big Asteroid2Big hit. Spawning two regular Asteroid2.");
                SpawnSplitAsteroid(asteroid2Prefab);
                SpawnSplitAsteroid(asteroid2Prefab);
            }
            Destroy(gameObject);
        }
    }

    // Handles physical collisions (for the player).
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Asteroid collided with Player via OnCollisionEnter.");
            GameManager.instance.RemoveLife();
            TriggerExplosion();
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}