using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTrigger : MonoBehaviour
{
    public GameObject particlePrefab;  // ��ƼŬ �ý��� �������� ������ ����

    private void OnCollisionEnter(Collision collision)
    {
        if (particlePrefab != null)
        {
            // �浹 ������ ��ƼŬ �ý��� ������ ����
            Instantiate(particlePrefab, collision.contacts[0].point, Quaternion.identity);
        }
    }
}
