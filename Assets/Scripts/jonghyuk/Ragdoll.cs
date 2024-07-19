using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Quaternion modelRotation;


    private void Update()
    {
        modelRotation = transform.rotation;
    }

}
