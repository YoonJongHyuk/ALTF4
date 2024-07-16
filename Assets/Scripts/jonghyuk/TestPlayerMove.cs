using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class TestPlayerMove : MonoBehaviour
{
    public enum PlayerType
    {
        Player,
        Chicken
    }

    public PlayerType playerType;

    public float maximumSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 20f;
    public float rollForce = 10f;  // 플레이어가 구를 때 가할 힘의 크기
    float fallSpeedThreshold = -20f;

    bool invincibility = false;
    bool isRegdoll = false;
    public bool isRoll;
    public bool isJumping;
    public bool isDead = false;

    Animator anim;

    float rotY;


    public Transform RespawnPoint;
    public GameObject PlayerPrefab;


    public Transform RespawnPointChicken;
    public GameObject ChickenPrefab;

    public GameObject RagdollPrefab;



    public static int dieCount = 0;  // dieCount 변수를 static으로 선언하여 모든 인스턴스에서 공유
    private TMP_Text killCountText;  // killCountText 변수를 선언

    TipUIManager tipUI;

    public Transform cameraTransform;
    private Rigidbody rb;

    Item item;

    private void Start()
    {
        switch (playerType)
        {
            case PlayerType.Player:
                if (transform.Find("Guard02").gameObject != null)
                {
                    transform.Find("Guard02").gameObject.SetActive(true);
                }
                break;
        }
        
        item = GameObject.FindObjectOfType<Item>();

        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isJumping = false;
        isRoll = true;
        anim = GetComponentInChildren<Animator>();


        // Kill Count Text를 찾습니다
        GameObject text_killCount = GameObject.Find("Canvas").transform.Find("tmp_killCount").gameObject;
        if (text_killCount != null)
        {
            killCountText = text_killCount.GetComponent<TMP_Text>();
            UpdateKillCountText();
        }
    }

    private void Update()
    {
        if (isDead == false)
        {
            Jump();
        }

        // Left Control 키 입력을 감지합니다.
        if (Input.GetKeyDown(KeyCode.LeftControl) && isDead == false && isRoll && !item.itemOK && !isJumping)
        {
            isRoll = false;
            Roll();
            StartCoroutine(WaitForit());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isRegdoll = true;
        }

        if (rb.velocity.y < fallSpeedThreshold && !isDead && playerType == PlayerType.Player && !isRegdoll)
        {
            // 사망하고, 리스폰한다
            Die();
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
        // 플레이어의 수평 입력을 가져옵니다.
        float horizontalInput = Input.GetAxis("Horizontal");
        // 플레이어의 수직 입력을 가져옵니다.
        float verticalInput = Input.GetAxis("Vertical");

        // 수평 및 수직 입력을 사용하여 이동 방향을 계산합니다.
        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        // 입력 크기를 0과 1 사이로 클램프합니다.
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        // 이동 속도를 계산합니다.
        float speed = inputMagnitude * maximumSpeed;

        // 카메라의 회전을 기준으로 이동 방향을 변환합니다.
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        // 속도를 계산합니다.
        Vector3 velocity = movementDirection * speed;
        // Rigidbody를 사용하여 플레이어의 위치를 이동합니다.
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // 이동 방향으로 플레이어의 Y축 회전을 설정합니다.
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Animator가 있는 경우 애니메이션 상태를 업데이트합니다.
        if (anim != null)
        {
            // 플레이어가 움직이고 있는지 여부를 계산합니다.
            bool isMoving = Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0;
            // 애니메이터의 'moveBool' 파라미터를 업데이트합니다.
            anim.SetBool("moveBool", isMoving);
        }
        else
        {
            // Animator 컴포넌트가 없으면 경고를 출력합니다.
            Debug.LogWarning("Animator component is missing!");
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetBool("jumpBool", true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 만일, 플레이어가 땅과 닿는다면
        if (collision.gameObject.layer == 8)
        {
            // 점프가 가능하게 된다
            isJumping = false;
            anim.SetBool("jumpBool", false);
            if (isRegdoll)
            {
                isRegdoll = false;
                isJumping = false;
            }
        }
        // 만일, 플레이어가 함정에 부딪힌다면
        if (collision.gameObject.CompareTag("Trap"))
        {
            // 죽지 않았을 경우
            if (!isDead)
            {
                // 사망하고, 리스폰한다
                Die();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RollingTrap")
        {
            // 죽지 않았을 경우
            if (!isDead && !invincibility)
            {
                // 사망하고, 리스폰한다
                Die();
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

            TestPlayerMove playerMove = player.GetComponent<TestPlayerMove>();

            // PlayerType을 Player로 변경하고 리스폰
            playerMove.playerType = PlayerType.Player;
            playerMove.isJumping = false;
            Destroy(other.gameObject);
            gameObject.SetActive(false);  // ChangeZone 트리거 객체를 비활성화
            RespawnPointChicken.gameObject.SetActive(false);  // 기존 RespawnPoint를 비활성화
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
        tipUI = GameObject.Find("EventSystem").transform.GetComponent<TipUIManager>();
        // 데스 카운트가 늘어나고, UI를 업데이트한다.
        isDead = true;
        dieCount++;
        tipUI.ShowTipUI();
        UpdateKillCountText();
        switch (playerType)
        {
            case PlayerType.Player:
                // PrioritySetting 설정
                PrioritySetting setting = GameObject.FindObjectOfType<PrioritySetting>();
                
                setting.FreeLook3();
                StopAllCoroutines();
                StartCoroutine(RespawnPlayer());
                setting.FreeLook4();
                break;
            case PlayerType.Chicken:
                anim.SetBool("dieBool", true);
                StopAllCoroutines();
                StartCoroutine(RespawnChicken());
                break;
        }
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
        print("플레이어 테스트");
        // 새 플레이어를 생성한다
        GameObject newPlayer = Instantiate(PlayerPrefab, RespawnPoint.transform.position, RespawnPoint.transform.rotation);
        transform.Find("Guard02").gameObject.SetActive(false);
        GameObject newRagdoll = Instantiate(RagdollPrefab, this.gameObject.transform);




        // 시네머신 FreeLook 카메라를 찾아서 Follow와 Look At 속성을 업데이트합니다.
        CinemachineFreeLook freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        virtualCamera.Follow = newRagdoll.transform;
        virtualCamera.LookAt = newRagdoll.transform.Find("cm").transform;



        // 기존 플레이어 오브젝트와 차별점을 위해, 리스폰된 오브젝트에 숫자를 기입한다.
        newPlayer.name = "Player" + (dieCount + 1);


        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();
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

        

        Transform newShootPosTransform = newPlayer.GetComponent<Transform>().transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);
        if (freeLookCamera != null)
        {
            

            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
            virtualCamera.Follow = newPlayer.transform;
            virtualCamera.LookAt = newPlayer.transform;


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
        // 디버그용 메시지 출력
        print("치킨 테스트");

        // 새 플레이어(치킨) 생성
        GameObject newPlayer = Instantiate(ChickenPrefab, RespawnPointChicken.transform.position, RespawnPointChicken.transform.rotation);

        // 새 플레이어의 이름에 번호 추가
        newPlayer.name = "Player" + (dieCount + 1);

        // 새 플레이어의 TestPlayerMove 컴포넌트 가져오기
        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();
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
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // 새 플레이어의 이동 활성화
        playerCode.isDead = false;

        // 코루틴 종료
        yield return null;
    }
}
