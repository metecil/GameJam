using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.Translate(Vector3.up * (speed * Time.deltaTime));
    }

    private void OnBecameInvisible() => Destroy(gameObject);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        if (other.gameObject.CompareTag("Asteroid2")) GameManager.instance.AddScore(1);
        if (other.gameObject.CompareTag("Asteroid1")) GameManager.instance.AddScore(2);
        Destroy(other.gameObject);
        Destroy(gameObject);

       
    }
}
