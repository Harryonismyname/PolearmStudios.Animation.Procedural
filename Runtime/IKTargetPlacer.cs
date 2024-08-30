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
        [SerializeField] Transform parent;
        [SerializeField] CenterOfMass centerOfMass;
        [SerializeField] LayerMask groundMask;
        [SerializeField] IKTargetPlacerData data;

        [SerializeField] ProceduralStepData stepData;

        bool isGrounded;
        RaycastHit hit;
        float time;
        float distTraveled;
        float stepThreshold;
        float stepHeight;
        float speed;

        #region Complex Variables
        private Vector3 FootDestination { get 
            { 
                Vector3 adjustedHitPoint = hit.point + ( centerOfMass.Offset + (centerOfMass.MovementDirection * centerOfMass.CurrentSpeed));
                return adjustedHitPoint + (((stepData.StepLength) * (adjustedHitPoint - target.position).normalized) + (hit.normal * stepData.FootHeight)); 
            } 
        }
        private bool LongStep { get { return Vector3.Distance(FootDestination, target.position) > stepData.FarDistance && (opposite.isGrounded || centerOfMass.MovementDirection.magnitude > stepData.StepThreshold); } }
        private bool ShortStep { get { return Vector3.Distance(FootDestination, target.position) > stepData.NearDistance && (opposite.isGrounded || centerOfMass.MovementDirection.magnitude > stepData.StepThreshold); } }
        private bool IsRunning { get { return centerOfMass.CurrentSpeed > stepData.RunSpeedThreshold; } }
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
            void RandomizeVariables()
        {
            float Randomize() => Random.Range(-1, 1) * stepData.RandomizationOffset;
            stepData.StepSpeed += Randomize();
            stepData.NearDistance += Randomize();
            stepData.FarDistance += Randomize();
        }
            stepData = data.LoadData();
            transform.localPosition = new(transform.localPosition.x, stepData.LegLength * .5f, transform.localPosition.z);
            RandomizeVariables();
            speed = stepData.StepSpeed;
        }

        public void CheckForStep()
        {
            bool SpherecastInDirection(Vector3 direction)
            {
                bool Cast(Vector3 dir, float radius) => Physics.SphereCast(transform.position, radius, dir, out hit, stepData.LegLength, groundMask);
                return Cast(direction, stepData.FarDistance) || Cast(direction, stepData.NearDistance);
            }
            bool RaycastInDirection(Vector3 direction, float dist) => Physics.Raycast(transform.position, direction, out hit, dist, groundMask);
            // Checking Forward
            if (SpherecastInDirection(transform.forward))
            {
                PrepareStep();
                return;
            }
            // Checking Backwards
            if (RaycastInDirection(-transform.forward, stepData.LegLength))
            {
                PrepareStep();
                return;
            }
            // Checking Away From Opposite
            if (RaycastInDirection((transform.position - opposite.transform.position).normalized, stepData.LegLength * 0.5f))
            {
                PrepareStep();
                return;
            }
            // Checking Ground
            if (SpherecastInDirection(-transform.up))
            {
                PrepareStep();
                return;
            }
        }

        private void PrepareStep()
        {
            if (!ShortStep && !LongStep) return;
            target.up = hit.normal;
            /*if (centerOfMass.CurrentSpeed > stepData.RunSpeedThreshold)
            {
                speed = stepData.StepSpeed * stepData.RunSpeedThreshold;
                distTraveled = Vector3.Distance(FootDestination, target.position) * stepData.RunModifier;
                stepHeight = stepData.StepHeight;// * stepData.RunModifier;
                stepThreshold = stepData.StepThreshold * stepData.RunModifier;
                isGrounded = false;
                return;
            }*/
            stepThreshold = stepData.StepThreshold;
            stepHeight = stepData.StepHeight;
            speed = ShortStep ? stepData.StepSpeed : stepData.StepSpeed * stepData.LongStepSpeedModifier;
            distTraveled = Vector3.Distance(FootDestination, target.position) * (ShortStep ? stepData.ShortStepHeightModifier : 1);
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

        public void SetTargetParent(Transform newParent) => target.SetParent(newParent);

        private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            float f(float x) => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, .5f);
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo))
            {
                Gizmos.DrawLine(transform.position, hitInfo.point);
            }
            Gizmos.color = Color.green;
            if (!isGrounded) Gizmos.color = Color.blue;
            if (ShortStep) Gizmos.color = Color.yellow;
            if (LongStep) Gizmos.color = Color.red;
            Gizmos.DrawRay(target.position, centerOfMass.Offset);
            Gizmos.DrawCube(target.position, Vector3.one);
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(FootDestination, .5f);
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(hit.point, stepData.FarDistance);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(hit.point, stepData.NearDistance);
            if (!isGrounded) return;
            Gizmos.color = Color.magenta;
            for (float i = 0; i < Vector3.Distance(target.position, FootDestination); i += 0.1f)
            {
                Gizmos.DrawLine(Parabola(target.position, FootDestination, Mathf.Clamp(Vector3.Distance(target.position, FootDestination), 0, stepData.StepHeight), i), Parabola(target.position, FootDestination, Mathf.Clamp(Vector3.Distance(target.position, FootDestination), 0, stepData.StepHeight), i + 0.1f));
            }
        }
        private void OnValidate() 
        {
            if(opposite != null && opposite.opposite == null) opposite.opposite = this;
        }
    }

}