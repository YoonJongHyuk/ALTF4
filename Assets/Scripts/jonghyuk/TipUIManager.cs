using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipUIManager : MonoBehaviour
{
    // TextMeshPro ��ü�� Inspector���� ������ �� �ֵ��� public���� ����
    public TMP_Text textComponent;

    public GameObject tipUI;

    // �������� ������ ���ڿ� ����Ʈ
    public List<string> stringList;

    public void ShowTipUI()
    {
        StopCoroutine(ShowUI());
        StartCoroutine(ShowUI());
    }

    
    

    


    IEnumerator ShowUI()
    {
        tipUI.SetActive(true);
        // stringList�� ��� ���� ������ Ȯ��
        if (stringList != null && stringList.Count > 0)
        {
            // ���ڿ� ����Ʈ���� �������� �ϳ��� ���ڿ� ����
            int randomIndex = Random.Range(0, stringList.Count);
            string randomString = stringList[randomIndex];

            // ���õ� ���ڿ��� TMP_Text ������Ʈ�� ����
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
