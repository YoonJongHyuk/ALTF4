using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    // �߻�ü�� �߻��� ��ġ ����
    public GameObject shootPos;
    // �߻�ü ������
    public GameObject shootPrefab;
    public GameObject shootChicken;
    GameObject shoot;
    public float shootingPower = 12f;
    private Rigidbody rb;
    Animator anim;

    TestPlayerMove player;


    float delyTime = 1.5f;

    private bool canShoot = true; // �߻� ���� ���θ� üũ�ϴ� ����

    private void Start()
    {
        player = GetComponent<TestPlayerMove>();
        switch (player.playerType)
        {
            case TestPlayerMove.PlayerType.Player:
                anim = GetComponent<Animator>();
                break;
            case TestPlayerMove.PlayerType.Chicken:
                anim = transform.Find("Chicken").GetComponent<Animator>();
                break;
        }
    }

    private void Update()
    {
        switch (player.playerType)
        {
            case TestPlayerMove.PlayerType.Player:
                if (Input.GetMouseButtonDown(0) && canShoot && !player.isRegdoll && !player.isDead)
                {
                    StartCoroutine(Throwing(shootChicken));
                }
                break;

            case TestPlayerMove.PlayerType.Chicken:
                if (Input.GetMouseButtonDown(0) && canShoot && !player.isRegdoll && !player.isDead)
                {
                    StartCoroutine(Throwing(shootPrefab));
                }
                break;
        }
    }

    IEnumerator Throwing(GameObject shootObject)
    {
        anim.SetTrigger("throwing");
        canShoot = false;
        yield return new WaitForSeconds(0.5f);
        shoot = Instantiate(shootObject);
        shoot.transform.position = shootPos.transform.position;
        rb = shoot.GetComponent<Rigidbody>();
        // ī�޶��� ���� �������� �߻�ü�� ������ ���� ����
        rb.AddForce(Camera.main.transform.forward * shootingPower, ForceMode.Impulse);
        StartCoroutine(WaitForit(shoot));
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    private void OnDestroy()
    {
        Destroy(shoot);
    }


    IEnumerator WaitForit(GameObject value)
    {
        yield return new WaitForSeconds(delyTime);
        Destroy(value.gameObject);
        canShoot = true;
    }
}
