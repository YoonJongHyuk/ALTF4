using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5.0f;

    public GameObject player;

    private void Update()
    {
        Vector3 playerVector = new Vector3(player.transform.position.x, player.transform.position.y + 3f, player.transform.position.z);
        Vector3 dir = playerVector - this.transform.position;
        
        if (dir.magnitude < 0.1f)
        {
            this.transform.position = playerVector;
        }
        else
        {
            dir.Normalize();
            transform.position += dir * cameraSpeed * Time.deltaTime;
        }

    }
}
