using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.RemoveLife();
            Destroy(gameObject);
        }
        else
        {
            return;
        }

    }
        
}
