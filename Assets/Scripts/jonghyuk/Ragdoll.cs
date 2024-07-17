using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    TestPlayerMove player;

    private void Start()
    {
        player = GetComponent<TestPlayerMove>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            player.isRegdoll = false;
        }
    }

    IEnumerator WakeUpPlayer()
    {
        yield return new WaitForSeconds(2.0f);
    }

}
