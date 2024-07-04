using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerMove : MonoBehaviour
{
    public float maximumSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 20f;
    private bool isJumping;
    public bool isDead = false;
    public Transform RespawnPoint;
    public GameObject PlayerPrefab;
    public static int dieCount = 0;  // dieCount ������ static���� �����Ͽ� ��� �ν��Ͻ����� ����
    private Text killCountText;  // killCountText ������ ����

    public Transform cameraTransform;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isJumping = false;

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
    }

    private void FixedUpdate()
    {
        if(isDead == false)
        {
            MovePlayer();
        }
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

    //private void RotatePlayer()
    //{
    //    float horizontalInput = Input.GetAxis("Horizontal");
    //    float verticalInput = Input.GetAxis("Vertical");

    //    Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
    //    if (movementDirection != Vector3.zero)
    //    {
    //        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
    //        Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
    //    }
    //}

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
        if (collision.gameObject.layer == 8)
        {
            isJumping = false;
        }
        if (collision.gameObject.CompareTag("Trap"))
        {
            if (!isDead)
            {
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
        isDead = true;
        dieCount++;
        UpdateKillCountText();
    }

    private void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            killCountText.text = dieCount.ToString();
        }
    }

    IEnumerator RespawnPlayer()
    {
        // �� �÷��̾ �����մϴ�.
        GameObject newPlayer = Instantiate(PlayerPrefab, RespawnPoint.transform.position, RespawnPoint.transform.rotation);
        newPlayer.name = "Player" + dieCount;
        TestPlayerMove playerCode = newPlayer.GetComponent<TestPlayerMove>();
        GameObject text_dieText = GameObject.Find("Canvas").transform.Find("text_dieText").gameObject;
        text_dieText.SetActive(true);

        ShootController shoot = gameObject.GetComponent<ShootController>();
        shoot.SetCanShoot(false);
        ShootController shootController = newPlayer.GetComponent<ShootController>();
        shootController.SetCanShoot(false);

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
