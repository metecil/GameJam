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

        minSpeed = 2f + currentScore / 100f;
        maxSpeed = 5f + currentScore / 50f;
        spawnRate = Mathf.Max(0.5f, 2f - currentScore / 100f);

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
