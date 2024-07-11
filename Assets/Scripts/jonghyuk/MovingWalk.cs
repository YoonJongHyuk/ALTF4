using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWalk : MonoBehaviour
{
    public float speed = 1.0f;  // ������ũ�� �ӵ�

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Rigidbody ������Ʈ�� ����Ͽ� ������ �̵� ����
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 movement = new Vector3(0, 0, speed * Time.deltaTime);
                rb.MovePosition(rb.position + movement);
            }
        }
    }
}
