using UnityEngine;

// put this on anything that should splash when it falls into the water (toys and the cat)
// it watches for the object dropping through the water surface and spawns a splash effect there
public class WaterSplash : MonoBehaviour
{
    public WaterRise waterRise;

    [Header("Effects")]
    public GameObject splashPrefab;
    public GameObject bubblesPrefab;
    public float splashSize = 0.3f;

    [Header("Settings")]
    // how fast it has to be falling to splash, so bobbing on the water doesnt keep splashing
    public float minFallSpeed = 0f;
    public bool playSplashSound = true;

    [Header("Camera Hold (cat only)")]
    // the cat gets respawned straight away when it dies, so the camera would fly off before the splash is seen.
    // SplashAndHoldCamera stops the camera for a moment so the player sees the splash first
    public ThirdPersonCameraRig cameraRig;
    public float cameraHoldTime = 0.75f;

    private Rigidbody rb;
    private CharacterController controller;
    private AudioManager audioManager;

    private bool wasAboveWater = true;
    private float lastSplashTime = -10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        audioManager = FindFirstObjectByType<AudioManager>();

        if (waterRise == null)
        {
            waterRise = FindFirstObjectByType<WaterRise>();
        }
    }

    private void Update()
    {
        // no water in this scene
        if (waterRise == null)
        {
            return;
        }

        bool isAboveWater = transform.position.y > waterRise.waterLevel;

        // only splash when going from above the water to below it, while falling fast enough
        if (wasAboveWater && !isAboveWater && GetFallSpeed() >= minFallSpeed)
        {
            SpawnSplash();
        }

        wasAboveWater = isAboveWater;
    }

    // public so it can also be hooked up to events, eg. PlayerHealth's onPlayerMadeWet and onPlayerFullyKilled
    public void Splash()
    {
        if (waterRise == null)
        {
            return;
        }

        SpawnSplash();
    }

    // hook this to PlayerHealth's onPlayerMadeWet (the first death)
    // dont use it on onPlayerFullyKilled, PlayerDeath turns the camera off for the death screen
    public void SplashAndHoldCamera()
    {
        Splash();

        if (cameraRig != null)
        {
            cameraRig.enabled = false;
            Invoke("ReleaseCamera", cameraHoldTime);
        }
    }

    private void ReleaseCamera()
    {
        cameraRig.enabled = true;
    }

    private void SpawnSplash()
    {
        // stops a double splash when the fall and a death event happen at the same time
        if (Time.time - lastSplashTime < 0.5f)
        {
            return;
        }
        lastSplashTime = Time.time;

        Vector3 splashPosition = new Vector3(transform.position.x, waterRise.waterLevel, transform.position.z);

        if (splashPrefab != null)
        {
            GameObject splash = Instantiate(splashPrefab, splashPosition, Quaternion.identity);
            splash.transform.localScale = Vector3.one * splashSize;
            // the effects dont delete themselves, so clean them up once they've finished
            Destroy(splash, 3f);
        }

        if (bubblesPrefab != null)
        {
            GameObject bubbles = Instantiate(bubblesPrefab, splashPosition + Vector3.down * 0.2f, Quaternion.identity);
            bubbles.transform.localScale = Vector3.one * splashSize;
            Destroy(bubbles, 3f);
        }

        if (playSplashSound && audioManager != null)
        {
            audioManager.SplashSFX();
        }

        Debug.Log("Water Splash: " + name + " splashed");
    }

    // how fast the object is moving downwards
    private float GetFallSpeed()
    {
        if (rb != null)
        {
            return -rb.linearVelocity.y;
        }

        if (controller != null)
        {
            return -controller.velocity.y;
        }

        return 0f;
    }
}
