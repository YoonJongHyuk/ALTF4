using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // �÷��̾� ������Ʈ�� Transform
    public Transform player;


    void Update()
    {
        // �� ������Ʈ�� ��ġ�� �÷��̾��� ��ġ�� ����
        transform.position = player.position;
    }
}
