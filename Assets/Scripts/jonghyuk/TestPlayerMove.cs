using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerMove : MonoBehaviour
{

    public enum PlayerType
    {
        Player,
        Chicken
    }

    PlayerType playerType;

    public float maximumSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 20f;
    public float rollForce = 10f;  // 플레이어가 구를 때 가할 힘의 크기
    bool invincibility = false;

    public bool isRoll;
    public bool isJumping;
    public bool isDead = false;




    public Transform RespawnPoint;
    public GameObject PlayerPrefab;


    public Transform RespawnPointChicken;
    public GameObject ChickenPrefab;



    public static int dieCount = 0;  // dieCount 변수를 static으로 선언하여 모든 인스턴스에서 공유
    private Text killCountText;  // killCountText 변수를 선언

    public Transform cameraTransform;
    private Rigidbody rb;

    Item item;

    private void Start()
    {
        playerType = PlayerType.Chicken;
        item = GameObject.FindObjectOfType<Item>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isJumping = false;
        isRoll = true;

        // Kill Count Text를 찾습니다
        GameObject text_killCount = GameObject.Find("Canvas").transform.Find("text_killCount").gameObject;
        if (text_killCount != null)
        {
            killCountText = text_killCount.GetComponent<Text>();
            UpdateKillCountText();
        }
    }

    private void Update()
    {
        if (isDead == false)
        {
            Jump();
            //RotatePlayer();
        }

        // Left Control 키 입력을 감지합니다.
        if (Input.GetKeyDown(KeyCode.LeftControl) && isDead == false && isRoll && !item.itemOK && !isJumping)
        {
            isRoll = false;
            Roll();
            StartCoroutine(WaitForit());
            WaitForit();
        }
    }

    private void FixedUpdate()
    {
        if(isDead == false)
        {
            MovePlayer();
        }
    }

    IEnumerator WaitForit()
    {
        invincibility = true;
        yield return new WaitForSeconds(1f);
        invincibility = false;
        yield return new WaitForSeconds(1f);
        isRoll = true;
        
    }

    void Roll()
    {
        // 플레이어의 앞 방향으로 힘을 가합니다.
        rb.AddForce(Camera.main.transform.forward * rollForce, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        float speed = inputMagnitude * maximumSpeed;

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        Vector3 velocity = movementDirection * speed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 만일, 플레이어가 땅과 닿는다면
        if (collision.gameObject.layer == 8)
        {
            // 점프가 가능하게 된다
            isJumping = false;
        }
        // 만일, 플레이어가 함정에 부딪힌다면
        if (collision.gameObject.CompareTag("Trap"))
        {
            // 죽지 않았을 경우
            if (!isDead)
            {
                // 사망하고, 리스폰한다
                Die();
                switch(playerType)
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "RollingTrap")
        {
            // 죽지 않았을 경우
            if (!isDead && !invincibility)
            {
                // 사망하고, 리스폰한다
                Die();
                switch(playerType)
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
        if (other.gameObject.tag == "ChangeZone")
        {
            print("테스트");
            GameObject player = GameObject.Find("FourStage").transform.Find("Player").gameObject;
            GameObject respawnPoint = GameObject.Find("FourStage").transform.Find("RespawnPoint").gameObject;
            player.SetActive(true);
            respawnPoint.SetActive(true);
            PrioritySetting setting = GameObject.FindObjectOfType<PrioritySetting>();
            setting.buttonFreeLook1();
            GameObject freelook1 = GameObject.Find("CameraParent").transform.Find("FreeLook Camera1").gameObject;
            GameObject freelook2 = GameObject.Find("CameraParent").transform.Find("FreeLook Camera2").gameObject;
            freelook1.SetActive(true);
            print(freelook1);
            freelook2.SetActive(false);
            playerType = PlayerType.Player;
            gameObject.SetActive(false);
            RespawnPointChicken.gameObject.SetActive(false);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Die()
    {
        // 데스 카운트가 늘어나고, UI를 업데이트한다.
        isDead = true;
        dieCount++;
        UpdateKillCountText();
    }

    private void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            // killCountText에 dieCount를 할당한다.
            killCountText.text = dieCount.ToString();
        }
    }

    IEnumerator RespawnPlayer()
    {
        // 새 플레이어를 생성한다
        GameObject newPlayer = Instantiate(PlayerPrefab, RespawnPoint.transform.position, RespawnPoint.transform.rotation);

        // 기존 플레이어 오브젝트와 차별점을 위해, 리스폰된 오브젝트에 숫자를 기입한다.
        newPlayer.name = "Player" + (dieCount + 1);


        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();

        GameObject text_dieText = GameObject.Find("Canvas").transform.Find("text_dieText").gameObject;
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
        
        Transform newShootPosTransform = newPlayer.transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);
        if (freeLookCamera != null)
        {
            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
        }
        // 기존 플레이어에서 TestPlayerMove 스크립트를 제거하여 시체로 남깁니다.
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // 카메라 설정이 완료된 후 새 플레이어의 이동을 활성화
        playerCode.isDead = false;

        yield return null;
    }

    IEnumerator RespawnChicken()
    {
        // 새 플레이어를 생성한다
        GameObject newPlayer = Instantiate(ChickenPrefab, RespawnPointChicken.transform.position, RespawnPointChicken.transform.rotation);

        // 기존 플레이어 오브젝트와 차별점을 위해, 리스폰된 오브젝트에 숫자를 기입한다.
        newPlayer.name = "Player" + (dieCount + 1);


        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();

        GameObject text_dieText = GameObject.Find("Canvas").transform.Find("text_dieText").gameObject;
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
        Transform newRespawnPoint = GameObject.Find("ChickenRespawnPoint").transform;
        playerCode.RespawnPointChicken = newRespawnPoint;

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

        Transform newShootPosTransform = newPlayer.transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);
        if (freeLookCamera != null)
        {
            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
        }
        // 기존 플레이어에서 TestPlayerMove 스크립트를 제거하여 시체로 남깁니다.
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // 카메라 설정이 완료된 후 새 플레이어의 이동을 활성화
        playerCode.isDead = false;

        yield return null;
    }
}
