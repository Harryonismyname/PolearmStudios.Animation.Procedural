using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolearmStudios.Animation.Procedural
{
    [SelectionBase]
    public class IKBodyController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] IKTargetPlacer[] legs;
        [SerializeField] Transform body;
        [SerializeField] LayerMask groundMask;
        [Header("Settings")]
        [SerializeField] Vector3 offset = Vector3.up;
        [SerializeField] float movementSpeed = 1.5f;

        Vector3 averagePosition;
        Vector3 averageUp;

        private void Start()
        {
            foreach (var le in legs)
            {
                le.SetTargetParent(transform.parent);
                le.CheckForStep();
            }
        }

        private void FixedUpdate()
        {
            CalculateAverages();
            if (Physics.Raycast(body.position, -body.up, out RaycastHit hit, offset.y, groundMask))
            {
                averagePosition += hit.normal * offset.y;
                return;
            }
            averagePosition += offset;
        }

        private void LateUpdate()
        {
            body.position = Vector3.MoveTowards(body.position, averagePosition, Time.smoothDeltaTime * movementSpeed);
            body.up = Vector3.MoveTowards(body.up, averageUp, Time.smoothDeltaTime * movementSpeed);
            body.Rotate(0, -Vector3.SignedAngle(transform.forward, body.forward, transform.up), 0);
        }

        private void CalculateAverages()
        {
            averagePosition = Vector3.zero;
            averageUp = Vector3.zero;
            foreach (var le in legs)
            {
                if (le.SkipForAverage) continue;
                averagePosition += le.TargetPos;
                averageUp += (body.position - le.TargetPos).normalized;
            }
            averagePosition /= legs.Length;
            averageUp /= legs.Length;
        }
    }

}