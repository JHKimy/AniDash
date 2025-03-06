using UnityEngine;
using System.Collections;

public class MeteorStorm : MonoBehaviour
{
    public GameObject missilePrefab; //  미사일 프리팹
    public float spawnRadius = 20f;  //  생성 반경
    public float minSpawnInterval = 0.5f; //  최소 생성 간격
    public float maxSpawnInterval = 2f;  //  최대 생성 간격
    public int minMissileCount = 3; //  최소 생성 개수
    public int maxMissileCount = 7; //  최대 생성 개수

    public float minHeight = 15f; //  미사일이 생성되는 최소 높이
    public float maxHeight = 25f; //  미사일이 생성되는 최대 높이

    private bool isStormActive = false;

    void Start()
    {
        // StartCoroutine(SpawnMeteorStorm());
    }
    IEnumerator SpawnMeteorStorm()
    {
        isStormActive = true;

        while (isStormActive)
        {
            //  랜덤한 개수의 미사일 생성
            int missileCount = Random.Range(minMissileCount, maxMissileCount);

            for (int i = 0; i < missileCount; i++)
            {
                SpawnMissile();
            }

            //  랜덤한 시간 간격으로 다음 폭격 실행
            float nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }
    void SpawnMissile()
    {
        //  랜덤 위치 계산
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius), // X축 랜덤
            Random.Range(minHeight, maxHeight),     // Y축 (높이) 랜덤
            Random.Range(-spawnRadius, spawnRadius) // Z축 랜덤
        );

        //  미사일 생성
        Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
    }
    //  폭격 시작 / 중지 기능 추가
    public void StartStorm() => StartCoroutine(SpawnMeteorStorm());
    public void StopStorm() => isStormActive = false;

}
