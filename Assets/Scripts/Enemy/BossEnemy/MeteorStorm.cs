using UnityEngine;
using System.Collections;

public class MeteorStorm : MonoBehaviour
{
    public GameObject missilePrefab; //  �̻��� ������
    public float spawnRadius = 20f;  //  ���� �ݰ�
    public float minSpawnInterval = 0.5f; //  �ּ� ���� ����
    public float maxSpawnInterval = 2f;  //  �ִ� ���� ����
    public int minMissileCount = 3; //  �ּ� ���� ����
    public int maxMissileCount = 7; //  �ִ� ���� ����

    public float minHeight = 15f; //  �̻����� �����Ǵ� �ּ� ����
    public float maxHeight = 25f; //  �̻����� �����Ǵ� �ִ� ����

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
            //  ������ ������ �̻��� ����
            int missileCount = Random.Range(minMissileCount, maxMissileCount);

            for (int i = 0; i < missileCount; i++)
            {
                SpawnMissile();
            }

            //  ������ �ð� �������� ���� ���� ����
            float nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }
    void SpawnMissile()
    {
        //  ���� ��ġ ���
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius), // X�� ����
            Random.Range(minHeight, maxHeight),     // Y�� (����) ����
            Random.Range(-spawnRadius, spawnRadius) // Z�� ����
        );

        //  �̻��� ����
        Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
    }
    //  ���� ���� / ���� ��� �߰�
    public void StartStorm() => StartCoroutine(SpawnMeteorStorm());
    public void StopStorm() => isStormActive = false;

}
