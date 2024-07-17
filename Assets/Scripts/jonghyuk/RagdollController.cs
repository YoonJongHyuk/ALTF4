using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator anim = null;
    private Collider[] cols = null;
    private Rigidbody[] rbs = null;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        cols = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EnabledRagdoll(true);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            EnabledRagdoll(false);
        }

        if (IsRagdoll())
        {
            if (Input.GetMouseButtonDown(0))
            {
                HitRagdoll();
            }
        }
    }
    private void EnabledRagdoll(bool _isEnabled)
    {
        anim.enabled = !_isEnabled;
        foreach (Collider col in cols)
        {
            col.enabled = _isEnabled;
        }
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = _isEnabled;
            rb.isKinematic = !_isEnabled;
        }
    }

    private bool IsRagdoll()
    {
        return !anim.enabled;
    }

    private void HitRagdoll()
    {
        Rigidbody rb = rbs[Random.Range(0, rbs.Length)];
        rb.AddForce(Random.insideUnitSphere * 50f, ForceMode.Impulse);
    }
}
