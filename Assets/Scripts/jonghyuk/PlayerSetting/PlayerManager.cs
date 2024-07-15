using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum PlayerType
{
    Player,
    Chicken
}

public class PlayerManager : MonoBehaviour
{
    


    public PlayerType playerType = PlayerType.Chicken;
    public float maximumSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 20f;
    public float rollForce = 10f;  // 플레이어가 구를 때 가할 힘의 크기
    float fallSpeedThreshold = -20f;

    public bool invincibility = false;
    public bool isRegdoll = false;

    public bool isRoll;
    public bool isJumping;
    public bool isDead = false;

    public Transform RespawnPoint;
    public GameObject PlayerPrefab;


    public Transform RespawnPointChicken;
    public GameObject ChickenPrefab;


    public Transform cameraTransform;
    public Rigidbody rb;

    TipUIManager tipUI;

    PlayerUI playerUI;

    Item item;

    TriggerPlayer triggerPlayer;

    public Animator anim;

    private void Start()
    {
        item = GameObject.FindObjectOfType<Item>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isJumping = false;
        isRoll = true;
        anim = GetComponentInChildren<Animator>();
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
            switch (playerType)
            {
                case PlayerType.Player:
                    StopAllCoroutines();
                    StartCoroutine(triggerPlayer.RespawnPlayer());
                    break;
                case PlayerType.Chicken:
                    StopAllCoroutines();
                    StartCoroutine(triggerPlayer.RespawnChicken());
                    break;
            }
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
        // 플레이어의 앞 방향으로 힘을 가합니다.
        rb.AddForce(Camera.main.transform.forward * rollForce, ForceMode.Impulse);
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

    public void Die()
    {
        anim.SetBool("dieBool", true);
        tipUI = GameObject.Find("EventSystem").transform.GetComponent<TipUIManager>();
        // 데스 카운트가 늘어나고, UI를 업데이트한다.
        isDead = true;
        playerUI.deathCount++;
        tipUI.ShowTipUI();
        playerUI.UpdateKillCountText();
    }

    public void MovePlayer()
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

}

