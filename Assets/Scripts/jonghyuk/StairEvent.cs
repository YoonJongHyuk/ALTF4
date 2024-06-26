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

                // �� �����Ӹ��� ����մϴ�.
                yield return null;
            }

            // ��Ȯ�� ��ġ�� ������ ��ġ�� ����ϴ�.
            stairBodyList[i].transform.position = stairPointList[i].transform.position;

            // delayTime��ŭ ����մϴ�.
            yield return new WaitForSeconds(delayTime);
        }

        isTrue = false;
        movementCoroutine = null;
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("��");
        Destroy(gameObject);
    }

}