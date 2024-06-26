using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 0f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        Move();
        Jump();
    }

    public void Jump()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);


        dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    

}
