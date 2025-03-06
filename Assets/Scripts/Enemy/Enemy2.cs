using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy2 : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform firePoint;
    public int poolSize = 5;

    public float fireRate = 3f;
    public float laserSpeed = 70f;
    private float laserLifeTime = 5f;

    private float nextFireTime = 0f;
    private Queue<Laser> laserPool = new Queue<Laser>();

    void Start()
    {
        InitializeLaserPool();
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireLaser();
            nextFireTime = Time.time + fireRate;
        }
    }

    void InitializeLaserPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject laserObj = Instantiate(laserPrefab);
            Laser laser = laserObj.GetComponent<Laser>(); 
            if (laser == null)
            {
                laser = laserObj.AddComponent<Laser>(); // Laser ������Ʈ ������ �߰�
            }
            laserObj.SetActive(false);
            laserPool.Enqueue(laser);
        }
    }

    void FireLaser()
    {
        if (laserPool.Count > 0)
        {
            Laser laser = laserPool.Dequeue();
            laser.transform.position = firePoint.position;
            laser.transform.rotation = firePoint.rotation * Quaternion.Euler(90, 0, 0);
            laser.gameObject.SetActive(true);

            Rigidbody rb = laser.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = false;
                // rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // ���� ��ü ����
                // rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.linearVelocity = firePoint.forward * laserSpeed;
            }

            StartCoroutine(DisableLaserAfterTime(laser, laserLifeTime));
        }
    }

    IEnumerator DisableLaserAfterTime(Laser laser, float time)
    {
        yield return new WaitForSeconds(time);
        laser.gameObject.SetActive(false);
        laserPool.Enqueue(laser);
    }
}

// ������ �浹 ó��
public class Laser : MonoBehaviour
{
    public float knockbackForce = 10f;

    private void Start()
    {
        Debug.Log("start");
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision");

        if (other.gameObject.tag =="Player")
        {
            Rigidbody playerRb = other.rigidbody;
            if (playerRb != null && !playerRb.isKinematic)
            {
                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);

                Debug.Log("Collision with Player");
            }
        }

        Destroy(gameObject);
        // StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}



//using Unity.VisualScripting;
//using UnityEngine;
//using System.Collections;

//public class Enemy2 : MonoBehaviour
//{
//    public GameObject laserPrefab; // ������ �߻�ü ������
//    public Transform firePoint;    // �������� ������ ��ġ

//    public float fireRate = 3f;    // �߻� ���� (��)
//    private float laserSpeed = 10f; // ������ �ӵ�
//    public float laserLifeTime = 3f; // ������ ���� �ð�
//    public float knockbackForce = 10f; // �÷��̾� ƨ�ܳ����� ��

//    private float nextFireTime = 0f;

//    void Update()
//    {
//        if (Time.time >= nextFireTime)
//        {
//            FireLaser();
//            nextFireTime = Time.time + fireRate;
//        }
//    }

//    void FireLaser()
//    {
//        Quaternion laserRotation = firePoint.rotation * Quaternion.Euler(90, 0, 0);

//        // ������ �������� firePoint ��ġ���� ����
//        GameObject laser = Instantiate(laserPrefab, firePoint.position, laserRotation);

//        // ������ �̵��� ���� Rigidbody �߰�
//        Rigidbody rb = laser.GetComponent<Rigidbody>();
//        if (rb == null)
//        {
//            rb = laser.AddComponent<Rigidbody>();
//        }
//        rb.useGravity = false;
//        rb.linearVelocity = firePoint.forward * laserSpeed; // �������� �̵�

//        // �������� �浹 ���� ��ũ��Ʈ �߰�
//        Laser laserScript = laser.AddComponent<Laser>();
//        laserScript.knockbackForce = knockbackForce;

//        // ���� �ð��� ������ ������ ����
//        Destroy(laser, laserLifeTime);
//    }
//}

//// ������ �浹 ó��
//public class Laser : MonoBehaviour
//{
//    public float knockbackForce = 10f;
//    private void Start()
//    {
//        Debug.Log("sdfaasf");
//    }
//    void OnCollisionEnter(Collision other)
//    {
//        if (other.gameObject.tag == "Player") // �÷��̾� ������ ƨ�ܳ���
//        {
//            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
//            other.rigidbody.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
//            Debug.Log("Collision!");
//        }
//        // 0.1�� ���� ��ٸ� �� ���� (�浹 ȿ�� ������ ����)
//        // StartCoroutine(DestroyAfterDelay());
//        Destroy(gameObject);
//    }

//    IEnumerator DestroyAfterDelay()
//    {
//        yield return new WaitForSeconds(0.1f); // �ణ�� ������ �༭ ���� ������ �ð� Ȯ��
//        Destroy(gameObject);
//    }

//    //void OnCollisionEnter(Collision other)
//    //{
//    //    if (other.gameObject.tag == "Box")
//    //    {
//    //        currentState = State.Hit;
//    //        //Vector3 knockbackDir = (transform.position - other.transform.position).normalized;
//    //        //_rigidbody.AddForce(knockbackDir * knockBackForce, ForceMode.Impulse);
//    //    }
//    //}

//}