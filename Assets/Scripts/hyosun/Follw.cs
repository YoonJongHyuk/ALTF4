using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //public Transform target;
    //public Vector3 offset;
    public float rotSpeed = 200f;
    void Start()
    {

    }


    void Update()
    {

        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        Vector3 dir = new Vector3(-mouse_Y, -mouse_X, 0);

        transform.eulerAngles += dir * rotSpeed * Time.deltaTime;


        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.Clamp(rot.x, -90f, 90f);
        transform.eulerAngles = rot;
        //transform.position = target.position + offset;
    }
}

