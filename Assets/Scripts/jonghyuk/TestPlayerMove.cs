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
    public float rollForce = 10f;  // �÷��̾ ���� �� ���� ���� ũ��
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



    public static int dieCount = 0;  // dieCount ������ static���� �����Ͽ� ��� �ν��Ͻ����� ����
    private TMP_Text killCountText;  // killCountText ������ ����

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


        // Kill Count Text�� ã���ϴ�
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

        // Left Control Ű �Է��� �����մϴ�.
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
            // ����ϰ�, �������Ѵ�
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
        // �÷��̾��� �� �������� ���� ���մϴ�.
        rb.AddForce(Camera.main.transform.forward * rollForce, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
        // �÷��̾��� ���� �Է��� �����ɴϴ�.
        float horizontalInput = Input.GetAxis("Horizontal");
        // �÷��̾��� ���� �Է��� �����ɴϴ�.
        float verticalInput = Input.GetAxis("Vertical");

        // ���� �� ���� �Է��� ����Ͽ� �̵� ������ ����մϴ�.
        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        // �Է� ũ�⸦ 0�� 1 ���̷� Ŭ�����մϴ�.
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        // �̵� �ӵ��� ����մϴ�.
        float speed = inputMagnitude * maximumSpeed;

        // ī�޶��� ȸ���� �������� �̵� ������ ��ȯ�մϴ�.
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        // �ӵ��� ����մϴ�.
        Vector3 velocity = movementDirection * speed;
        // Rigidbody�� ����Ͽ� �÷��̾��� ��ġ�� �̵��մϴ�.
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // �̵� �������� �÷��̾��� Y�� ȸ���� �����մϴ�.
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Animator�� �ִ� ��� �ִϸ��̼� ���¸� ������Ʈ�մϴ�.
        if (anim != null)
        {
            // �÷��̾ �����̰� �ִ��� ���θ� ����մϴ�.
            bool isMoving = Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0;
            // �ִϸ������� 'moveBool' �Ķ���͸� ������Ʈ�մϴ�.
            anim.SetBool("moveBool", isMoving);
        }
        else
        {
            // Animator ������Ʈ�� ������ ��� ����մϴ�.
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
        // ����, �÷��̾ ���� ��´ٸ�
        if (collision.gameObject.layer == 8)
        {
            // ������ �����ϰ� �ȴ�
            isJumping = false;
            anim.SetBool("jumpBool", false);
            if (isRegdoll)
            {
                isRegdoll = false;
                isJumping = false;
            }
        }
        // ����, �÷��̾ ������ �ε����ٸ�
        if (collision.gameObject.CompareTag("Trap"))
        {
            // ���� �ʾ��� ���
            if (!isDead)
            {
                // ����ϰ�, �������Ѵ�
                Die();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RollingTrap")
        {
            // ���� �ʾ��� ���
            if (!isDead && !invincibility)
            {
                // ����ϰ�, �������Ѵ�
                Die();
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

            TestPlayerMove playerMove = player.GetComponent<TestPlayerMove>();

            // PlayerType�� Player�� �����ϰ� ������
            playerMove.playerType = PlayerType.Player;
            playerMove.isJumping = false;
            Destroy(other.gameObject);
            gameObject.SetActive(false);  // ChangeZone Ʈ���� ��ü�� ��Ȱ��ȭ
            RespawnPointChicken.gameObject.SetActive(false);  // ���� RespawnPoint�� ��Ȱ��ȭ
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
        // ���� ī��Ʈ�� �þ��, UI�� ������Ʈ�Ѵ�.
        isDead = true;
        dieCount++;
        tipUI.ShowTipUI();
        UpdateKillCountText();
        switch (playerType)
        {
            case PlayerType.Player:
                // PrioritySetting ����
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
            // killCountText�� dieCount�� �Ҵ��Ѵ�.
            killCountText.text = dieCount.ToString();
        }
    }

    IEnumerator RespawnPlayer()
    {
        print("�÷��̾� �׽�Ʈ");
        // �� �÷��̾ �����Ѵ�
        GameObject newPlayer = Instantiate(PlayerPrefab, RespawnPoint.transform.position, RespawnPoint.transform.rotation);
        transform.Find("Guard02").gameObject.SetActive(false);
        GameObject newRagdoll = Instantiate(RagdollPrefab, this.gameObject.transform);




        // �ó׸ӽ� FreeLook ī�޶� ã�Ƽ� Follow�� Look At �Ӽ��� ������Ʈ�մϴ�.
        CinemachineFreeLook freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        virtualCamera.Follow = newRagdoll.transform;
        virtualCamera.LookAt = newRagdoll.transform.Find("cm").transform;



        // ���� �÷��̾� ������Ʈ�� �������� ����, �������� ������Ʈ�� ���ڸ� �����Ѵ�.
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

        // �� �÷��̾ �������� �ʵ��� ����
        playerCode.isDead = true;

        // ���� �÷��̾��� Rigidbody ȸ�� ������ �����մϴ�.
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

        

        // ���� �÷��̾�� TestPlayerMove ��ũ��Ʈ�� �����Ͽ� ��ü�� ����ϴ�.
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // ī�޶� ������ �Ϸ�� �� �� �÷��̾��� �̵��� Ȱ��ȭ
        playerCode.isDead = false;

        yield return null;
    }

    IEnumerator RespawnChicken()
    {
        // ����׿� �޽��� ���
        print("ġŲ �׽�Ʈ");

        // �� �÷��̾�(ġŲ) ����
        GameObject newPlayer = Instantiate(ChickenPrefab, RespawnPointChicken.transform.position, RespawnPointChicken.transform.rotation);

        // �� �÷��̾��� �̸��� ��ȣ �߰�
        newPlayer.name = "Player" + (dieCount + 1);

        // �� �÷��̾��� TestPlayerMove ������Ʈ ��������
        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();
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
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // �� �÷��̾��� �̵� Ȱ��ȭ
        playerCode.isDead = false;

        // �ڷ�ƾ ����
        yield return null;
    }
}
