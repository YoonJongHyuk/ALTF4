using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTrigger : MonoBehaviour
{
    public GameObject particlePrefab;  // 파티클 시스템 프리팹을 참조할 변수

    private void OnCollisionEnter(Collision collision)
    {
        if (particlePrefab != null)
        {
            // 충돌 지점에 파티클 시스템 프리팹 생성
            Instantiate(particlePrefab, collision.contacts[0].point, Quaternion.identity);
        }
    }
}
