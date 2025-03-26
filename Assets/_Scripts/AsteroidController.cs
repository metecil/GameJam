using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab; // Explosion visual effect prefab
    [SerializeField] private AudioClip explosionSound;     // Explosion sound effect
    [SerializeField] private float explosionLifetime = 2f;   // Duration after which the explosion is destroyed

    public void TriggerExplosion()
    {
        // Instantiate the explosion effect and schedule it for destruction.
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosionInstance, explosionLifetime); // Destroy after the explosion effect finishes
        }
        
        // Play the explosion sound; AudioSource.PlayClipAtPoint automatically cleans up the temporary AudioSource.
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.RemoveLife();
            TriggerExplosion();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Laser"))
        {
            TriggerExplosion();
            Destroy(collision.gameObject); // Destroy the laser
            Destroy(gameObject);
        }
    }
}
