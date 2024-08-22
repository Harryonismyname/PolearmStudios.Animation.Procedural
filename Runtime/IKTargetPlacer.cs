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
        [SerializeField] IKTargetPlacerData data;
        float stepSpeed;
        float farDistance;
        float nearDistance;

        bool isGrounded;
        RaycastHit hit;
        float time;
        float distTraveled;
        float speed;

        #region Complex Variables
        private Vector3 FootDestination { get { return hit.point + (((data.StepLength) * (hit.point - target.position).normalized) + (hit.normal * data.FootHeight)); } }
        private Vector3 Outward { get { return (transform.position - opposite.transform.position).normalized; } }
        private Vector3 Down => -transform.up;
        private Vector3 Forward => transform.forward;
        private Vector3 Backward => -transform.forward;
        private bool LongStep { get { return Vector3.Distance(FootDestination, target.position) > farDistance && opposite.isGrounded; } }
        private bool ShortStep { get { return Vector3.Distance(FootDestination, target.position) > nearDistance && opposite.isGrounded; } }
        #endregion

        #region Public Variables
        public Vector3 TargetPos { get => target.position; }
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
            data.LoadData();
            stepSpeed = data.StepSpeed;
            nearDistance = data.NearDistance;
            farDistance = data.FarDistance;
            transform.localPosition = new(transform.localPosition.x, data.LegLength * .5f, transform.localPosition.z);
            RandomizeVariables();
            speed = stepSpeed;
        }

        private void RandomizeVariables()
        {
            stepSpeed += Random.Range(-1, 1) * data.RandomizationOffset;
            nearDistance += Random.Range(-1, 1) * data.RandomizationOffset;
            farDistance += Random.Range(-1, 1) * data.RandomizationOffset;
        }

        public void CheckForStep()
        {
            if (SpherecastInDirection(Forward)) return;                         // Checking Forward

            if (RaycastInDirection(Backward, data.LegLength)) return;           // Checking Backwards

            if (RaycastInDirection(Outward, data.LegLength * 0.5f)) return;     // Checking Away From Opposite

            if (SpherecastInDirection(Down)) return;                            // Checking Ground
        }

        private bool SpherecastInDirection(Vector3 direction)
        {
            bool Cast(Vector3 dir, float radius) => Physics.SphereCast(transform.position, radius, dir, out hit, data.LegLength, groundMask);

            if (Cast(direction, farDistance) || Cast(direction, nearDistance))
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
            speed = ShortStep ? stepSpeed : stepSpeed * data.LongStepSpeedModifier;
            distTraveled = Vector3.Distance(FootDestination, target.position) * (ShortStep ? data.ShortStepHeightModifier : 1);
            isGrounded = false;
        }

        private void PerformStep()
        {
            time += Time.fixedDeltaTime * speed;
            target.position = Parabola(target.position, FootDestination, Mathf.Clamp(distTraveled, 0, data.StepHeight), time);
            if (Vector3.Distance(FootDestination, target.position) < data.StepThreshold)
            {
                time = 0;
                isGrounded = true;
            }
        }

        public void SetTargetParent(Transform newParent) => target.SetParent(newParent);

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
            if (!isGrounded) return;
            Gizmos.color = Color.magenta;
            for (float i = 0; i < Vector3.Distance(target.position, FootDestination); i += 0.1f)
            {
                Gizmos.DrawLine(Parabola(target.position, FootDestination, Mathf.Clamp(Vector3.Distance(target.position, FootDestination), 0, data.StepHeight), i), Parabola(target.position, FootDestination, Mathf.Clamp(Vector3.Distance(target.position, FootDestination), 0, data.StepHeight), i + 0.1f));
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