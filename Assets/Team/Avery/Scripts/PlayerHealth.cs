using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    private MeshFilter playerMeshFilter;
    private MeshRenderer playerMeshRenderer;
    private CharacterController characterController;

    public Mesh wetMesh;
    public Material wetMat;

    [SerializeField] private float wetDamping = 0f;

    [SerializeField] private Transform spawnPoint;
    private int maxLifes = 2;
    private int wetLife = 2;
    public int currentLife = 1;

    public UnityEvent onPlayerMadeWet;
    public UnityEvent onPlayerFullyKilled;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point not found");
            }
        }

        playerMeshFilter = GetComponent<MeshFilter>();
        playerMeshRenderer = GetComponent<MeshRenderer>();
        characterController = GetComponent<CharacterController>();
    }

    public void Kill()
    {
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        audioManager.SplashSFX();

        if (currentLife >= maxLifes)
        {
            onPlayerFullyKilled?.Invoke();
            return;
        }
        else
        {
            currentLife++;
            MakeWet();
            Respawn();
            Debug.Log("Player has been killed. Current life: " + currentLife);
        }
    }

    public void Respawn()
    {
        if (spawnPoint == null) return;

        characterController.enabled = false;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        characterController.enabled = true;
    }

    public void MakeWet()
    {
        if (currentLife < wetLife)
        {
            return;
        }
        else
        {
            if (playerMeshFilter && wetMesh)
            {
                playerMeshFilter.mesh = wetMesh;
            }

            if (playerMeshRenderer && wetMat)
            {
                playerMeshRenderer.material = wetMat;
            }

            Debug.Log("Cat is now wet!");
            onPlayerMadeWet?.Invoke();
        }
    }
}
