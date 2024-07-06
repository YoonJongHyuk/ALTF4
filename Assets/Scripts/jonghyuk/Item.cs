using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    ItemUse itemUse;
    public bool itemOK = false;
    public enum ItemType
    {
        Propeller,
        None
    }

    private void Start()
    {
        itemUse = GameObject.FindObjectOfType<ItemUse>();
    }


    public ItemType itemType = ItemType.None;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shoot"))
        {
            print("충돌 테스트");
            switch (itemType)
            {
                case ItemType.None:
                    itemType = ItemType.Propeller;
                    itemOK = true;
                    break;
            }
        }
    }

}
