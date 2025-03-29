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

    // Optional: Coin prefab to spawn when an asteroid is destroyed while invincible.
    [SerializeField] private GameObject coinPrefab;

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
    [SerializeField] private PowerUpPanelUI powerUpPanelUI;
    private Rigidbody rigidBody;
    private int currentCannon = 0;
    private float time = 0f;
    private Collider[] colliders;
    private Camera mainCamera;
    private bool isInvulnerable = false;

    // --- POWER-UP FIELDS ---
    // Laser Spray Power-Up.
    private bool laserSprayUnlocked = false;
    public bool HasLaserSprayPowerUp { get { return laserSprayUnlocked; } }

    // Invincibility Power-Up.
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite invincibleSprite;

    private bool invincibilityUnlocked = false;
    public bool HasInvincibilityPowerUp { get { return invincibilityUnlocked; } }

    // Convenience property to check if any power-up is saved.
    public bool HasAnyPowerUp { get { return laserSprayUnlocked || invincibilityUnlocked; } }

    // Flags for the invincibility effect.
    private bool isInvincibleActive = false;

    // For boosting speed and rotation during invincibility.
    private float originalMovementSpeed;
    private float originalRotationSpeed;

    // To prevent push-back when invincible, we store the last known velocity.
    private Vector3 lastNonCollisionVelocity;

    private void Start()
    {
        mainCamera = Camera.main;
        colliders = GetComponents<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        originalMovementSpeed = movementSpeed;
        originalRotationSpeed = rotationSpeed;
    }

    private void Update()
    {
        // Always update thruster display position/rotation.
        thrusterDisplay.transform.position = transform.TransformPoint(thrusterLocalOffset);
        thrusterDisplay.transform.rotation = transform.rotation;

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
        
        // Use F key to trigger the saved power-up (if any).
        if (Input.GetKeyDown(KeyCode.F) && HasAnyPowerUp)
        {
            if (laserSprayUnlocked)
            {
                ActivateLaserSpray();
                laserSprayUnlocked = false;
            }
            else if (invincibilityUnlocked)
            {
                StartCoroutine(InvincibilityCoroutine());
                invincibilityUnlocked = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Store the current velocity.
        lastNonCollisionVelocity = rigidBody.linearVelocity;

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
        if (collision.gameObject.tag.StartsWith("Asteroid"))
        {
            if (isInvincibleActive)
            {
                // When invincible, trigger the asteroid's explosion and coin drop if possible,
                // then destroy the asteroid and reset the ship's velocity.
                AsteroidController asteroidCtrl = collision.gameObject.GetComponent<AsteroidController>();
                if (asteroidCtrl != null)
                {
                    asteroidCtrl.TriggerExplosion();
                }
                if (coinPrefab != null)
                {
                    Instantiate(coinPrefab, collision.transform.position, Quaternion.identity);
                }
                Destroy(collision.gameObject);
                rigidBody.linearVelocity = lastNonCollisionVelocity;
                return;
            }
            if (!isInvulnerable)
            {
                StartCoroutine(HandleDeath());
            }
        }
    }

    private IEnumerator HandleDeath()
    {
        isInvulnerable = true;

        if (deathAudioClip != null)
            AudioSource.PlayClipAtPoint(deathAudioClip, transform.position);

        foreach (Collider col in colliders)
            col.enabled = false;

        Vector3 center = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane));
        center.z = 0f;
        transform.position = center;

        rigidBody.linearVelocity = Vector3.zero;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float cycleDuration = respawnDelay / beamCycles;
            float halfCycle = cycleDuration / 2f;

            for (int i = 0; i < beamCycles; i++)
            {
                for (float t = 0; t < halfCycle; t += Time.deltaTime)
                {
                    float alpha = Mathf.Lerp(1, 0, t / halfCycle);
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                    yield return null;
                }
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);

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

    public void ActivateLaserSpray(float duration = 5f, float sprayInterval = 0.5f)
    {
        StartCoroutine(LaserSprayCoroutine(duration, sprayInterval));
    }

    private IEnumerator LaserSprayCoroutine(float duration, float interval)
    {
        float elapsed = 0f;
        powerUpPanelUI.HidePowerUpPanel();
        while (elapsed < duration)
        {
            for (int angle = 0; angle < 360; angle += 45)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Instantiate(laser, transform.position, rotation);
            }
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    public void UnlockLaserSpray()
    {
        // Only unlock if no other power-up is already saved.
        if (!HasAnyPowerUp)
            laserSprayUnlocked = true;
        else
            Debug.Log("Another power-up is already saved.");
    }

    public void UnlockInvincibility()
    {
        // Only unlock if no other power-up is already saved.
        if (!HasAnyPowerUp)
            invincibilityUnlocked = true;
        else
            Debug.Log("Another power-up is already saved.");
    }

    private IEnumerator InvincibilityCoroutine(float duration = 15f, float flashInterval = 0.2f)
    {
        powerUpPanelUI.HidePowerUpPanel();
        // Set both invincibility flags.
        isInvincibleActive = true;
        isInvulnerable = true;

        // Boost the player's speed and rotation.
        float boostFactor = 1.5f;
        float tempMovementSpeed = movementSpeed;
        float tempRotationSpeed = rotationSpeed;
        movementSpeed *= boostFactor;
        rotationSpeed *= boostFactor;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        bool toggle = false;
        while (elapsed < duration)
        {
            sr.sprite = toggle ? invincibleSprite : normalSprite;
            toggle = !toggle;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }
        sr.sprite = normalSprite;

        // Restore the player's speed and rotation.
        movementSpeed = tempMovementSpeed;
        rotationSpeed = tempRotationSpeed;

        // Reset invincibility flags.
        isInvincibleActive = false;
        isInvulnerable = false;
        yield break;
    }
    public bool IsInvincibleActive
    {
        get { return isInvincibleActive; }
    }

}
