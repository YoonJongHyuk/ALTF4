using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StairEvent : MonoBehaviour
{
    public List<GameObject> stairBodyList;
    public List<GameObject> stairPointList;
    Vector3 dir;
    bool isTrue;
    float stairSpeed = 1f;

    Coroutine movementCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        isTrue = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrue)
        {
            movementCoroutine = StartCoroutine(MoveStairsWithDelay(0.05f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTrue = true;
        }
    }

    IEnumerator MoveStairsWithDelay(float delayTime)
    {
        for (int i = 0; i < stairBodyList.Count; i++)
        {
            while (Vector3.Distance(stairBodyList[i].transform.position, stairPointList[i].transform.position) > 0.1f)
            {
                Vector3 dir = stairPointList[i].transform.position - stairBodyList[i].transform.position;
                dir.Normalize();
                stairBodyList[i].transform.position += dir * stairSpeed * Time.deltaTime;

                // 매 프레임마다 대기합니다.
                yield return null;
            }

            // 정확한 위치에 도착시 위치를 맞춥니다.
            stairBodyList[i].transform.position = stairPointList[i].transform.position;

            // delayTime만큼 대기합니다.
            yield return new WaitForSeconds(delayTime);
        }

        isTrue = false;
        movementCoroutine = null;
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
