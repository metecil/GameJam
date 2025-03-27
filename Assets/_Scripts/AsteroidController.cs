using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;   // Explosion effect prefab
    [SerializeField] private AudioClip explosionSound;       // Explosion sound effect
    [SerializeField] private float explosionLifetime = 2f;     // Time before explosion is destroyed

    // Fields for splitting big asteroids:
    [SerializeField] private GameObject asteroid1Prefab;       // For splitting Asteroid1Big
    [SerializeField] private GameObject asteroid2Prefab;       // For splitting Asteroid2Big
    [SerializeField] private float splitAsteroidMinSpeed = 2f;
    [SerializeField] private float splitAsteroidMaxSpeed = 5f;

    // New field for maximum asteroid speed:
    [SerializeField] private float maxAsteroidSpeed = 20f; // Adjust as needed

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosionInstance, explosionLifetime);
            Debug.Log("Explosion triggered at " + transform.position);
        }
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
        Rigidbody splitRb = splitAsteroid.GetComponent<Rigidbody>();
        if (splitRb != null)
        {
            float speed = Random.Range(splitAsteroidMinSpeed, splitAsteroidMaxSpeed);
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            splitRb.linearVelocity = new Vector3(randomDir.x, randomDir.y, 0f) * speed;
            splitRb.angularVelocity = new Vector3(0, 0, Random.Range(-5f, 5f));
            Debug.Log("Split asteroid spawned with speed: " + speed + " and direction: " + randomDir);
        }
        else
        {
            Debug.LogWarning("The spawned split asteroid (" + prefab.name + ") has no Rigidbody!");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gravityActive)
        {
            // --- Gravitational pull from the spaceship ---
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float myMass = (gameObject.CompareTag("Asteroid1Big") || gameObject.CompareTag("Asteroid2Big")) ? 2f : 1f;
                Vector3 directionToPlayer = player.transform.position - transform.position;
                float distanceToPlayer = directionToPlayer.magnitude;
                if (distanceToPlayer < 0.1f)
                    distanceToPlayer = 0.1f;
                float forceMagPlayer = GameManager.instance.gravityConstant * (myMass * 1f) / (distanceToPlayer * distanceToPlayer);
                float spaceshipGravityMultiplier = 20f;
                rb.AddForce(directionToPlayer.normalized * forceMagPlayer * spaceshipGravityMultiplier);
            }

            // --- Gravitational pull from other asteroids ---
            AsteroidController[] allAsteroids = Object.FindObjectsByType<AsteroidController>(FindObjectsSortMode.None);
            foreach (AsteroidController other in allAsteroids)
            {
                if (other == this)
                    continue;

                Vector3 direction = other.transform.position - transform.position;
                float distance = direction.magnitude;
                if (distance < 0.1f)
                    distance = 0.1f;

                float otherMass = (other.gameObject.CompareTag("Asteroid1Big") || other.gameObject.CompareTag("Asteroid2Big")) ? 2f : 1f;
                float myMass = (gameObject.CompareTag("Asteroid1Big") || gameObject.CompareTag("Asteroid2Big")) ? 2f : 1f;
                float forceMagAsteroid = GameManager.instance.gravityConstant * (myMass * otherMass) / (distance * distance);
                float asteroidGravityMultiplier = 5f;
                rb.AddForce(direction.normalized * forceMagAsteroid * asteroidGravityMultiplier);
            }
        }

        // --- Clamp asteroid speed ---
        if (rb.linearVelocity.magnitude > maxAsteroidSpeed)
        {
            Debug.Log("Clamping asteroid speed from " + rb.linearVelocity.magnitude + " to " + maxAsteroidSpeed);
            rb.linearVelocity = rb.linearVelocity.normalized * maxAsteroidSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            Debug.Log("Asteroid triggered collision with Laser.");
            TriggerExplosion();
            Destroy(other.gameObject);
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
