using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] asteroidPrefabs; // Assign asteroid prefabs
    [SerializeField] private float spawnRate = 2f; // Time between spawns
    [SerializeField] private float spawnDistance = 2f; // Distance from screen edge
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 5f;

    private Camera mainCamera;
    private float screenWidth;
    private float screenHeight;

    private void Start()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;

        InvokeRepeating(nameof(SpawnAsteroid), 1f, spawnRate);
    }

    private void SpawnAsteroid()
    {
        if (asteroidPrefabs.Length == 0) return;

        int currentScore = 0; 
        if (GameManager.Instance != null)
        {
            currentScore = GameManager.Instance.GetScore();
        }
        switch (currentScore)
        {
            case > 100: 
                spawnRate = 1f;
                minSpeed = 3f;
                maxSpeed = 7f;
                break;
            case > 90: 
                spawnRate = 1.1f;
                minSpeed = 2.9f;
                maxSpeed = 6.8f;
                break;
            case > 80:
                spawnRate = 1.2f;
                minSpeed = 2.8f;
                maxSpeed = 6.6f;
                break;
            case > 70:
                spawnRate = 1.3f;
                minSpeed = 2.7f;
                maxSpeed = 6.4f;
                break;
            case > 60:
                spawnRate = 1.4f;
                minSpeed = 2.6f;
                maxSpeed = 6.2f;
                break;
            case > 50:
                spawnRate = 1.5f;
                minSpeed = 2.5f;
                maxSpeed = 6f;
                break;
            case > 40:
                spawnRate = 1.6f;
                minSpeed = 2.4f;
                maxSpeed = 5.8f;
                break;
            case > 30:
                spawnRate = 1.7f;
                minSpeed = 2.3f;
                maxSpeed = 5.6f;
                break;
            case > 20:
                spawnRate = 1.8f;
                minSpeed = 2.2f;
                maxSpeed = 5.4f;
                break;
            case > 10:
                spawnRate = 1.9f;
                minSpeed = 2.1f;
                maxSpeed = 5.2f;
                break;
        }


        GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

        Vector3 spawnPos = GetRandomEdgePosition();

        Vector3 direction = GetRandomMovementDirection(spawnPos);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb != null)

            rb.useGravity = false;
        {
            float speed = Random.Range(minSpeed, maxSpeed);
            rb.linearVelocity = direction * speed;

            rb.angularVelocity = new Vector3(0, 0, Random.Range(-5f, 5f)); 
            
        }
    }

    private Vector3 GetRandomEdgePosition()
    {
        int edge = Random.Range(0, 4); 
        float x, y;

        switch (edge)
        {
            case 0: 
                x = Random.Range(-screenWidth, screenWidth);
                y = screenHeight + spawnDistance;
                break;
            case 1: 
                x = Random.Range(-screenWidth, screenWidth);
                y = -screenHeight - spawnDistance;
                break;
            case 2: 
                x = -screenWidth - spawnDistance;
                y = Random.Range(-screenHeight, screenHeight);
                break;
            default:
                x = screenWidth + spawnDistance;
                y = Random.Range(-screenHeight, screenHeight);
                break;
        }

        return new Vector3(x, y, 0f); 
    }

    private Vector3 GetRandomMovementDirection(Vector3 spawnPos)
    {
        
        Vector3 randomTarget = new Vector3(
            Random.Range(-screenWidth, screenWidth),
            Random.Range(-screenHeight, screenHeight),
            0f
        );

        return (randomTarget - spawnPos).normalized;
    }
}
