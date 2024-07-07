using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None; // �� ��ȯ �� Ŀ�� ��� ����
            Cursor.visible = true; // Ŀ�� ���̰� ����
            SceneManager.LoadScene(0);
        }
    }
}
