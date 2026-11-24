using UnityEngine;

// makes a toy float on Avery's rising water
// the toy uses normal physics until the water reaches it, then it bobs on the surface
// it does nothing while the cat is holding it (ObjectHold makes it kinematic)
[RequireComponent(typeof(Rigidbody))]
public class ToyFloat : MonoBehaviour
{
    public WaterRise waterRise;

    [Header("Floating")]
    // how far below the water surface the toy's centre sits (bigger = sits lower in the water)
    public float floatDepth = 0.05f;
    // how strongly the water pushes the toy back to the surface
    public float floatStrength = 20f;
    // slows the toy down in the water so it doesnt bounce around
    public float waterDrag = 3f;

    [Header("Bobbing")]
    public float bobAmount = 0.05f;
    public float bobSpeed = 1.5f;

    private Rigidbody rb;
    private float bobOffset;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (waterRise == null)
        {
            waterRise = FindFirstObjectByType<WaterRise>();
        }

        // give each toy a different starting point so they dont all bob together
        bobOffset = Random.Range(0f, 10f);
    }

    private void FixedUpdate()
    {
        // no water in the scene, or the cat is holding the toy
        if (waterRise == null || rb.isKinematic)
        {
            return;
        }

        float bob = Mathf.Sin((Time.time + bobOffset) * bobSpeed) * bobAmount;
        float surface = waterRise.waterLevel + bob;

        // still above the water, so let gravity do its thing
        if (transform.position.y > surface)
        {
            return;
        }

        // cancel out gravity while in the water
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);

        // push the toy towards its floating height
        float targetHeight = surface - floatDepth;
        float distance = targetHeight - transform.position.y;
        rb.AddForce(Vector3.up * distance * floatStrength, ForceMode.Acceleration);

        // water slows everything down
        rb.AddForce(-rb.linearVelocity * waterDrag, ForceMode.Acceleration);
    }
}
