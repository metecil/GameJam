using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject coinPrefab;

    private void Update()
    {
        transform.Translate(Vector3.up * (speed * Time.deltaTime));
    }

    private void OnBecameInvisible() => Destroy(gameObject);

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with the player.
        if (other.gameObject.CompareTag("Player"))
            return;

        // If the laser hits an asteroid, add score and drop a coin.
        if (other.gameObject.CompareTag("Asteroid2"))
        {
            GameManager.instance.AddScore(1);
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, other.transform.position, Quaternion.identity);
            }
        }
        else if (other.gameObject.CompareTag("Asteroid1"))
        // If the laser hits an asteroid, trigger explosion effects.
        if (other.gameObject.CompareTag("Asteroid1Big") || other.gameObject.CompareTag("Asteroid2Big"))
        {
            GameManager.instance.AddScore(2);
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, other.transform.position, Quaternion.identity);
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