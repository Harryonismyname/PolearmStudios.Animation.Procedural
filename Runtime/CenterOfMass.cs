using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    [SerializeField] Transform root;
    [SerializeField] Transform body;
    [SerializeField] WeightDistributionNode[] Nodes;
    [SerializeField] LayerMask groundMask;

    float sumMass;
    Vector3 massWeightedPosition;

    public Vector3 Offset { get { return root.position - transform.position; } }
    public Vector3 PreviousPosition { get; private set; }
    public Vector3 MovementDirection { get { return transform.position - PreviousPosition; } }
    public float CurrentSpeed { get { return Vector3.Distance(transform.position, PreviousPosition); } }


    private void FixedUpdate()
    {
        FindCenterOfMass();
        transform.position = massWeightedPosition;
        PreviousPosition = transform.position;
    }

    private void FindCenterOfMass()
    {
        massWeightedPosition = body.position * Nodes[0].Weight;
        sumMass = Nodes[0].Weight;
        foreach (var node in Nodes)
        {
            massWeightedPosition += node.transform.position * node.Weight;
            sumMass += node.Weight;
        }
        massWeightedPosition /= sumMass;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
        Gizmos.DrawRay(transform.position, Offset);
        Gizmos.DrawLine(transform.position, PreviousPosition);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }
}
