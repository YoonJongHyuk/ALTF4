using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayer : MonoBehaviour
{
    PlayerManager manager;

    PlayerType playerType;

    PlayerUI playerUI;

    private void Start()
    {
        manager = GetComponent<PlayerManager>();
        playerType = manager.playerType;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ����, �÷��̾ ���� ��´ٸ�
        if (collision.gameObject.layer == 8)
        {
            // ������ �����ϰ� �ȴ�
            manager.isJumping = false;
            manager.anim.SetBool("jumpBool", false);
            if (manager.isRegdoll)
            {
                manager.isRegdoll = false;
                manager.isJumping = false;
            }
        }
        // ����, �÷��̾ ������ �ε����ٸ�
        if (collision.gameObject.CompareTag("Trap"))
        {
            // ���� �ʾ��� ���
            if (!manager.isDead)
            {
                // ����ϰ�, �������Ѵ�
                manager.Die();
                switch (playerType)
                {
                    case PlayerType.Player:
                        StopAllCoroutines();
                        StartCoroutine(RespawnPlayer());
                        break;
                    case PlayerType.Chicken:
                        StopAllCoroutines();
                        StartCoroutine(RespawnChicken());
                        break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RollingTrap")
        {
            // ���� �ʾ��� ���
            if (!manager.isDead && !manager.invincibility)
            {
                // ����ϰ�, �������Ѵ�
                manager.Die();
                switch (playerType)
                {
                    case PlayerType.Player:
                        StopAllCoroutines();
                        StartCoroutine(RespawnPlayer());
                        break;
                    case PlayerType.Chicken:
                        StopAllCoroutines();
                        StartCoroutine(RespawnChicken());
                        break;
                }
            }
        }
        else if (other.gameObject.tag == "ChangeZone")
        {
            // Player�� RespawnPoint Ȱ��ȭ
            GameObject player = GameObject.Find("FourStage").transform.Find("Player").gameObject;
            GameObject respawnPoint = GameObject.Find("FourStage").transform.Find("RespawnPoint").gameObject;
            player.SetActive(true);
            respawnPoint.SetActive(true);

            // PrioritySetting ����
            PrioritySetting setting = GameObject.FindObjectOfType<PrioritySetting>();
            setting.buttonFreeLook1();

            // FreeLook ī�޶� ����
            GameObject freelook1 = GameObject.Find("CameraParent").transform.Find("FreeLook Camera1").gameObject;
            GameObject freelook2 = GameObject.Find("CameraParent").transform.Find("FreeLook Camera2").gameObject;
            freelook1.SetActive(true);
            freelook2.SetActive(false);

            PlayerManager playerMove = player.GetComponent<PlayerManager>();

            // PlayerType�� Player�� �����ϰ� ������
            playerMove.playerType = PlayerType.Player;
            playerMove.isJumping = false;
            Destroy(other.gameObject);
            gameObject.SetActive(false);  // ChangeZone Ʈ���� ��ü�� ��Ȱ��ȭ
            manager.RespawnPointChicken.gameObject.SetActive(false);  // ���� RespawnPoint�� ��Ȱ��ȭ
        }
    }

    

    public IEnumerator RespawnPlayer()
    {
        print("�÷��̾� �׽�Ʈ");
        // �� �÷��̾ �����Ѵ�
        GameObject newPlayer = Instantiate(manager.PlayerPrefab, manager.RespawnPoint.transform.position, manager.RespawnPoint.transform.rotation);

        // ���� �÷��̾� ������Ʈ�� �������� ����, �������� ������Ʈ�� ���ڸ� �����Ѵ�.
        newPlayer.name = "Player" + (playerUI.deathCount + 1);


        PlayerManager playerCode = newPlayer.GetComponent<PlayerManager>();
        playerCode.isJumping = false;

        GameObject text_dieText = GameObject.Find("Canvas").transform.Find("tmp_dieText").gameObject;
        text_dieText.SetActive(true);

        ShootController shoot = gameObject.GetComponent<ShootController>();
        shoot.SetCanShoot(false);
        ShootController shootController = newPlayer.GetComponent<ShootController>();
        shootController.SetCanShoot(false);

        Item item = GameObject.FindObjectOfType<Item>();
        item.itemOK = false;
        item.itemType = Item.ItemType.None;

        Camera camera = FindObjectOfType<Camera>();
        playerCode.cameraTransform = camera.transform;
        Transform newRespawnPoint = GameObject.Find("RespawnPoint").transform;
        playerCode.RespawnPoint = newRespawnPoint;

        // �� �÷��̾ �������� �ʵ��� ����
        playerCode.isDead = true;

        // ���� �÷��̾��� Rigidbody ȸ�� ������ �����մϴ�.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        yield return new WaitForSeconds(2.0f);

        // �ó׸ӽ� FreeLook ī�޶� ã�Ƽ� Follow�� Look At �Ӽ��� ������Ʈ�մϴ�.
        CinemachineFreeLook freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        Transform newShootPosTransform = newPlayer.GetComponent<Transform>().transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);
        if (freeLookCamera != null)
        {
            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
        }
        // ���� �÷��̾�� PlayerManager ��ũ��Ʈ�� �����Ͽ� ��ü�� ����ϴ�.
        Destroy(GetComponent<PlayerManager>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // ī�޶� ������ �Ϸ�� �� �� �÷��̾��� �̵��� Ȱ��ȭ
        playerCode.isDead = false;

        yield return null;
    }


    public IEnumerator RespawnChicken()
    {
        // ����׿� �޽��� ���
        print("ġŲ �׽�Ʈ");

        // �� �÷��̾�(ġŲ) ����
        GameObject newPlayer = Instantiate(manager.ChickenPrefab, manager.RespawnPointChicken.transform.position, manager.RespawnPointChicken.transform.rotation);

        // �� �÷��̾��� �̸��� ��ȣ �߰�
        newPlayer.name = "Player" + (playerUI.deathCount + 1);

        // �� �÷��̾��� PlayerManager ������Ʈ ��������
        PlayerManager playerCode = newPlayer.GetComponent<PlayerManager>();
        playerCode.isJumping = false;

        // �� �÷��̾��� Ÿ���� ġŲ���� ����
        playerCode.playerType = PlayerType.Chicken;

        // ȭ�鿡 ǥ�õ� �ؽ�Ʈ ������Ʈ Ȱ��ȭ
        GameObject text_dieText = GameObject.Find("Canvas").transform.Find("tmp_dieText").gameObject;
        text_dieText.SetActive(true);

        // ���� �÷��̾��� ShootController ��Ȱ��ȭ
        ShootController shoot = gameObject.GetComponent<ShootController>();
        shoot.SetCanShoot(false);

        // �� �÷��̾��� ShootController ��Ȱ��ȭ
        ShootController shootController = newPlayer.GetComponent<ShootController>();
        shootController.SetCanShoot(false);

        // ������ ���� �ʱ�ȭ
        Item item = GameObject.FindObjectOfType<Item>();
        item.itemOK = false;
        item.itemType = Item.ItemType.None;

        // ī�޶� ���� ����
        Camera camera = FindObjectOfType<Camera>();
        playerCode.cameraTransform = camera.transform;

        // �� ������ ���� ����
        Transform newRespawnPoint = GameObject.Find("ChickenRespawnPoint").transform;
        playerCode.RespawnPointChicken = newRespawnPoint;

        // �� �÷��̾��� �̵��� �Ͻ������� ��Ȱ��ȭ
        playerCode.isDead = true;

        // ���� �÷��̾��� Rigidbody ���� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        // 2�� ���
        yield return new WaitForSeconds(2.0f);

        // �ó׸ӽ� FreeLook ī�޶� ���� ������Ʈ
        CinemachineFreeLook freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        // �� �÷��̾��� ShootPos ���� �� ShootController Ȱ��ȭ
        Transform newShootPosTransform = newPlayer.GetComponent<Transform>().transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);

        if (freeLookCamera != null)
        {
            // �ؽ�Ʈ ��Ȱ��ȭ �� ī�޶� Follow, LookAt ���� ������Ʈ
            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
        }

        // ���� �÷��̾��� ��ũ��Ʈ �����Ͽ� ��ü�� ����
        Destroy(GetComponent<PlayerManager>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // �� �÷��̾��� �̵� Ȱ��ȭ
        playerCode.isDead = false;

        // �ڷ�ƾ ����
        yield return null;
    }

}
