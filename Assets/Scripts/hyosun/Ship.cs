using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    int health = 100;
    public int attackPower;
    void TakeDamage(int value)
    {
        health -= value;
        if(health <=0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TakeDamage(attackPower);
        Destroy(collision.gameObject);
    }
}
