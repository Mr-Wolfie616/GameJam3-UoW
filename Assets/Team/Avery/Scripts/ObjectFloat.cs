using System;
using UnityEngine;

namespace FishSlop
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectFloat : MonoBehaviour
    {
        [SerializeField] private Collider hullCollider;
        [SerializeField] private float restDepth = 0.5f;
        [SerializeField] private float damping = 0.4f;
        [SerializeField] private float torqueDamping = 0.5f;
        [SerializeField] private float horizontalDrag = 2f;

        private Rigidbody rb;
        private Vector3[] floatPoints = new Vector3[4];
        private float strength;
        private float dampingForce;
        private bool ready;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (hullCollider == null) hullCollider = GetComponent<Collider>();
            InitialiseBuoyancy();
        }

        private void InitialiseBuoyancy()
        {
            Physics.SyncTransforms();

            Bounds b = hullCollider.bounds;
            floatPoints[0] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.min.z));
            floatPoints[1] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.min.z));
            floatPoints[2] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.max.z));
            floatPoints[3] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.max.z));

            strength = rb.mass * Physics.gravity.magnitude / (floatPoints.Length * restDepth);
            dampingForce = damping * 2f * Mathf.Sqrt(strength * rb.mass / floatPoints.Length);

            ready = true;
        }

        private void FixedUpdate()
        {
            if (!ready || WaterWaves.Instance == null) return;

            float t = Time.time;

            foreach (Vector3 floatPoint in floatPoints)
            {
                Vector3 point = transform.TransformPoint(floatPoint);
                float depth = WaterWaves.Instance.GetHeight(point, t) - point.y;

                if (depth <= 0f) continue;

                Vector3 pointVelocity = rb.GetPointVelocity(point);

                float force = depth * strength - pointVelocity.y * dampingForce;

                if (force > 0f) rb.AddForceAtPosition(Vector3.up * force, point);

                float submersion = Mathf.Clamp01(depth / restDepth);

                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(pointVelocity, Vector3.up);
                Vector3 dragForce = horizontalDrag * submersion * -horizontalVelocity;

                rb.AddForceAtPosition(dragForce, point);

                if (Vector3.Dot(transform.up, Vector3.up) < 0.995f)
                {
                    Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
                    Debug.DrawRay(point, torque, Color.red);
                    rb.AddTorque(torque * torqueDamping);
                }
            }
        }
    }
}