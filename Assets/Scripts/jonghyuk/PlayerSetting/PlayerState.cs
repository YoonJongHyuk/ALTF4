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
        // �÷��̾��� �� �������� ���� ���մϴ�.
        manager.rb.AddForce(Camera.main.transform.forward * manager.rollForce, ForceMode.Impulse);
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
        float speed = inputMagnitude * manager.maximumSpeed;

        // ī�޶��� ȸ���� �������� �̵� ������ ��ȯ�մϴ�.
        movementDirection = Quaternion.AngleAxis(manager.cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        // �ӵ��� ����մϴ�.
        Vector3 velocity = movementDirection * speed;
        // Rigidbody�� ����Ͽ� �÷��̾��� ��ġ�� �̵��մϴ�.
        manager.rb.MovePosition(manager.rb.position + velocity * Time.fixedDeltaTime);

        // �̵� �������� �÷��̾��� Y�� ȸ���� �����մϴ�.
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * manager.rotationSpeed);
        }

        // Animator�� �ִ� ��� �ִϸ��̼� ���¸� ������Ʈ�մϴ�.
        if (manager.anim != null)
        {
            // �÷��̾ �����̰� �ִ��� ���θ� ����մϴ�.
            bool isMoving = Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0;
            // �ִϸ������� 'moveBool' �Ķ���͸� ������Ʈ�մϴ�.
            manager.anim.SetBool("moveBool", isMoving);
        }
        else
        {
            // Animator ������Ʈ�� ������ ��� ����մϴ�.
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
