using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    PlayerManager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForit()
    {
        manager.invincibility = true;
        yield return new WaitForSeconds(1f);
        manager.invincibility = false;
        yield return new WaitForSeconds(1f);
        manager.isRoll = true;

    }

    void Roll()
    {
        // 플레이어의 앞 방향으로 힘을 가합니다.
        manager.rb.AddForce(Camera.main.transform.forward * manager.rollForce, ForceMode.Impulse);
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
        float speed = inputMagnitude * manager.maximumSpeed;

        // 카메라의 회전을 기준으로 이동 방향을 변환합니다.
        movementDirection = Quaternion.AngleAxis(manager.cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        // 속도를 계산합니다.
        Vector3 velocity = movementDirection * speed;
        // Rigidbody를 사용하여 플레이어의 위치를 이동합니다.
        manager.rb.MovePosition(manager.rb.position + velocity * Time.fixedDeltaTime);

        // 이동 방향으로 플레이어의 Y축 회전을 설정합니다.
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * manager.rotationSpeed);
        }

        // Animator가 있는 경우 애니메이션 상태를 업데이트합니다.
        if (manager.anim != null)
        {
            // 플레이어가 움직이고 있는지 여부를 계산합니다.
            bool isMoving = Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0;
            // 애니메이터의 'moveBool' 파라미터를 업데이트합니다.
            manager.anim.SetBool("moveBool", isMoving);
        }
        else
        {
            // Animator 컴포넌트가 없으면 경고를 출력합니다.
            Debug.LogWarning("Animator component is missing!");
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !manager.isJumping)
        {
            manager.isJumping = true;
            manager.rb.AddForce(Vector3.up * manager.jumpForce, ForceMode.Impulse);
            manager.anim.SetBool("jumpBool", true);
        }
    }

}
