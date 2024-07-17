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
        // 머리 트랜스폼의 로컬 Z축 방향을 사용하여 앞뒤를 판단합니다.
        // forwardThreshold는 로컬 Z축의 값을 기준으로 합니다.
        return headTransform.forward.z > 0.5f;
    }

}
