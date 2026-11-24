//using System;
//using UnityEngine;

//namespace FishSlop
//{
//    [RequireComponent(typeof(Rigidbody))]
//    public class ObjectFloat : MonoBehaviour
//    {
//        [SerializeField] private Collider hullCollider;
//        [SerializeField] private float restDepth = 0.5f;
//        [SerializeField] private float damping = 0.4f;
//        [SerializeField] private float torqueDamping = 0.5f;
//        [SerializeField] private float horizontalDrag = 2f;

//        public Transform playerTrans;
//        private Rigidbody rb;
//        private Vector3[] floatPoints = new Vector3[4];
//        private float strength;
//        private float dampingForce;
//        private bool ready;
//        private float cacheTorqueDamping;

//        private float verticalOffset;

//        private bool playerOnBoard = false;

//        private Vector3 playerScale;
//        public Transform childAnchor;

//        private void Awake()
//        {
//            playerScale = playerTrans != null ? playerTrans.localScale : Vector3.one;
//            rb = GetComponent<Rigidbody>();
//            if (hullCollider == null) hullCollider = GetComponent<Collider>();
//            InitialiseBuoyancy();
//            cacheTorqueDamping = torqueDamping;

//            if (playerTrans == null)
//            {
//                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//                if (playerObj != null)
//                {
//                    playerTrans = playerObj.transform;
//                }
//            }

//            GameObject anchorObj = new GameObject($"{transform.name}_Anchor");
//            anchorObj.transform.position = transform.position;
//            anchorObj.transform.rotation = transform.rotation;
//            childAnchor = anchorObj.transform;

//            if (childAnchor != null)
//            {
//                childAnchor.parent = null;
//            }
//        }

//        private void Update()
//        {
//            if (playerOnBoard && WaterWaves.Instance != null)
//            {
//                Vector3 pos = transform.position;
//                float targetY = WaterWaves.Instance.GetHeight(pos, Time.time) + restDepth + (verticalOffset / 2);
//                float smoothY = Mathf.Lerp(pos.y, targetY, 0.15f);
//                transform.position = new Vector3(pos.x, smoothY, pos.z);

//                Quaternion targetRot = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
//                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.15f);

//                childAnchor.transform.position = new Vector3(pos.x, smoothY, pos.z); ;
//                childAnchor.transform.rotation = transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.15f); ;
//            }
//        }

//        private void InitialiseBuoyancy()
//        {
//            Physics.SyncTransforms();

//            Bounds b = hullCollider.bounds;
//            verticalOffset = b.min.y - transform.position.y;
//            floatPoints[0] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.min.z));
//            floatPoints[1] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.min.z));
//            floatPoints[2] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.max.z));
//            floatPoints[3] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.max.z));

//            strength = rb.mass * Physics.gravity.magnitude / (floatPoints.Length * restDepth);
//            dampingForce = damping * 2f * Mathf.Sqrt(strength * rb.mass / floatPoints.Length);

//            ready = true;
//        }

//        private void FixedUpdate()
//        {
//            if (!ready || WaterWaves.Instance == null) return;

//            float t = Time.time;

//            if (playerOnBoard)
//            {
//                rb.isKinematic = true;
//                rb.useGravity = false;
//                return;
//            }
//            else
//            {
//                rb.useGravity = true;
//                rb.isKinematic = false;
//            }

//            foreach (Vector3 floatPoint in floatPoints)
//            {
//                Vector3 point = transform.TransformPoint(floatPoint);
//                float waterHeight = WaterWaves.Instance.GetHeight(point, t);
//                float depth = waterHeight - point.y;

//                if (depth <= 0f) continue;

//                Vector3 pointVelocity = rb.GetPointVelocity(point);
//                float force = depth * strength - pointVelocity.y * dampingForce;

//                if (force > 0f) rb.AddForceAtPosition(Vector3.up * force, point);

//                float submersion = Mathf.Clamp01(depth / restDepth);
//                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(pointVelocity, Vector3.up);
//                Vector3 dragForce = horizontalDrag * submersion * -horizontalVelocity;

//                rb.AddForceAtPosition(dragForce, point);

//                float uprightDot = Vector3.Dot(transform.up, Vector3.up);
//                if (uprightDot < 0.995f)
//                {
//                    Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
//                    rb.AddTorque(torque * torqueDamping);
//                }
//            }
//        }

//        public void PlayerOnBoard(bool a, Transform p)
//        {
//            Debug.Log($"PlayerOnBoard called with value: {a}");
//            playerOnBoard = a;
//            playerTrans = p;
//        }
//    }
//}