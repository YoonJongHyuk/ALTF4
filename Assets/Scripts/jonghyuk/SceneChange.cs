using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Image LoadingScene;

    bool startTimer = false;
    float currentTime;
    int delayTime = 3;

    private void Awake()
    {
        startTimer = false;
        currentTime = 0f;
    }

    private void Update()
    {
        if (startTimer)
        {
            currentTime += Time.deltaTime;
            if (currentTime > delayTime)
            {
                startTimer = false;
                SceneManager.LoadScene(1);
            }
        }
    }
    public void NextScene()
    {
        LoadingScene.gameObject.SetActive(true);
        startTimer = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
