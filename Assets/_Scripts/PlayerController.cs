using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed = 10f; // Maximum speed limit
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject[] cannons;
    //[SerializeField] private GameObject thruster;
    [SerializeField] private float cooldown = 1f;

    [SerializeField] private AudioClip fireAudioClip; // Laser fire sound effect

    // New thruster logic fields:
    [SerializeField] private GameObject thrusterDisplay; // GameObject that displays the thruster sprite
    [SerializeField] private Sprite[] thrusterSprites;     // Array of 5 thruster sprites (levels 1 to 5)
    [SerializeField] private float thrusterAccelerationTime = 1f; // Time required to reach the highest thruster level
    [SerializeField] private Vector3 thrusterLocalOffset = new Vector3(0, -1f, 0); // Custom local offset from ship's pivot for the thruster
    private float thrusterTimer = 0f;

    private Rigidbody rigidBody;
    private int currentCannon = 0;

    //private AudioManager audioManager;
    //private ParticleSystem thrusterParticles;

    private float time = 0f;
    private Collider[] colliders;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        colliders = GetComponents<Collider>();
        rigidBody = GetComponent<Rigidbody>();

        // Optionally adjust Rigidbody's drag in the Inspector.
        //audioManager = GameObject.FindGameObjectWithTag("AudioManager")?.GetComponent<AudioManager>();

        //thrusterParticles = thruster.GetComponent<ParticleSystem>();
        //thrusterParticles.Stop();
    }

    private void Update()
    {
        // Update the thruster display's position and rotation so that it's always
        // at the designated local offset relative to the ship's geometry.
        thrusterDisplay.transform.position = transform.TransformPoint(thrusterLocalOffset);
        thrusterDisplay.transform.rotation = transform.rotation;

        // Laser firing logic
        if (time > 0f)
        {
            time -= Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            // Make laser originate from alternate cannon on each shot
            var laserOriginTransform = transform;
            if (cannons.Length > 0)
            {
                laserOriginTransform = cannons[currentCannon++].transform;
                if (currentCannon >= cannons.Length)
                {
                    currentCannon = 0;
                }
            }

            // Spawn laser directly at the cannon's pivot point.
            Instantiate(laser, laserOriginTransform.position, laserOriginTransform.rotation);
            
            // Play the laser fire sound at the cannon's position.
            if (fireAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(fireAudioClip, laserOriginTransform.position);
            }
            
            time = cooldown;
            //audioManager?.PlaySfx(fireAudioClip);
        }

        // Player movement and thruster logic
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rigidBody.AddForce(transform.up * (movementSpeed * Time.deltaTime));
            // Increase thruster timer and update sprite based on forward acceleration.
            thrusterTimer += Time.deltaTime;
            thrusterTimer = Mathf.Clamp(thrusterTimer, 0f, thrusterAccelerationTime);
            int level = Mathf.FloorToInt((thrusterTimer / thrusterAccelerationTime) * thrusterSprites.Length);
            level = Mathf.Clamp(level, 0, thrusterSprites.Length - 1);
            thrusterDisplay.GetComponent<SpriteRenderer>().sprite = thrusterSprites[level];
        }
        else
        {
            // Reset thruster when not accelerating forward.
            thrusterTimer = 0f;
            thrusterDisplay.GetComponent<SpriteRenderer>().sprite = thrusterSprites[0];
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rigidBody.AddForce(transform.up * (-movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        // Clamp the spaceship's velocity to prevent it from exceeding maxSpeed.
        if (rigidBody.linearVelocity.magnitude > maxSpeed)
        {
            rigidBody.linearVelocity = rigidBody.linearVelocity.normalized * maxSpeed;
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
        //audioManager?.StopSfx(thrustAudioClip);
        //GameObject.FindWithTag("GameController")?.GetComponent<GameController>()?.GameOver();
    }
}
