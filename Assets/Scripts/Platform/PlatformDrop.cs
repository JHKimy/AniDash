using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDrop : MonoBehaviour
{
    public DropObject dropPrefab; // DropObject Ÿ������ ����
    public Transform dropPoint;
    private int poolSize = 5;
    public float dropRate = 2f;
    private Queue<DropObject> dropPool = new Queue<DropObject>();

    void Start()
    {
        Debug.Log("Start �����"); // Start() ���� Ȯ��
        InitializeDropPool();
        StartCoroutine(SpawnDropObjects());
    }

    void InitializeDropPool()
    {
        Debug.Log($"Initializing pool with size: {poolSize}"); // poolSize �� Ȯ��

        for (int i = 0; i < poolSize; i++)
        {
            Debug.Log($"Creating drop object {i + 1}"); // �� �� �����Ǵ��� Ȯ��

            DropObject tempDropObject = Instantiate(dropPrefab); // GameObject�� ����

            if (tempDropObject == null)
            {
                Debug.LogError($"DropObject ������Ʈ�� {dropPrefab.name}�� ����!");
                return;
            }

            tempDropObject.SetPool(this);
            tempDropObject.gameObject.SetActive(false);
            dropPool.Enqueue(tempDropObject);
            Debug.Log("put"); // put �αװ� ��µ�
        }

        Debug.Log($"Pool initialized with {dropPool.Count} objects"); // Ǯ ũ�� Ȯ��
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

        Debug.Log("All objects in pool are active, creating a new one."); // ���� ���� �� �α� ���
        DropObject newDrop = Instantiate(dropPrefab); // GameObject�� ����
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
//            rb.linearVelocity = Vector3.zero; // �ùٸ� �ӵ� �ʱ�ȭ
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
