using UnityEngine;

public class ObjectHold : MonoBehaviour
{
    public Transform mouth;
    public float pickupRange = 2f;

    private Rigidbody heldObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                Pickup();
            else
                Drop();
        }
    }

    void Pickup()
    {
        // check for anything close to the mouth
        Collider[] hits = Physics.OverlapSphere(mouth.position, pickupRange);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;

            // only pick up toys
            if (rb == null || !rb.CompareTag("Toy"))
                continue;

            heldObject = rb;

            // turn off physics so it doesnt fall out of the mouth
            heldObject.isKinematic = true;
            hit.enabled = false;

            heldObject.transform.SetParent(mouth);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
            return;
        }
    }

    void Drop()
    {
        heldObject.transform.SetParent(null);
        heldObject.isKinematic = false;
        heldObject.GetComponentInChildren<Collider>(true).enabled = true;
        heldObject = null;
    }
}
