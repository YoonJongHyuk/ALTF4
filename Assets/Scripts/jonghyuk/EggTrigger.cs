using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTrigger : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
