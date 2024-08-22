using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolearmStudios.Animation.Procedural
{
    public class IKTargetPlacer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] IKTargetPlacer opposite;
        [SerializeField] Transform target;
        [SerializeField] LayerMask groundMask;
        [Header("Constraints")]
        [SerializeField] float farDistance = 2;
        [SerializeField] float nearDistance = 2;
        [SerializeField] float legLength = 9;
        [SerializeField] float footHeight = 2;
        [Header("Step Data")]
        [SerializeField] float stepHeight = .5f;
        [SerializeField] float stepSpeed = 1.5f;
        [SerializeField] float stepSpeedOffset = .25f;
        [SerializeField] float stepLength = 1.5f;
        [SerializeField] float stepThreshold = .1f;

        bool isGrounded;
        RaycastHit hit;
        float time;
        float distTraveled;
        float speed;

        #region Complex Variables
        private Vector3 FootDestination { get { return hit.point + (((stepLength) * (hit.point - target.position).normalized) + (hit.normal * footHeight)); } }
        private Vector3 Outward { get { return (transform.position - opposite.transform.position).normalized; } }
        private Vector3 Down => -transform.up;
        private Vector3 Forward => transform.forward;
        private Vector3 Backward => -transform.forward;
        private bool LongStep { get { return Vector3.Distance(FootDestination, target.position) > farDistance && opposite.isGrounded; } }
        private bool ShortStep { get { return Vector3.Distance(FootDestination, target.position) > nearDistance && opposite.isGrounded; } }
        #endregion

        #region Public Variables
        public Vector3 TargetPos { get => target.position; }
        public Vector3 DestinationNormal { get => hit.normal; }
        public bool SkipForAverage;
        #endregion

        private void Awake() => Initialize();

        private void FixedUpdate()
        {
            if (!isGrounded)
            {
                PerformStep();
                return;
            }
            CheckForStep();
        }

        private void Initialize()
        {
            transform.localPosition = new(transform.localPosition.x, legLength * .5f, transform.localPosition.z);
            RandomizeVariables();
            speed = stepSpeed;
        }

        private void RandomizeVariables()
        {
            stepSpeed += Random.Range(-1, 1) * stepSpeedOffset;
            nearDistance += Random.Range(-1, 1) * stepSpeedOffset;
            farDistance += Random.Range(-1, 1) * stepSpeedOffset;
        }

        public void CheckForStep()
        {
            if (SpherecastInDirection(Forward)) return;                     // Checking Forward

            if (RaycastInDirection(Backward, legLength)) return;            // Checking Backwards

            if (RaycastInDirection(Outward, legLength * 0.5f)) return;      // Checking Away From Opposite

            if (SpherecastInDirection(Down)) return;                        // Checking Ground
        }

        private bool SpherecastInDirection(Vector3 direction)
        {
            bool CheckHit(Vector3 dir, float radius)
            {
                return Physics.SphereCast(transform.position, radius, dir, out hit, legLength, groundMask);
            }

            if (CheckHit(direction, farDistance) || CheckHit(direction, nearDistance))
            {
                PrepareStep();
                return true;
            }
            return false;
        }

        private bool RaycastInDirection(Vector3 direction, float dist)
        {
            if (Physics.Raycast(transform.position, direction, out hit, dist, groundMask))
            {
                PrepareStep();
                return true;
            }
            return false;
        }

        private void PrepareStep()
        {
            if (!ShortStep && !LongStep) return;
            target.up = hit.normal;
            speed = ShortStep ? stepSpeed : stepSpeed * 1.25f;
            distTraveled = Vector3.Distance(FootDestination, target.position) * (ShortStep ? .1f : 1);
            isGrounded = false;
        }

        private void PerformStep()
        {
            time += Time.fixedDeltaTime * speed;
            target.position = Parabola(target.position, FootDestination, Mathf.Clamp(distTraveled, 0, stepHeight), time);
            if (Vector3.Distance(FootDestination, target.position) < stepThreshold)
            {
                time = 0;
                isGrounded = true;
            }
        }

        public void SetTargetParent(Transform newParent)
        {
            target.SetParent(newParent);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (!isGrounded) Gizmos.color = Color.blue;
            if (ShortStep) Gizmos.color = Color.yellow;
            if (LongStep) Gizmos.color = Color.red;

            Gizmos.DrawCube(target.position, Vector3.one);
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(FootDestination, .5f);
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(hit.point, farDistance);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(hit.point, nearDistance);
            Gizmos.color = Color.magenta;
            for (float i = 0; i < Vector3.Distance(target.position, FootDestination); i += 0.1f)
            {
                Gizmos.DrawLine(Parabola(target.position, FootDestination, Mathf.Clamp(Vector3.Distance(target.position, FootDestination), 0, stepHeight), i), Parabola(target.position, FootDestination, Mathf.Clamp(Vector3.Distance(target.position, FootDestination), 0, stepHeight), i + 0.1f));
            }
        }




        private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            float f(float x) => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }
}