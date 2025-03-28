using System.Collections;
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

    // Dying/respawn mechanic fields:
    [SerializeField] private AudioClip deathAudioClip; // Ship's dying sound
    [SerializeField] private float respawnDelay = 3f;    // Duration of invulnerability & fade effect
    [SerializeField] private int beamCycles = 3;         // Number of beaming (fade in/out) cycles during respawn

    private Rigidbody rigidBody;
    private int currentCannon = 0;
    private float time = 0f;
    private Collider[] colliders;
    private Camera mainCamera;
    private bool isInvulnerable = false;

    private void Start()
    {
        mainCamera = Camera.main;
        colliders = GetComponents<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Always update thruster display position/rotation.
        thrusterDisplay.transform.position = transform.TransformPoint(thrusterLocalOffset);
        thrusterDisplay.transform.rotation = transform.rotation;

        // Note: Input is processed even if the ship is invulnerable.
        // Laser firing logic.
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
        // Gravity logic (unchanged).
        if (GameManager.instance.gravityActive)
        {
            AsteroidController[] asteroids = Object.FindObjectsByType<AsteroidController>(FindObjectsSortMode.None);
            Vector3 totalForce = Vector3.zero;
            foreach (AsteroidController a in asteroids)
            {
                if (!a.gameObject.tag.StartsWith("Asteroid"))
                    continue;
                Vector3 direction = a.transform.position - transform.position;
                float distance = direction.magnitude;
                if (distance < 0.1f)
                    distance = 0.1f;
                float forceMag = GameManager.instance.gravityConstant * (1f * 1f) / (distance * distance);
                totalForce += direction.normalized * forceMag;
            }
            rigidBody.AddForce(totalForce);
            Debug.Log("Applied gravitational force to spaceship: " + totalForce);
        }
    }

    // Dying mechanics: on collision with an asteroid.
    private void OnCollisionEnter(Collision collision)
    {
        if (!isInvulnerable && collision.gameObject.tag.StartsWith("Asteroid"))
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        isInvulnerable = true;

        // Play death sound.
        if (deathAudioClip != null)
            AudioSource.PlayClipAtPoint(deathAudioClip, transform.position);

        // Disable colliders to prevent further collisions.
        foreach (Collider col in colliders)
            col.enabled = false;

        // Reposition ship to the center of the screen.
        Vector3 center = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane));
        center.z = 0f;
        transform.position = center;

        // Clear any residual velocity.
        rigidBody.linearVelocity = Vector3.zero;

        // Beaming effect: ship fades out and in multiple times.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float cycleDuration = respawnDelay / beamCycles;
            float halfCycle = cycleDuration / 2f;

            for (int i = 0; i < beamCycles; i++)
            {
                // Fade out.
                for (float t = 0; t < halfCycle; t += Time.deltaTime)
                {
                    float alpha = Mathf.Lerp(1, 0, t / halfCycle);
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                    yield return null;
                }
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);

                // Fade in.
                for (float t = 0; t < halfCycle; t += Time.deltaTime)
                {
                    float alpha = Mathf.Lerp(0, 1, t / halfCycle);
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                    yield return null;
                }
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
            }
        }
        else
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        // Re-enable colliders.
        foreach (Collider col in colliders)
            col.enabled = true;

        isInvulnerable = false;
        yield break;
    }

    // Shop upgrade methods.
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
