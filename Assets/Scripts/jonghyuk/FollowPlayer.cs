using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // 플레이어 오브젝트의 Transform
    public Transform player;


    void Update()
    {
        // 빈 오브젝트의 위치를 플레이어의 위치로 설정
        transform.position = player.position;
    }
}
