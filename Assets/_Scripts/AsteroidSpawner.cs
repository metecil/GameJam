using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] asteroidPrefabs; // Assign asteroid prefabs
    [SerializeField] private float spawnDistance = 2f; // Distance from screen edge
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private int initialWaveCount = 4;      // Number of asteroids in the first wave
    [SerializeField] private float spawnDuration = 15f;       // Duration to spawn the wave
    [SerializeField] private int waveIncrement = 1;           // How many more asteroids to spawn in the next wave

    private int currentWaveCount;
    private Camera mainCamera;
    private float screenWidth;
    private float screenHeight;

    private void Start()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;

        currentWaveCount = initialWaveCount;
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        // Calculate time between spawns in the current wave.
        float spawnInterval = spawnDuration / currentWaveCount;

        // Spawn currentWaveCount asteroids gradually.
        for (int i = 0; i < currentWaveCount; i++)
        {
            SpawnAsteroid();
            yield return new WaitForSeconds(spawnInterval);
        }

        // Wait until all asteroids (with an AsteroidController) are destroyed.
        while (Object.FindObjectsByType<AsteroidController>(FindObjectsSortMode.None).Length > 0)
        {
            yield return null;
        }

        // Increase the wave count for the next wave.
        currentWaveCount += waveIncrement;

        // Start the next wave.
        StartCoroutine(SpawnWave());
    }

    private void SpawnAsteroid()
    {
        if (asteroidPrefabs.Length == 0) return;

        int currentScore = 0;
        if (GameManager.Instance != null)
        {
            currentScore = GameManager.Instance.GetScore();
        }

        // Adjust speeds based on score if desired.
        minSpeed = 2f + currentScore / 100f;
        maxSpeed = 5f + currentScore / 50f;

        // Pick a random asteroid prefab.
        GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        Vector3 spawnPos = GetRandomEdgePosition();
        Vector3 direction = GetRandomMovementDirection(spawnPos);
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
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
            case 0: // Top
                x = Random.Range(-screenWidth, screenWidth);
                y = screenHeight + spawnDistance;
                break;
            case 1: // Bottom
                x = Random.Range(-screenWidth, screenWidth);
                y = -screenHeight - spawnDistance;
                break;
            case 2: // Left
                x = -screenWidth - spawnDistance;
                y = Random.Range(-screenHeight, screenHeight);
                break;
            default: // Right
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