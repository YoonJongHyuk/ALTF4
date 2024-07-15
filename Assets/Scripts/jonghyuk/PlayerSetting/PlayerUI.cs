using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TipUIManager tipManager;

    public int deathCount = 0;  // dieCount 변수를 static으로 선언하여 모든 인스턴스에서 공유
    private TMP_Text killCountText;  // killCountText 변수를 선언

    // Start is called before the first frame update
    void Start()
    {
        tipManager = GetComponent<TipUIManager>();
    }

    public void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            // killCountText에 dieCount를 할당한다.
            killCountText.text = deathCount.ToString();
        }
    }

}
