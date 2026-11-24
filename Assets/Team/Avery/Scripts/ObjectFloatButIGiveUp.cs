using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ObjectFloat : MonoBehaviour
{
    [SerializeField] private WaterRise waterRise;

    [SerializeField] private float floatHeight = 1f;
    [SerializeField] private float followSpeed = 5f;

    [SerializeField] private Vector2 bobSpeedRange = new Vector2(0.8f, 1.4f);
    [SerializeField] private Vector2 bobAmountRange = new Vector2(0.05f, 0.15f);

    private float bobSpeed, bobAmount, bobPhase;
    private float currentY;

    public bool PlayerOn { get; set; }

    private void Awake()
    {
        bobSpeed = Random.Range(bobSpeedRange.x, bobSpeedRange.y);
        bobAmount = Random.Range(bobAmountRange.x, bobAmountRange.y);
        bobPhase = Random.Range(0f, Mathf.PI * 2f);
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
        SetY(currentY);
    }

    private void Update()
    {
        float baseY = waterRise.waterLevel + floatHeight;
        float targetY;

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
        Physics.SyncTransforms();
    }
}