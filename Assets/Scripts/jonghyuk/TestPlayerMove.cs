using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Antlr3.Runtime.Tree;


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
    public float forceAmount = 10f; // ���� ���� ũ��
    float fallSpeedThreshold = -20f;

    bool invincibility = false;
    public bool isRegdoll = false;
    public bool isRoll;
    public bool isJumping;
    public bool isDead = false;
    bool skyRagdoll = false;

    [HideInInspector]
    public Animator anim;

    float rotY;


    public Transform RespawnPoint;
    public GameObject PlayerPrefab;


    public Transform RespawnPointChicken;
    public GameObject ChickenPrefab;


    public GameObject RagdollPrefab;

    public GameObject RagdollSpine;

    Vector3 movementDirection;

    bool isMove;

    CinemachineFreeLook freeLookCamera1;
    CinemachineFreeLook freeLookCamera2;



    public static int dieCount = 0;  // dieCount ������ static���� �����Ͽ� ��� �ν��Ͻ����� ����
    private TMP_Text killCountText;  // killCountText ������ ����

    TipUIManager tipUI;

    public Transform cameraTransform;
    private Rigidbody rb;

    Item item;

    private void Start()
    {
        freeLookCamera1 = GameObject.Find("CameraParent").transform.Find("FreeLookCamera1").GetComponent<CinemachineFreeLook>();
        freeLookCamera2 = GameObject.Find("CameraParent").transform.Find("FreeLookCamera2").GetComponent<CinemachineFreeLook>();
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
        if (isDead == false && isRegdoll == false)
        {
            Jump();
        }

        // Left Control Ű �Է��� �����մϴ�.
        if (Input.GetKeyDown(KeyCode.LeftControl) && isDead == false && isRoll && !item.itemOK && !isJumping && isRegdoll == false && isMove)
        {
            isRoll = false;
            Roll();
            StartCoroutine(WaitForit());
        }

        if (rb.velocity.y < -1f && !isDead && playerType == PlayerType.Player && !isRegdoll)
        {
            skyRagdoll = true;
        }

        if (Input.GetKeyDown(KeyCode.R) && isRegdoll == false)
        {
            switch (playerType)
            {
                case PlayerType.Player:
                    if(isJumping)
                        skyRagdoll = true;
                    Ragdoll();
                    break;
            }
            
        }

        if (rb.velocity.y < fallSpeedThreshold && !isDead && playerType == PlayerType.Player && !isRegdoll)
        {
            // ����ϰ�, �������Ѵ�
            Die();
        }
    }

    private void FixedUpdate()
    {
        if(isDead == false && isRegdoll == false)
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


    void Ragdoll()
    {
        isRegdoll = true;

        if (rb.velocity.y < -10f)
        {
            skyRagdoll = true;
        }

        GameObject player = transform.Find("Guard02").gameObject;
        player.gameObject.SetActive(false);
        GameObject ragdollPrefab = transform.Find("RagdollPrefab").gameObject;
        ragdollPrefab.transform.position = player.transform.position;
        ragdollPrefab.gameObject.SetActive(true);

        freeLookCamera1.Follow = gameObject.transform.Find("RagdollPrefab");
        freeLookCamera1.LookAt = gameObject.transform.Find("RagdollCm").transform;

        StartCoroutine(ChangeRagdoll());
    }

    IEnumerator ChangeRagdoll()
    {
        if (skyRagdoll)
            yield return new WaitForSeconds(1.0f);
        else
        {


            #region �ִϸ��̼� ���� ���󺹱�
            yield return new WaitForSeconds(2.0f);
            freeLookCamera1.Follow = gameObject.transform;
            freeLookCamera1.LookAt = gameObject.transform.Find("cm").transform;
            GameObject player = transform.Find("Guard02").gameObject;
            player.gameObject.SetActive(true);
            GameObject ragdollPrefab = transform.Find("RagdollPrefab").gameObject;
            ragdollPrefab.gameObject.SetActive(false);
            ragdollPrefab.transform.position = Vector3.zero;
            ragdollPrefab.transform.rotation = Quaternion.identity;
            ragdollPrefab.transform.localScale = Vector3.one;
            isRegdoll = false;
            #endregion
        }

    }

    void Roll()
    {
        if (playerType == PlayerType.Player)
        {
            anim.SetTrigger("Rolling");
            // �÷��̾��� �� �������� ���� ���մϴ�.
            rb.AddForce(Camera.main.transform.forward * rollForce, ForceMode.Impulse);
        }
    }

    private void MovePlayer()
    {
        // �÷��̾��� ���� �Է��� �����ɴϴ�.
        float horizontalInput = Input.GetAxis("Horizontal");
        // �÷��̾��� ���� �Է��� �����ɴϴ�.
        float verticalInput = Input.GetAxis("Vertical");

        // ���� �� ���� �Է��� ����Ͽ� �̵� ������ ����մϴ�.
        movementDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        isMove = Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0;
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
            skyRagdoll = false;
            // ������ �����ϰ� �ȴ�
            isJumping = false;
            anim.SetBool("jumpBool", false);
            if (isRegdoll)
            {
                StartCoroutine(ChangeRagdoll());
                skyRagdoll = false;
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

    #region OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RollingTrap") && !isDead && !invincibility)
        {
            // ����ϰ�, �������Ѵ�
            Die();
        }
        if (other.gameObject.CompareTag("ChangeZone"))
        {
            // Player�� RespawnPoint Ȱ��ȭ
            GameObject player = GameObject.Find("FourStage").transform.Find("FinalPlayer").gameObject;
            GameObject respawnPoint = GameObject.Find("FourStage").transform.Find("RespawnPoint").gameObject;
            player.SetActive(true);
            respawnPoint.SetActive(true);

            // PrioritySetting ����
            PrioritySetting setting = GameObject.FindObjectOfType<PrioritySetting>();
            setting.buttonFreeLook1();

            // FreeLook ī�޶� ����
            GameObject freelook1 = freeLookCamera1.gameObject;
            GameObject freelook2 = freeLookCamera2.gameObject;
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
    #endregion

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
        // ���� ī��Ʈ�� �þ��, UI�� ������Ʈ�Ѵ�.
        isDead = true;
        dieCount++;
        UpdateKillCountText();
        switch (playerType)
        {
            case PlayerType.Player:
                freeLookCamera1.Follow = gameObject.transform.Find("RagdollPrefab");
                freeLookCamera1.LookAt = gameObject.transform.Find("RagdollCm").transform;
                StopAllCoroutines();
                StartCoroutine(RespawnPlayer());
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
        GameObject newRagdoll = transform.Find("RagdollPrefab").gameObject;
        newRagdoll.transform.position = gameObject.transform.position;


        newRagdoll.SetActive(true);


        tipUI = GameObject.Find("EventSystem").transform.GetComponent<TipUIManager>();


        // ���� �÷��̾� ������Ʈ�� �������� ����, �������� ������Ʈ�� ���ڸ� �����Ѵ�.
        newPlayer.name = "Player" + (dieCount + 1);


        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();
        playerCode.isJumping = false;

        // �� �÷��̾ �������� �ʵ��� ����
        playerCode.isDead = true;

        GameObject image_DieUI = GameObject.Find("Canvas").transform.Find("image_DieUI").gameObject;
        image_DieUI.SetActive(true);

        ShootController shoot = gameObject.GetComponent<ShootController>();
        shoot.SetCanShoot(false);
        ShootController shootController = newPlayer.GetComponent<ShootController>();
        shootController.SetCanShoot(false);

        Item item = GameObject.FindObjectOfType<Item>();
        item.itemOK = false;
        item.itemType = Item.ItemType.None;

        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCode.cameraTransform = camera.transform;
        Transform newRespawnPoint = GameObject.Find("RespawnPoint").transform;
        playerCode.RespawnPoint = newRespawnPoint;

        // ���� �÷��̾��� Rigidbody ȸ�� ������ �����մϴ�.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
        
        yield return new WaitForSeconds(2.0f);

        

        Transform newShootPosTransform = newPlayer.GetComponent<Transform>().transform.Find("ShootPos");
        Transform newLookAtTransform  = newPlayer.GetComponent<Transform>().transform.Find("cm");

        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);
        if (freeLookCamera1 != null)
        {


            image_DieUI.SetActive(false);
            freeLookCamera1.Follow = newPlayer.transform;
            freeLookCamera1.LookAt = newLookAtTransform;
        }

        // ī�޶� ������ �Ϸ�� �� �� �÷��̾��� �̵��� Ȱ��ȭ
        playerCode.isDead = false;

        tipUI.ShowTipUI();

        // ���� �÷��̾�� TestPlayerMove ��ũ��Ʈ�� �����Ͽ� ��ü�� ����ϴ�.
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());
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

        // �� �÷��̾��� �̵��� �Ͻ������� ��Ȱ��ȭ
        playerCode.isDead = true;

        // �� �÷��̾��� Ÿ���� ġŲ���� ����
        playerCode.playerType = PlayerType.Chicken;

        // ȭ�鿡 ǥ�õ� �ؽ�Ʈ ������Ʈ Ȱ��ȭ
        GameObject image_DieUI = GameObject.Find("Canvas").transform.Find("image_DieUI").gameObject;
        image_DieUI.SetActive(true);

        tipUI = GameObject.Find("EventSystem").transform.GetComponent<TipUIManager>();

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
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCode.cameraTransform = camera.transform;

        // �� ������ ���� ����
        Transform newRespawnPoint = GameObject.Find("ChickenRespawnPoint").transform;
        playerCode.RespawnPointChicken = newRespawnPoint;

        

        // ���� �÷��̾��� Rigidbody ���� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        // 2�� ���
        yield return new WaitForSeconds(2.0f);

        // �� �÷��̾��� ShootPos ���� �� ShootController Ȱ��ȭ
        Transform newShootPosTransform = newPlayer.GetComponent<Transform>().transform.Find("ShootPos");
        Transform newLookAtTransform = newPlayer.GetComponent<Transform>().transform.Find("cm");

        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);

        if (freeLookCamera2 != null)
        {
            // �ؽ�Ʈ ��Ȱ��ȭ �� ī�޶� Follow, LookAt ���� ������Ʈ
            image_DieUI.SetActive(false);
            freeLookCamera2.Follow = newPlayer.transform;
            freeLookCamera2.LookAt = newLookAtTransform;
        }

        // �� �÷��̾��� �̵� Ȱ��ȭ
        playerCode.isDead = false;

        tipUI.ShowTipUI();



        // ���� �÷��̾��� ��ũ��Ʈ �����Ͽ� ��ü�� ����
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());
    }
}
