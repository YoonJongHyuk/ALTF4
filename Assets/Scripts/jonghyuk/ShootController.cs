using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    // 발사체를 발사할 위치 지정
    public GameObject shootPos;
    // 발사체 프리팹
    public GameObject shootPrefab;
    public GameObject shootChicken;
    GameObject shoot;
    public float shootingPower = 12f;
    private Rigidbody rb;
    Animator anim;

    TestPlayerMove player;


    float delyTime = 1.5f;

    private bool canShoot = true; // 발사 가능 여부를 체크하는 변수

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
        // 카메라의 정면 방향으로 발사체에 물리적 힘을 가함
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
