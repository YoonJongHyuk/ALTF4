using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipUIManager : MonoBehaviour
{
    // TextMeshPro 객체를 Inspector에서 설정할 수 있도록 public으로 선언
    public TMP_Text textComponent;

    public GameObject tipUI;

    // 무작위로 선택할 문자열 리스트
    public List<string> stringList;

    public void ShowTipUI()
    {
        StopCoroutine(ShowUI());
        StartCoroutine(ShowUI());
    }

    
    

    


    IEnumerator ShowUI()
    {
        tipUI.SetActive(true);
        // stringList가 비어 있지 않은지 확인
        if (stringList != null && stringList.Count > 0)
        {
            // 문자열 리스트에서 무작위로 하나의 문자열 선택
            int randomIndex = Random.Range(0, stringList.Count);
            string randomString = stringList[randomIndex];

            // 선택된 문자열을 TMP_Text 컴포넌트에 설정
            textComponent.text = randomString;
        }
        else
        {
            Debug.LogError("String list is empty or not assigned.");
        }
        yield return new WaitForSeconds(3f);
        tipUI.SetActive(false);
    }
}
