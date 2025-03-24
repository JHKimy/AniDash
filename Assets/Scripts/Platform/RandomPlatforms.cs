using UnityEngine;
using System.Collections.Generic;

public class RandomPlatforms : MonoBehaviour
{
    public float speed = 2.0f;  // 이동 속도
    public float moveRange = 3.0f; // 이동 범위 (X, Z)

    private List<Transform> platforms = new List<Transform>(); // 발판 리스트
    private Dictionary<Transform, Vector3> targetPositions = new Dictionary<Transform, Vector3>(); // 발판별 목표 위치
    private Dictionary<Transform, bool> moveX = new Dictionary<Transform, bool>(); // 특정 플랫폼이 Z축으로만 움직이는지 여부



    void Start()
    {
        // 하위 오브젝트(발판)들을 자동으로 가져옴
        foreach (Transform child in transform)
        {
            platforms.Add(child);
            // 태그 기반으로 이동 방식 설정
            if (child.CompareTag("MoveX"))
            {
                moveX[child] = true; // Z축 이동만 가능
            }
            else if (child.CompareTag("MoveRandom"))
            {
                moveX[child] = false; // 기본적으로 XZ 이동 가능
            }
            SetRandomTarget(child);
        }
    }

    void Update()
    {
        // 모든 발판을 이동
        foreach (Transform platform in platforms)
        {
            Vector3 target = targetPositions[platform];
            platform.position = Vector3.MoveTowards(platform.position, target, speed * Time.deltaTime);

            // 목표 지점에 도착하면 새로운 위치 설정
            if (Vector3.Distance(platform.position, target) < 0.1f)
            {
                SetRandomTarget(platform);
            }
        }
    }

    // 랜덤한 목표 위치 설정
    void SetRandomTarget(Transform platform)
    {
        Vector3 startPos = platform.position;
        
        float randomX = 0;
        float randomY = 0;
        float randomZ = 0;

        if (moveX[platform]) // Z축 이동만 가능
        { 
            randomX = Random.Range(-moveRange, moveRange);
        }
        else
        {
            randomX = Random.Range(-moveRange, moveRange);
            randomY = Random.Range(-moveRange, moveRange);
            randomZ = Random.Range(-moveRange, moveRange);
        }
        Vector3 newTarget = new Vector3(startPos.x + randomX, startPos.y + randomY, startPos.z + randomZ);

        targetPositions[platform] = newTarget;
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        // 충돌한 플랫폼의 Transform을 구한다
    //        Transform platform = collision.contacts[0].thisCollider.transform;

    //        // 플레이어를 그 플랫폼의 자식으로 설정
    //        collision.transform.parent = platform;
    //    }
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        collision.transform.parent = null;
    //    }
    //}

}
