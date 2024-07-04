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
    public static int dieCount = 0;  // dieCount 변수를 static으로 선언하여 모든 인스턴스에서 공유
    private Text killCountText;  // killCountText 변수를 선언

    public Transform cameraTransform;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isJumping = false;

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
        // 새 플레이어를 생성합니다.
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
