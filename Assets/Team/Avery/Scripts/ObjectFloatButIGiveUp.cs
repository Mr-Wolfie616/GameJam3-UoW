using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ObjectFloat : MonoBehaviour
{
    [SerializeField] private WaterRise waterRise;
    private Transform player;

    [SerializeField] private float floatHeight = 0f;
    [SerializeField] private float followSpeed = 5f;
	[SerializeField] private float ceilingNegativeOffset = 0.1f;
	
	[SerializeField] private Transform ceiling;
	private float ceilingHeight = 6f;

    [SerializeField] private Vector2 bobSpeedRange = new Vector2(0.8f, 1.4f);
    [SerializeField] private Vector2 bobAmountRange = new Vector2(0.05f, 0.15f);

    private float bobSpeed, bobAmount, bobPhase;
    private float currentY;
	private float maxY;

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
		currentY = startPos.y;
		
		if (ceiling != null) {
			ceilingHeight = ceiling.GetComponent<Collider>().bounds.min.y - GetComponentInChildren<Collider>(true).bounds.max.y;
			ceilingHeight -= ceilingNegativeOffset;
		}
    }

    private void Start()
    {
        if (waterRise == null) waterRise = FindFirstObjectByType<WaterRise>();

        if (waterRise == null)
        {
            enabled = false;
            return;
        }
		
		startPos = transform.position;
		currentY = startPos.y;
		maxY = startPos.y + ceilingHeight;

		isFloating = false;
    }

    private void Update()
    {   
        float baseY = waterRise.waterLevel + floatHeight;
        float targetY;

        if (!isFloating){
		if (waterRise.waterLevel < startPos.y) { return; }
			
			isFloating = true;
			currentY = transform.position.y;
		};

        if (PlayerOn)
        {
            targetY = Mathf.Min(Mathf.Max(currentY, baseY), maxY);
        }
        else
        {
            float t = Time.time;
            float bob = (Mathf.Sin(t * bobSpeed + bobPhase) + 0.5f * Mathf.Sin(t * bobSpeed * 2.3f + bobPhase * 1.7f)) * bobAmount;
            targetY = Mathf.Min(baseY + bob, maxY);
        }

        currentY = Mathf.Lerp(currentY, targetY, 1f - Mathf.Exp(-followSpeed * Time.deltaTime));
		currentY = Mathf.Min(currentY, maxY);
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