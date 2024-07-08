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
        // 각 계단의 몸체(stairBody)와 목표 지점(stairPoint) 사이의 거리 확인 및 이동
        for (int i = 0; i < stairBodyList.Count; i++)
        {
            // 현재 계단 몸체가 목표 지점에 도달할 때까지 반복
            while (Vector3.Distance(stairBodyList[i].transform.position, stairPointList[i].transform.position) > 0.1f)
            {
                // 이동 방향 벡터 계산
                Vector3 dir = stairPointList[i].transform.position - stairBodyList[i].transform.position;
                dir.Normalize(); // 방향 벡터를 정규화

                // 계단 몸체를 목표 지점 방향으로 이동
                stairBodyList[i].transform.position += dir * stairSpeed * Time.deltaTime;

                // 매 프레임마다 대기
                yield return null;
            }

            // 목표 지점에 정확히 위치하도록 위치를 맞춤
            stairBodyList[i].transform.position = stairPointList[i].transform.position;

            // delayTime만큼 대기
            yield return new WaitForSeconds(delayTime);
        }

        // 작업 완료 후 isTrue를 false로 설정하고, movementCoroutine을 null로 설정
        isTrue = false;
        movementCoroutine = null;

        // 계단 파괴 코루틴 시작
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
