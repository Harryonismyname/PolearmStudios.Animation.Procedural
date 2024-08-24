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
        [SerializeField] CenterOfMass centerOfMass;
        [SerializeField] LayerMask groundMask;
        [Header("Settings")]
        [SerializeField] Vector3 offset = Vector3.up;
        [SerializeField] float movementSpeed = 1.5f;
        [SerializeField] float mass = 15;

        Vector3 averagePosition;
        Vector3 averageUp;
        Vector3 desiredPosition;
        Vector3 previousPos;
        Quaternion desiredRotation;

        private Vector3 PrevBodyOffset { get { return (body.position - previousPos).normalized; } }


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
            desiredPosition = averagePosition + centerOfMass.Offset;
            desiredRotation = Quaternion.LookRotation(PrevBodyOffset.magnitude > 0 ? PrevBodyOffset : body.forward, averageUp);
        }

        private void LateUpdate()
        {
            //body.up = Vector3.MoveTowards(body.up, averageUp, Time.smoothDeltaTime * movementSpeed);

            body.SetPositionAndRotation(
                Vector3.MoveTowards(body.position, desiredPosition, Time.smoothDeltaTime * movementSpeed),
                Quaternion.Slerp(body.rotation, desiredRotation, movementSpeed * Time.smoothDeltaTime)
                );
            
            previousPos = body.position;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(averagePosition, 0.5f);
            if (Physics.Raycast(averagePosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                Gizmos.DrawLine(averagePosition, hit.point);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(desiredPosition, 0.5f);

        }
    }

}