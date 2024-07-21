using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelFader : MonoBehaviour
{
    public CanvasGroup[] canvasGroups;
    public float fadeDuration = 1.0f;
    public bool OnlyOne = false;
    float delayTime1 = 2f;
    float delayTime2 = 4f;
    private float currentTime = 0f;
    private int currentPanelIndex = 0;
    private bool isFading = false;
    private bool hasFadedOut = false;

    private void Awake()
    {
        if (canvasGroups == null || canvasGroups.Length != 2)
        {
            Debug.LogError("Exactly two CanvasGroups should be assigned.");
            return;
        }
    }

    private void Update()
    {
        if (hasFadedOut)
            return;

        currentTime += Time.deltaTime;

        if (!isFading && !OnlyOne)
        {
            if (currentPanelIndex == 0 && currentTime > delayTime1)
            {
                StartCoroutine(FadeOut(canvasGroups[currentPanelIndex]));
                currentPanelIndex++;
                currentTime = 0f;
            }
            else if (currentPanelIndex == 1 && currentTime > delayTime2 - delayTime1)
            {
                StartCoroutine(FadeOut(canvasGroups[currentPanelIndex]));
                hasFadedOut = true;  // Mark as completed
            }
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        isFading = true;
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
        isFading = false;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
        cg.interactable = end == 1;
        cg.blocksRaycasts = end == 1;
    }
}
