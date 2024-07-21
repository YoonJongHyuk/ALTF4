using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscManager : MonoBehaviour
{
    public GameObject EscPanel;
    public GameObject EscAskPanel;

    public Button myButton;

    bool isTrue = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isTrue)
        {
            EscButtonActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isTrue)
        {
            EscButtonActive(false);
        }
    }


    public void EscButtonActive(bool value)
    {
        EscPanel.SetActive(value);
        isTrue = value;
        if(value)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            EventSystem.current.SetSelectedGameObject(null);
            Time.timeScale = 1f;
        }

        Cursor.visible = value;
    }

    public void RestartButtonActive()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void StartSceneButtonActive()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void EscAskPanelButtonActive(bool value)
    {
        EscAskPanel.SetActive(value);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
