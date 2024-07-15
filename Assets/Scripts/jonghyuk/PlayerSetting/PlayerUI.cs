using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TipUIManager tipManager;

    public int deathCount = 0;  // dieCount ������ static���� �����Ͽ� ��� �ν��Ͻ����� ����
    private TMP_Text killCountText;  // killCountText ������ ����

    // Start is called before the first frame update
    void Start()
    {
        tipManager = GetComponent<TipUIManager>();
    }

    public void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            // killCountText�� dieCount�� �Ҵ��Ѵ�.
            killCountText.text = deathCount.ToString();
        }
    }

}
