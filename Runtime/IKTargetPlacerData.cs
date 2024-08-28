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
    [System.Serializable]
    public struct ProceduralStepData
    {
        public float FarDistance;
        public float NearDistance;
        public float LegLength;
        public float FootHeight;
        public float StepHeight;
        public float ShortStepHeightModifier;
        public float StepSpeed;
        public float StepLength;
        public float StepThreshold;
        public float RandomizationOffset;
        public float LongStepSpeedModifier;
        public float RunSpeedThreshold;
        public float RunModifier;
    }
}