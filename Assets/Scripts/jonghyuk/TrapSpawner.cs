using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    public GameObject trapPrefab;
    public GameObject trapSpawnPoint;
    Coroutine currentCoroutine;

    private void Start()
    {
        StartCoroutine(TrapSpawnCoroutine(2f));
    }

    IEnumerator TrapSpawnCoroutine(float delyTime)
    {
        GameObject trap = Instantiate(trapPrefab);
        trap.transform.position = trapSpawnPoint.transform.position;
        yield return new WaitForSeconds(delyTime);
        StartCoroutine(TrapSpawnCoroutine(2f));
    }

}
