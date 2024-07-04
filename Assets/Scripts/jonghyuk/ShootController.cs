using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    // �߻�ü�� �߻��� ��ġ ����
    public GameObject shootPos;
    // �߻�ü ������
    public GameObject shootPrefab;
    GameObject shoot;
    public float shootingPower = 12f;
    private Rigidbody rb;

    float delyTime = 3f;

    private bool canShoot = true; // �߻� ���� ���θ� üũ�ϴ� ����

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            canShoot = false;
            shoot = Instantiate(shootPrefab);
            shoot.transform.position = shootPos.transform.position;
            rb = shoot.GetComponent<Rigidbody>();
            // ī�޶��� ���� �������� �߻�ü�� ������ ���� ����
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
