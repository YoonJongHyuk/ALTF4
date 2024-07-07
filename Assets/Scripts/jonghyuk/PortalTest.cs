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
            Cursor.lockState = CursorLockMode.None; // 씬 전환 전 커서 잠금 해제
            Cursor.visible = true; // 커서 보이게 설정
            SceneManager.LoadScene(0);
        }
    }
}
