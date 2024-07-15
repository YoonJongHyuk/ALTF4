using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    Item Item;

    Rigidbody rb;

    private Coroutine propellerCoroutine;

    public bool useStart = false;

    // Start is called before the first frame update
    void Start()
    {
        //proropellerPos = new Vector3(0, 8f, 0);
        rb = GetComponent<Rigidbody>();
        Item = GameObject.FindObjectOfType<Item>();
        Item.itemOK = false;
    }


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Item.itemOK)
        {
            print("버튼 클릭 테스트");
            useStart = true;
        }
        if (useStart)
        {
            useStart = false;
            Item.itemOK = false;
            if (propellerCoroutine != null)
            {
                StopCoroutine(propellerCoroutine);
            }
            propellerCoroutine = StartCoroutine(PropellerUse());
        }
    }



    IEnumerator PropellerUse()
    {
        rb.AddForce(Vector3.up * 950f, ForceMode.Acceleration);  // 힘을 더 크게 설정

        Item.itemType = Item.ItemType.None;
        PlayerManager manager = GetComponent<PlayerManager>();
        manager.isJumping = true;

        yield return new WaitForFixedUpdate();  // 고정된 업데이트 루프 내에서 실행
        yield return new WaitForSeconds(1.65f);
    }

}
