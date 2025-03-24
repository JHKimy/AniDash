using UnityEngine;
using System.Collections.Generic;

public class RandomPlatforms : MonoBehaviour
{
    public float speed = 2.0f;  // �̵� �ӵ�
    public float moveRange = 3.0f; // �̵� ���� (X, Z)

    private List<Transform> platforms = new List<Transform>(); // ���� ����Ʈ
    private Dictionary<Transform, Vector3> targetPositions = new Dictionary<Transform, Vector3>(); // ���Ǻ� ��ǥ ��ġ
    private Dictionary<Transform, bool> moveX = new Dictionary<Transform, bool>(); // Ư�� �÷����� Z�����θ� �����̴��� ����



    void Start()
    {
        // ���� ������Ʈ(����)���� �ڵ����� ������
        foreach (Transform child in transform)
        {
            platforms.Add(child);
            // �±� ������� �̵� ��� ����
            if (child.CompareTag("MoveX"))
            {
                moveX[child] = true; // Z�� �̵��� ����
            }
            else if (child.CompareTag("MoveRandom"))
            {
                moveX[child] = false; // �⺻������ XZ �̵� ����
            }
            SetRandomTarget(child);
        }
    }

    void Update()
    {
        // ��� ������ �̵�
        foreach (Transform platform in platforms)
        {
            Vector3 target = targetPositions[platform];
            platform.position = Vector3.MoveTowards(platform.position, target, speed * Time.deltaTime);

            // ��ǥ ������ �����ϸ� ���ο� ��ġ ����
            if (Vector3.Distance(platform.position, target) < 0.1f)
            {
                SetRandomTarget(platform);
            }
        }
    }

    // ������ ��ǥ ��ġ ����
    void SetRandomTarget(Transform platform)
    {
        Vector3 startPos = platform.position;
        
        float randomX = 0;
        float randomY = 0;
        float randomZ = 0;

        if (moveX[platform]) // Z�� �̵��� ����
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
    //        // �浹�� �÷����� Transform�� ���Ѵ�
    //        Transform platform = collision.contacts[0].thisCollider.transform;

    //        // �÷��̾ �� �÷����� �ڽ����� ����
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
