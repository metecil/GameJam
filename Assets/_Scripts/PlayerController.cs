using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed = 10f; // Maximum speed limit
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject[] cannons;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private AudioClip fireAudioClip; // Laser fire sound effect

    // Thruster logic fields:
    [SerializeField] private GameObject thrusterDisplay;
    [SerializeField] private Sprite[] thrusterSprites; 
    [SerializeField] private float thrusterAccelerationTime = 1f;
    [SerializeField] private Vector3 thrusterLocalOffset = new Vector3(0, -1f, 0);
    private float thrusterTimer = 0f;

    private Rigidbody rigidBody;
    private int currentCannon = 0;
    private float time = 0f;
    private Collider[] colliders;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        colliders = GetComponents<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Update thruster display position/rotation.
        thrusterDisplay.transform.position = transform.TransformPoint(thrusterLocalOffset);
        thrusterDisplay.transform.rotation = transform.rotation;

        // Laser firing.
        if (time > 0f)
            time -= Time.deltaTime;
        else if (Input.GetKey(KeyCode.Space))
        {
            var laserOriginTransform = transform;
            if (cannons.Length > 0)
            {
                laserOriginTransform = cannons[currentCannon++].transform;
                if (currentCannon >= cannons.Length)
                    currentCannon = 0;
            }
            Instantiate(laser, laserOriginTransform.position, laserOriginTransform.rotation);
            if (fireAudioClip != null)
                AudioSource.PlayClipAtPoint(fireAudioClip, laserOriginTransform.position);
            time = cooldown;
        }

        // Movement and thruster logic.
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rigidBody.AddForce(transform.up * (movementSpeed * Time.deltaTime));
            thrusterTimer += Time.deltaTime;
            thrusterTimer = Mathf.Clamp(thrusterTimer, 0f, thrusterAccelerationTime);
            int level = Mathf.FloorToInt((thrusterTimer / thrusterAccelerationTime) * thrusterSprites.Length);
            level = Mathf.Clamp(level, 0, thrusterSprites.Length - 1);
            thrusterDisplay.GetComponent<SpriteRenderer>().sprite = thrusterSprites[level];
        }
        else
        {
            thrusterTimer = 0f;
            thrusterDisplay.GetComponent<SpriteRenderer>().sprite = thrusterSprites[0];
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            rigidBody.AddForce(transform.up * (-movementSpeed * Time.deltaTime));
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        // Clamp velocity.
        if (rigidBody.linearVelocity.magnitude > maxSpeed)
            rigidBody.linearVelocity = rigidBody.linearVelocity.normalized * maxSpeed;
    }

    private void FixedUpdate()
    {
        // When gravity is active, apply gravitational pull from each asteroid.
        if (GameManager.instance.gravityActive)
        {
            // Get all asteroids via their AsteroidController.
            AsteroidController[] asteroids = FindObjectsOfType<AsteroidController>();
            Vector3 totalForce = Vector3.zero;
            foreach (AsteroidController a in asteroids)
            {
                // Determine a mass factor for the asteroid: regular = 1; big asteroids = 2.
                float asteroidMass = 1f;
                if (a.gameObject.CompareTag("Asteroid1Big") || a.gameObject.CompareTag("Asteroid2Big"))
                    asteroidMass = 2f;
                Vector3 direction = a.transform.position - transform.position;
                float distance = direction.magnitude;
                if (distance < 0.1f) distance = 0.1f;
                // Force = G * (m1*m2)/distance^2, with m1 (ship) = 1, m2 = asteroidMass.
                float forceMag = GameManager.instance.gravityConstant * (1f * asteroidMass) / (distance * distance);
                totalForce += direction.normalized * forceMag;
            }
            rigidBody.AddForce(totalForce);
            Debug.Log("Applied gravitational force to spaceship: " + totalForce);
        }
    }

    private bool IsPlayerVisible()
    {
        var collider = colliders.First(it => it.isTrigger);
        var planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }

    private void OnDestroy()
    {
        // Cleanup if needed.
    }

    public void IncreaseMovementSpeed(float movementAmount, float rotationAmount)
    {
        movementSpeed += movementAmount;
        rotationSpeed += rotationAmount;
        Debug.Log("New movementSpeed: " + movementSpeed);
    }
    public void DecreaseLaserCooldown(float amount)
    {
        cooldown = Mathf.Max(0.1f, cooldown - amount);
    }
}
