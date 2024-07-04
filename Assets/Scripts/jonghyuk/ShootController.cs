using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    // 발사체를 발사할 위치 지정
    public GameObject shootPos;
    // 발사체 프리팹
    public GameObject shootPrefab;
    GameObject shoot;
    public float shootingPower = 12f;
    private Rigidbody rb;

    float delyTime = 3f;

    private bool canShoot = true; // 발사 가능 여부를 체크하는 변수

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            canShoot = false;
            shoot = Instantiate(shootPrefab);
            shoot.transform.position = shootPos.transform.position;
            rb = shoot.GetComponent<Rigidbody>();
            // 카메라의 정면 방향으로 발사체에 물리적 힘을 가함
            rb.AddForce(Camera.main.transform.forward * shootingPower, ForceMode.Impulse);
            StartCoroutine(WaitForit(shoot));
        }
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
