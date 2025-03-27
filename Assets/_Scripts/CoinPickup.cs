using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    // Set how many coins this pickup is worth.
    [SerializeField] private int coinValue = 1;
    [SerializeField] private float timeout = 10.0f;

    // Audio clip to play when the coin is picked up.
    [SerializeField] private AudioClip pickupSound;

    private void Start()
    {
        // Automatically destroy this coin after 10 seconds if not picked up.
        Destroy(gameObject, timeout);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add coins to the player's total.
            GameManager.instance.AddCoins(coinValue);

            // Play the pickup sound at the coin's position.
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Destroy the coin game object.
            Destroy(gameObject);
        }
    }
}
