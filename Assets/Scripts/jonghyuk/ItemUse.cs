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
            print("��ư Ŭ�� �׽�Ʈ");
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
        rb.AddForce(Vector3.up * 950f, ForceMode.Acceleration);  // ���� �� ũ�� ����

        Item.itemType = Item.ItemType.None;
        PlayerManager manager = GetComponent<PlayerManager>();
        manager.isJumping = true;

        yield return new WaitForFixedUpdate();  // ������ ������Ʈ ���� ������ ����
        yield return new WaitForSeconds(1.65f);
    }

}
