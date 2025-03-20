using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;    // ������ ������
    public int initialSize = 10; // �ʱ� ������Ʈ ����
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
        }
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
