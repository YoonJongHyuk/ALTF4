using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemUse : MonoBehaviour
{
    Item Item;

    float propellerSpeed = 0.85f;

    Rigidbody rb;

    bool useStart = false;

    // Start is called before the first frame update
    void Start()
    {
        //proropellerPos = new Vector3(0, 8f, 0);
        rb = GetComponent<Rigidbody>();
        Item = GameObject.FindObjectOfType<Item>();
        Item.itemOK = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Item.itemOK)
        {
            useStart = true;
            print("버튼 클릭 테스트");
        }

        if (useStart)
        {
            print("클릭 확인 테스트");
            StopAllCoroutines();
            StartCoroutine(PropellerUse());
        }
    }

    

    IEnumerator PropellerUse()
    {
        rb.AddForce(Vector3.up * propellerSpeed, ForceMode.Acceleration);
        yield return null;
        yield return new WaitForSeconds(1.0f);
        Item.itemType = Item.ItemType.None;
        TestPlayerMove testPlayerMove = GetComponent<TestPlayerMove>();
        testPlayerMove.isJumping = true;
        Item.itemOK = false;
        useStart = false;
    }

}
