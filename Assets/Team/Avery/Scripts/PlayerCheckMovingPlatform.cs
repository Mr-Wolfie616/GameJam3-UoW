using FishSlop;
using UnityEngine;

public class PlayerCheckMovingPlatform : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.2f;
    [SerializeField] private LayerMask platformLayerMask;
    private ObjectFloat lastObjFloat;
    private void LateUpdate()
    {
        bool hitPlatform = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance, platformLayerMask);
        ObjectFloat current = hitPlatform ? hit.collider.GetComponentInParent<ObjectFloat>() : null;

        if (current == lastObjFloat) return;

        if (lastObjFloat) lastObjFloat.PlayerOn = false;
        if (current) current.PlayerOn = true;

        transform.SetParent(current ? GetCarrier(current.transform) : null, true);
        lastObjFloat = current;
    }
    private Transform GetCarrier(Transform platform)
    {
        Transform carrier = platform.Find("PlayerCarrier");
        if (!carrier)
        {
            carrier = new GameObject("PlayerCarrier").transform;
            carrier.SetParent(platform, false);
        }

        Vector3 s = platform.lossyScale;
        carrier.localPosition = Vector3.zero;
        carrier.localRotation = Quaternion.identity;
        carrier.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);
        return carrier;
    }
}