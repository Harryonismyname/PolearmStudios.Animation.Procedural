using UnityEngine;

namespace PolearmStudios.Animation.Procedural
{
    [CreateAssetMenu(fileName = "NewProceduralAnimationPreset", menuName = "Scriptable Objects/Procedural Animation/Procedural Animation Preset" )]
    public class IKTargetPlacerData : ScriptableObject
    {
        [Header("Constraints")]
        [SerializeField] float farDistance;
        [SerializeField] float nearDistance;
        [SerializeField] float legLength;
        [SerializeField] float footHeight;
        [Header("Step Data")]
        [SerializeField] float stepHeight;
        [SerializeField] float shortStepHeightModifier;
        [SerializeField] float stepSpeed;
        [SerializeField] float stepLength;
        [SerializeField] float stepThreshold;
        [SerializeField] float randomizationOffset;
        [SerializeField] float longStepSpeedModifier;
        [SerializeField] float runSpeedThreshold;
        [SerializeField] float runModifier;

        public ProceduralStepData LoadData()
        {
            ProceduralStepData stepData = new()
            {
                FarDistance = farDistance,
                NearDistance = nearDistance,
                LegLength = legLength,
                FootHeight = footHeight,
                StepHeight = stepHeight,
                ShortStepHeightModifier = shortStepHeightModifier,
                StepSpeed = stepSpeed,
                StepLength = stepLength,
                StepThreshold = stepThreshold,
                RandomizationOffset = randomizationOffset,
                LongStepSpeedModifier = longStepSpeedModifier,
                RunSpeedThreshold = runSpeedThreshold,
                RunModifier = runModifier,
            };
            return stepData;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            stepSpeed = stepSpeed > stepThreshold ? stepSpeed : stepThreshold;
            stepThreshold = stepThreshold < stepSpeed ? stepThreshold : stepSpeed;
            farDistance = farDistance > nearDistance ? farDistance : nearDistance;
            nearDistance = nearDistance < farDistance ? nearDistance : farDistance;
        }
#endif
    }
    public struct ProceduralStepData
    {
        public float FarDistance { get; set; }
        public float NearDistance { get; set; }
        public float LegLength { get; set; }
        public float FootHeight { get; set; }
        public float StepHeight { get; set; }
        public float ShortStepHeightModifier { get; set; }
        public float StepSpeed { get; set; }
        public float StepLength { get; set; }
        public float StepThreshold { get; set; }
        public float RandomizationOffset { get; set; }
        public float LongStepSpeedModifier { get; set; }
        public float RunSpeedThreshold { get; set; }
        public float RunModifier { get; set; }
    }
}