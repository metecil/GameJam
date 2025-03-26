using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject[] cannons;
    //[SerializeField] private GameObject thruster;
    [SerializeField] private float cooldown = 1f;

    //[SerializeField] private AudioClip thrustAudioClip;
    //[SerializeField] private AudioClip fireAudioClip;

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

        //audioManager = GameObject.FindGameObjectWithTag("AudioManager")?.GetComponent<AudioManager>();

        //thrusterParticles = thruster.GetComponent<ParticleSystem>();
        //thrusterParticles.Stop();
    }

    private void Update()
    {
        // Laser
        if (time > 0f)
        {
            time -= Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            // Make laser originate from alternate canon on each shot
            var laserOriginTransform = transform;
            if (cannons.Length > 0)
            {
                laserOriginTransform = cannons[currentCannon++].transform;

                if (currentCannon >= cannons.Length)
                {
                    currentCannon = 0;
                }
            }

            // Spawn laser directly at the canon's pivot point.
            Instantiate(laser, laserOriginTransform.position, laserOriginTransform.rotation);
            time = cooldown;

            //audioManager?.PlaySfx(fireAudioClip);
        }

        // Player movements
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            //audioManager?.PlaySfx(thrustAudioClip, true);
            rigidBody.AddForce(transform.up * (movementSpeed * Time.deltaTime));
            //if (!thrusterParticles.isPlaying)
            //{
            //    thrusterParticles.Play();
            //}
        }
        else
        {
            //thrusterParticles.Stop();
            //audioManager?.StopSfx(thrustAudioClip);
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

        // Move through screen borders using viewport wrapping
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool wrapped = false;

        if (viewportPos.x < 0f)
        {
            viewportPos.x = 1f;
            wrapped = true;
        }
        else if (viewportPos.x > 1f)
        {
            viewportPos.x = 0f;
            wrapped = true;
        }
        if (viewportPos.y < 0f)
        {
            viewportPos.y = 1f;
            wrapped = true;
        }
        else if (viewportPos.y > 1f)
        {
            viewportPos.y = 0f;
            wrapped = true;
        }

        if (wrapped)
        {
            Vector3 newWorldPos = mainCamera.ViewportToWorldPoint(viewportPos);
            newWorldPos.z = 0f;  // Ensure Z remains 0 in a 2D game.
            transform.position = newWorldPos;
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

    public void IncreaseMovementSpeed(float amount)
    {
        movementSpeed += amount;
        rotationSpeed += amount;
        Debug.Log("New movementSpeed: " + movementSpeed);
    }


    public void DecreaseLaserCooldown(float amount)
    {
        cooldown = Mathf.Max(0.1f, cooldown - amount);
    }

}
}
