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
        // 만일, 플레이어가 땅과 닿는다면
        if (collision.gameObject.layer == 8)
        {
            // 점프가 가능하게 된다
            manager.isJumping = false;
            manager.anim.SetBool("jumpBool", false);
            if (manager.isRegdoll)
            {
                manager.isRegdoll = false;
                manager.isJumping = false;
            }
        }
        // 만일, 플레이어가 함정에 부딪힌다면
        if (collision.gameObject.CompareTag("Trap"))
        {
            // 죽지 않았을 경우
            if (!manager.isDead)
            {
                // 사망하고, 리스폰한다
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
            // 죽지 않았을 경우
            if (!manager.isDead && !manager.invincibility)
            {
                // 사망하고, 리스폰한다
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
            // Player와 RespawnPoint 활성화
            GameObject player = GameObject.Find("FourStage").transform.Find("Player").gameObject;
            GameObject respawnPoint = GameObject.Find("FourStage").transform.Find("RespawnPoint").gameObject;
            player.SetActive(true);
            respawnPoint.SetActive(true);

            // PrioritySetting 설정
            PrioritySetting setting = GameObject.FindObjectOfType<PrioritySetting>();
            setting.buttonFreeLook1();

            // FreeLook 카메라 설정
            GameObject freelook1 = GameObject.Find("CameraParent").transform.Find("FreeLook Camera1").gameObject;
            GameObject freelook2 = GameObject.Find("CameraParent").transform.Find("FreeLook Camera2").gameObject;
            freelook1.SetActive(true);
            freelook2.SetActive(false);

            PlayerManager playerMove = player.GetComponent<PlayerManager>();

            // PlayerType을 Player로 변경하고 리스폰
            playerMove.playerType = PlayerType.Player;
            playerMove.isJumping = false;
            Destroy(other.gameObject);
            gameObject.SetActive(false);  // ChangeZone 트리거 객체를 비활성화
            manager.RespawnPointChicken.gameObject.SetActive(false);  // 기존 RespawnPoint를 비활성화
        }
    }

    

    public IEnumerator RespawnPlayer()
    {
        print("플레이어 테스트");
        // 새 플레이어를 생성한다
        GameObject newPlayer = Instantiate(manager.PlayerPrefab, manager.RespawnPoint.transform.position, manager.RespawnPoint.transform.rotation);

        // 기존 플레이어 오브젝트와 차별점을 위해, 리스폰된 오브젝트에 숫자를 기입한다.
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

        // 새 플레이어가 움직이지 않도록 설정
        playerCode.isDead = true;

        // 기존 플레이어의 Rigidbody 회전 고정을 해제합니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        yield return new WaitForSeconds(2.0f);

        // 시네머신 FreeLook 카메라를 찾아서 Follow와 Look At 속성을 업데이트합니다.
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
        // 기존 플레이어에서 PlayerManager 스크립트를 제거하여 시체로 남깁니다.
        Destroy(GetComponent<PlayerManager>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // 카메라 설정이 완료된 후 새 플레이어의 이동을 활성화
        playerCode.isDead = false;

        yield return null;
    }


    public IEnumerator RespawnChicken()
    {
        // 디버그용 메시지 출력
        print("치킨 테스트");

        // 새 플레이어(치킨) 생성
        GameObject newPlayer = Instantiate(manager.ChickenPrefab, manager.RespawnPointChicken.transform.position, manager.RespawnPointChicken.transform.rotation);

        // 새 플레이어의 이름에 번호 추가
        newPlayer.name = "Player" + (playerUI.deathCount + 1);

        // 새 플레이어의 PlayerManager 컴포넌트 가져오기
        PlayerManager playerCode = newPlayer.GetComponent<PlayerManager>();
        playerCode.isJumping = false;

        // 새 플레이어의 타입을 치킨으로 설정
        playerCode.playerType = PlayerType.Chicken;

        // 화면에 표시될 텍스트 오브젝트 활성화
        GameObject text_dieText = GameObject.Find("Canvas").transform.Find("tmp_dieText").gameObject;
        text_dieText.SetActive(true);

        // 기존 플레이어의 ShootController 비활성화
        ShootController shoot = gameObject.GetComponent<ShootController>();
        shoot.SetCanShoot(false);

        // 새 플레이어의 ShootController 비활성화
        ShootController shootController = newPlayer.GetComponent<ShootController>();
        shootController.SetCanShoot(false);

        // 아이템 상태 초기화
        Item item = GameObject.FindObjectOfType<Item>();
        item.itemOK = false;
        item.itemType = Item.ItemType.None;

        // 카메라 참조 설정
        Camera camera = FindObjectOfType<Camera>();
        playerCode.cameraTransform = camera.transform;

        // 새 리스폰 지점 설정
        Transform newRespawnPoint = GameObject.Find("ChickenRespawnPoint").transform;
        playerCode.RespawnPointChicken = newRespawnPoint;

        // 새 플레이어의 이동을 일시적으로 비활성화
        playerCode.isDead = true;

        // 기존 플레이어의 Rigidbody 제약 해제
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        // 2초 대기
        yield return new WaitForSeconds(2.0f);

        // 시네머신 FreeLook 카메라 설정 업데이트
        CinemachineFreeLook freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        // 새 플레이어의 ShootPos 설정 및 ShootController 활성화
        Transform newShootPosTransform = newPlayer.GetComponent<Transform>().transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);

        if (freeLookCamera != null)
        {
            // 텍스트 비활성화 및 카메라 Follow, LookAt 설정 업데이트
            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
        }

        // 기존 플레이어의 스크립트 제거하여 시체로 남김
        Destroy(GetComponent<PlayerManager>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // 새 플레이어의 이동 활성화
        playerCode.isDead = false;

        // 코루틴 종료
        yield return null;
    }

}
