using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TestMove : MonoBehaviour
{

    public float maximumSpeed = 3f;
    public float rotationSpeed = 20f;

    public bool isDead = false;

    Animator anim;

    Transform headTransform;


    public Transform cameraTransform;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Ragdoll();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    void Ragdoll()
    {
        GameObject player = transform.Find("Guard02").gameObject;
        player.gameObject.SetActive(false);
        GameObject ragdollPrefab = transform.Find("RagdollPrefab").gameObject;
        ragdollPrefab.gameObject.SetActive(true);
        StartCoroutine(ChangeRagdoll());
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

    IEnumerator ChangeRagdoll()
    {
        yield return new WaitForSeconds(2.0f);
        print(headTransform);
        GameObject player = transform.Find("Guard02").gameObject;
        player.gameObject.SetActive(true);
        GameObject ragdollPrefab = transform.Find("RagdollPrefab").gameObject;
        ragdollPrefab.gameObject.SetActive(false);
        
    }

    bool IsForward()
    {
        // �Ӹ� Ʈ�������� ���� Z�� ������ ����Ͽ� �յڸ� �Ǵ��մϴ�.
        // forwardThreshold�� ���� Z���� ���� �������� �մϴ�.
        return headTransform.forward.z > 0.5f;
    }

}
