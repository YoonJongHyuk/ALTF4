using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChickenMove : MonoBehaviour
{
    public float maximumSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 20f;
    public float rollForce = 10f;  // �÷��̾ ���� �� ���� ���� ũ��
    bool invincibility = false;

    public bool isRoll;
    public bool isJumping;
    public bool isDead = false;
    public Transform RespawnPoint;
    public GameObject PlayerPrefab;
    public static int dieCount = 0;  // dieCount ������ static���� �����Ͽ� ��� �ν��Ͻ����� ����
    private Text killCountText;  // killCountText ������ ����

    public Transform cameraTransform;
    private Rigidbody rb;

    Item item;

    private void Start()
    {
        item = GameObject.FindObjectOfType<Item>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isJumping = false;
        isRoll = true;

        // Kill Count Text�� ã���ϴ�
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

        // Left Control Ű �Է��� �����մϴ�.
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
        if (isDead == false)
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
        // ����, �÷��̾ ���� ��´ٸ�
        if (collision.gameObject.layer == 8)
        {
            // ������ �����ϰ� �ȴ�
            isJumping = false;
        }
        // ����, �÷��̾ ������ �ε����ٸ�
        if (collision.gameObject.CompareTag("Trap"))
        {
            // ���� �ʾ��� ���
            if (!isDead)
            {
                // ����ϰ�, �������Ѵ�
                Die();
                StartCoroutine(RespawnPlayer());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "RollingTrap")
        {
            // ���� �ʾ��� ���
            if (!isDead && !invincibility)
            {
                // ����ϰ�, �������Ѵ�
                Die();
                StartCoroutine(RespawnPlayer());
            }
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
        // ���� ī��Ʈ�� �þ��, UI�� ������Ʈ�Ѵ�.
        isDead = true;
        dieCount++;
        UpdateKillCountText();
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
        // �� �÷��̾ �����Ѵ�
        GameObject newPlayer = Instantiate(PlayerPrefab, RespawnPoint.transform.position, RespawnPoint.transform.rotation);

        // ���� �÷��̾� ������Ʈ�� �������� ����, �������� ������Ʈ�� ���ڸ� �����Ѵ�.
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

        Transform newShootPosTransform = newPlayer.transform.Find("ShootPos");
        shootController.shootPos = newShootPosTransform.gameObject;
        shootController.SetCanShoot(true);
        if (freeLookCamera != null)
        {
            text_dieText.SetActive(false);
            freeLookCamera.Follow = newPlayer.transform;
            freeLookCamera.LookAt = newPlayer.transform;
        }
        // ���� �÷��̾�� TestPlayerMove ��ũ��Ʈ�� �����Ͽ� ��ü�� ����ϴ�.
        Destroy(GetComponent<TestPlayerMove>());
        Destroy(GetComponent<ShootController>());
        Destroy(GetComponent<ItemUse>());

        // ī�޶� ������ �Ϸ�� �� �� �÷��̾��� �̵��� Ȱ��ȭ
        playerCode.isDead = false;

        yield return null;
    }
}
