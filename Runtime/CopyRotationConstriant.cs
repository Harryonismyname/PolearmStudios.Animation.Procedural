using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotationConstriant : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] float weight;
    [SerializeField] Transform target;
    [SerializeField] Transform source;

    private void FixedUpdate()
    {
        source.rotation = Quaternion.Slerp(source.rotation, target.rotation, weight);
    }
}
