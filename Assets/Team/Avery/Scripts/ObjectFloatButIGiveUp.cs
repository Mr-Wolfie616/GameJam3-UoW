using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ObjectFloat : MonoBehaviour
{
    [SerializeField] private WaterRise waterRise;
    private Transform player;

    [SerializeField] private float floatHeight = 1f;
    [SerializeField] private float followSpeed = 5f;

    [SerializeField] private Vector2 bobSpeedRange = new Vector2(0.8f, 1.4f);
    [SerializeField] private Vector2 bobAmountRange = new Vector2(0.05f, 0.15f);

    private float bobSpeed, bobAmount, bobPhase;
    private float currentY;

    private Vector3 startPos;
    public bool isFloating = false;
    public bool PlayerOn { get; set; }

    private void Awake()
    {
        bobSpeed = Random.Range(bobSpeedRange.x, bobSpeedRange.y);
        bobAmount = Random.Range(bobAmountRange.x, bobAmountRange.y);
        bobPhase = Random.Range(0f, Mathf.PI * 2f);
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        startPos = transform.position;
    }

    private void Start()
    {
        if (waterRise == null) waterRise = FindFirstObjectByType<WaterRise>();

        if (waterRise == null)
        {
            enabled = false;
            return;
        }

        currentY = waterRise.waterLevel + floatHeight;
        if (currentY < startPos.y)
        {
            return;
        }
        SetY(currentY);
    }

    private void Update()
    {   
        float baseY = waterRise.waterLevel + floatHeight;
        float targetY;

        if(transform.position.y <= baseY)
        {
            isFloating = true;
        }

        if (!isFloating) return;

        if (PlayerOn)
        {
            targetY = Mathf.Max(currentY, baseY);
        }
        else
        {
            float t = Time.time;
            float bob = (Mathf.Sin(t * bobSpeed + bobPhase) + 0.5f * Mathf.Sin(t * bobSpeed * 2.3f + bobPhase * 1.7f)) * bobAmount;
            targetY = baseY + bob;
        }

        currentY = Mathf.Lerp(currentY, targetY, 1f - Mathf.Exp(-followSpeed * Time.deltaTime));
        SetY(currentY);
    }

    private void SetY(float y)
    {
        Vector3 p = transform.position;
        p.y = y;
        transform.position = p;
        if (player != null)
        {
            Vector3 playerPos = player.position;
            float distance = Vector3.Distance(new Vector3(playerPos.x, 0f, playerPos.z), new Vector3(p.x, 0f, p.z));
            if (distance < 2f)
            {
                Physics.SyncTransforms();
            }
        }
    }
}