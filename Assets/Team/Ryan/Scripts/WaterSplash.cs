using FishSlop;
using UnityEngine;

// put this on anything that should splash when it falls into the water (toys and the cat)
// it watches for the object dropping through the water surface and spawns a splash effect there
public class WaterSplash : MonoBehaviour
{
    [Header("Effects")]
    public GameObject splashPrefab;
    public GameObject bubblesPrefab;
    public float splashSize = 0.25f;

    [Header("Settings")]
    // how fast it has to be falling to splash, so bobbing on the waves doesnt keep splashing
    public float minFallSpeed = 1.5f;
    public bool playSplashSound = true;

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
    }

    private void Update()
    {
        // no water in this scene
        if (WaterWaves.Instance == null)
        {
            return;
        }

        float waterHeight = WaterWaves.Instance.GetHeight(transform.position, Time.time);
        bool isAboveWater = transform.position.y > waterHeight;

        // only splash when going from above the water to below it, while falling fast enough
        if (wasAboveWater && !isAboveWater && GetFallSpeed() >= minFallSpeed)
        {
            SpawnSplash(waterHeight);
        }

        wasAboveWater = isAboveWater;
    }

    // public so it can also be hooked up to events, eg. PlayerHealth's onPlayerMadeWet and onPlayerFullyKilled
    public void Splash()
    {
        if (WaterWaves.Instance == null)
        {
            return;
        }

        SpawnSplash(WaterWaves.Instance.GetHeight(transform.position, Time.time));
    }

    private void SpawnSplash(float waterHeight)
    {
        // stops a double splash when the fall and a death event happen at the same time
        if (Time.time - lastSplashTime < 0.5f)
        {
            return;
        }
        lastSplashTime = Time.time;

        Vector3 splashPosition = new Vector3(transform.position.x, waterHeight, transform.position.z);

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
