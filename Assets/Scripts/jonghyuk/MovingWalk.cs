using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWalk : MonoBehaviour
{
    public float speed = 1.0f;  // 무빙워크의 속도

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Rigidbody 컴포넌트를 사용하여 물리적 이동 적용
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 movement = new Vector3(0, 0, speed * Time.deltaTime);
                rb.MovePosition(rb.position + movement);
            }
        }
    }
}
