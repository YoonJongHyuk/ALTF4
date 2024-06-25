using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFloor : MonoBehaviour
{
    public float TrapSpeed = 0f;
    Vector3 dir;

    void Start()
    {
        dir = new Vector3(0, 0, -1);
    }

    private void Update()
    {
        transform.position += dir * TrapSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TrapEndPoint"))
        {
            Destroy(this.gameObject);
        }
    }
}
