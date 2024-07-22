using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPuzzle : MonoBehaviour
{
    public GameObject door;
    public Transform doorClsePos;
    public static bool openDoor = false;
    Vector3 dir;

    MeshRenderer thisColor;

    public static int isOpen = 0;
    bool usePuzzle = false;

    private void Start()
    {
        openDoor = false;
        isOpen = 0;
        thisColor = this.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isOpen == 4 && door.transform.position.y > doorClsePos.transform.position.y)
        {
            openDoor = true;
            dir = doorClsePos.position - door.transform.position;
            door.transform.position += dir * Time.deltaTime;
        }
        else if (door.transform.position.y < doorClsePos.transform.position.y)
        {
            isOpen = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shoot") && !usePuzzle)
        {
            isOpen++;
            thisColor.material.color = Color.red;
            usePuzzle = true;
        }
    }
}
