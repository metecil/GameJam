using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    // Set how many coins this pickup is worth.
    public int coinValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddCoins(coinValue); // You'll need to add this method in GameManager.
            Destroy(gameObject);
        }
    }
}
