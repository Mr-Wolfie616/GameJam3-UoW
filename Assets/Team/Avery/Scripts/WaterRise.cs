using UnityEngine;

public class WaterRise : MonoBehaviour
{
    public bool isRising = true;
    [SerializeField] private float riseSpeed = 0.5f;
    [SerializeField] private float minHeight = 0f;
    [SerializeField] private float maxHeight = 10f;
    public float waterLevel;

    private void Awake()
    {
        if (transform.position.y < minHeight) {
            minHeight = transform.position.y;
        }
        Vector3 startPos = new Vector3(transform.position.x, minHeight, transform.position.z);
        transform.position = startPos;
    }

    private void Update()
    {
        if (!isRising) { return; }
        if (transform.position.y < maxHeight)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            waterLevel = transform.position.y;
        }
        else
        {
            Debug.Log("Water has reached max height.");
            isRising = false;
        }
    }

    public void StartRising()
    {
        isRising = true;
    }

    public void StopRising()
    {
        isRising = false;
    }
}
