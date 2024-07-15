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
    public float rollForce = 10f;  // �÷��̾ ���� �� ���� ���� ũ��
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
        // �÷��̾��� �� �������� ���� ���մϴ�.
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
        // ���� ī��Ʈ�� �þ��, UI�� ������Ʈ�Ѵ�.
        isDead = true;
        playerUI.deathCount++;
        tipUI.ShowTipUI();
        playerUI.UpdateKillCountText();
    }

    public void MovePlayer()
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

