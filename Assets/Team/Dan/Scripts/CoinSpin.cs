using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    [Header("Spin")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float spinSpeed = 180f;

    [Header("Bounce")]
    [SerializeField] private float bounceHeight = 0.25f;
    [SerializeField] private float bounceSpeed = 3f;

    private Vector3 startLocalPos;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        transform.Rotate(rotationAxis.normalized * (spinSpeed * Time.deltaTime), Space.Self);

        float yOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.localPosition = startLocalPos + new Vector3(0f, yOffset, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            audioManager.CoinSFX();
        }
    }
}