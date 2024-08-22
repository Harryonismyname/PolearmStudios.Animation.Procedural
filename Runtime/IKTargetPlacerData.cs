using UnityEngine;

namespace PolearmStudios.Animation.Procedural
{
    [CreateAssetMenu(fileName = "NewProceduralAnimationPreset", menuName = "Scriptable Objects/Procedural Animation/Procedural Animation Preset" )]
    public class IKTargetPlacerData : ScriptableObject
    {
        [Header("Constraints")]
        [SerializeField] float farDistance;
        public float FarDistance { get; set; }
        [SerializeField] float nearDistance;
        public float NearDistance {  get; set; }
        [SerializeField] float legLength;
        public float LegLength {  get; set; }
        [SerializeField] float footHeight;
        public float FootHeight { get; set; }
        [Header("Step Data")]
        [SerializeField] float stepHeight;
        public float StepHeight {  get; set; }
        [SerializeField] float shortStepHeightModifier;
        public float ShortStepHeightModifier {  get; set; }
        [SerializeField] float stepSpeed;
        public float StepSpeed {  get; set; }
        [SerializeField] float stepLength;
        public float StepLength {  get; set; }
        [SerializeField] float stepThreshold;
        public float StepThreshold {  get; set; }
        [SerializeField] float randomizationOffset;
        public float RandomizationOffset {  get; set; }
        [SerializeField] float longStepSpeedModifier;
        public float LongStepSpeedModifier {  get; set; }

        public void LoadData()
        {
            FarDistance = farDistance;
            NearDistance = nearDistance;
            LegLength = legLength;
            FootHeight = footHeight;
            StepHeight = stepHeight;
            ShortStepHeightModifier = shortStepHeightModifier;
            StepSpeed = stepSpeed;
            StepLength = stepLength;
            StepThreshold = stepThreshold;
            RandomizationOffset = randomizationOffset;
            LongStepSpeedModifier = longStepSpeedModifier;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FarDistance = farDistance;
            NearDistance = nearDistance;
            LegLength = legLength;
            FootHeight = footHeight;
            StepHeight = stepHeight;
            ShortStepHeightModifier = shortStepHeightModifier;
            StepSpeed = stepSpeed;
            StepLength = stepLength;
            StepThreshold = stepThreshold;
            RandomizationOffset = randomizationOffset;
            LongStepSpeedModifier = longStepSpeedModifier;

            StepSpeed = StepSpeed > StepThreshold ? StepSpeed : StepThreshold;
            StepThreshold = StepThreshold < StepSpeed ? StepThreshold : StepSpeed;
            FarDistance = FarDistance > NearDistance ? FarDistance : NearDistance;
            NearDistance = NearDistance < FarDistance ? NearDistance : FarDistance;
        }
#endif
    }

}