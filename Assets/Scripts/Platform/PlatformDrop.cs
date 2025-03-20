using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDrop : MonoBehaviour
{
    public DropObject dropPrefab; // DropObject 타입으로 변경
    public Transform dropPoint;
    private int poolSize = 5;
    public float dropRate = 2f;
    private Queue<DropObject> dropPool = new Queue<DropObject>();

    void Start()
    {
        Debug.Log("Start 실행됨"); // Start() 실행 확인
        InitializeDropPool();
        StartCoroutine(SpawnDropObjects());
    }

    void InitializeDropPool()
    {
        Debug.Log($"Initializing pool with size: {poolSize}"); // poolSize 값 확인

        for (int i = 0; i < poolSize; i++)
        {
            Debug.Log($"Creating drop object {i + 1}"); // 몇 개 생성되는지 확인

            DropObject tempDropObject = Instantiate(dropPrefab); // GameObject로 생성

            if (tempDropObject == null)
            {
                Debug.LogError($"DropObject 컴포넌트가 {dropPrefab.name}에 없음!");
                return;
            }

            tempDropObject.SetPool(this);
            tempDropObject.gameObject.SetActive(false);
            dropPool.Enqueue(tempDropObject);
            Debug.Log("put"); // put 로그가 출력됨
        }

        Debug.Log($"Pool initialized with {dropPool.Count} objects"); // 풀 크기 확인
    }

    IEnumerator SpawnDropObjects()
    {
        while (true)
        {
            SpawnDrop();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void SpawnDrop()
    {
        DropObject drop = GetDropObject();
        drop.transform.position = dropPoint.position;
        drop.gameObject.SetActive(true);
    }

    private DropObject GetDropObject()
    {
        DropObject dropOBJ = null;

        int count = dropPool.Count;
        for (int i = 0; i < count; i++)
        {
            dropOBJ = dropPool.Dequeue();
            if (!dropOBJ.gameObject.activeSelf)
            {
                return dropOBJ;
            }
            dropPool.Enqueue(dropOBJ);
        }

        Debug.Log("All objects in pool are active, creating a new one."); // 새로 생성 시 로그 출력
        DropObject newDrop = Instantiate(dropPrefab); // GameObject로 생성
        newDrop.SetPool(this);
        return newDrop;
    }

    public void ReturnToPool(DropObject dropObject)
    {
        dropObject.gameObject.SetActive(false);
        dropPool.Enqueue(dropObject);
    }
}

//public class DropObject : MonoBehaviour
//{
//    public float knockbackForce = 10f;
//    private PlatformDrop platformDrop;
//    private Rigidbody rb;

//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//    }

//    public void SetPool(PlatformDrop dropController)
//    {
//        platformDrop = dropController;
//    }

//    private void OnEnable()
//    {
//        if (rb != null)
//        {
//            rb.linearVelocity = Vector3.zero; // 올바른 속도 초기화
//            rb.angularVelocity = Vector3.zero;
//        }

//        StartCoroutine(DestroyAfterDelay());
//    }

//    void OnCollisionEnter(Collision other)
//    {
//        if (other.gameObject.CompareTag("Player"))
//        {
//            Rigidbody playerRb = other.rigidbody;
//            if (playerRb != null && !playerRb.isKinematic)
//            {
//                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
//                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
//            }
//        }

//        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Floor"))
//        {
//            ReturnToPool();
//        }
//    }

//    IEnumerator DestroyAfterDelay()
//    {
//        yield return new WaitForSeconds(3f);
//        ReturnToPool();
//    }

//    private void ReturnToPool()
//    {
//        platformDrop.ReturnToPool(this);
//    }
//}
