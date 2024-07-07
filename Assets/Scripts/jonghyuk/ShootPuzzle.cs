using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPuzzle : MonoBehaviour
{
    public GameObject door;
    public Transform doorClsePos;
    Vector3 dir;

    bool isOpen = false;

    // Update is called once per frame
    void Update()
    {
        
        if (isOpen && door.transform.position.y > doorClsePos.transform.position.y)
        {
            dir = doorClsePos.position - door.transform.position;
            door.transform.position += dir * Time.deltaTime;
        }
        else if (door.transform.position.y < doorClsePos.transform.position.y)
        {
            isOpen = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shoot"))
        {
            isOpen = true;
        }
    }
}
