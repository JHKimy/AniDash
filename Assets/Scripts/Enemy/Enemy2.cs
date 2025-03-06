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
                laser = laserObj.AddComponent<Laser>(); // Laser 컴포넌트 없으면 추가
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
                // rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 빠른 물체 감지
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

// 레이저 충돌 처리
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
//    public GameObject laserPrefab; // 레이저 발사체 프리팹
//    public Transform firePoint;    // 레이저가 나가는 위치

//    public float fireRate = 3f;    // 발사 간격 (초)
//    private float laserSpeed = 10f; // 레이저 속도
//    public float laserLifeTime = 3f; // 레이저 지속 시간
//    public float knockbackForce = 10f; // 플레이어 튕겨나가는 힘

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

//        // 레이저 프리팹을 firePoint 위치에서 생성
//        GameObject laser = Instantiate(laserPrefab, firePoint.position, laserRotation);

//        // 레이저 이동을 위한 Rigidbody 추가
//        Rigidbody rb = laser.GetComponent<Rigidbody>();
//        if (rb == null)
//        {
//            rb = laser.AddComponent<Rigidbody>();
//        }
//        rb.useGravity = false;
//        rb.linearVelocity = firePoint.forward * laserSpeed; // 전방으로 이동

//        // 레이저에 충돌 감지 스크립트 추가
//        Laser laserScript = laser.AddComponent<Laser>();
//        laserScript.knockbackForce = knockbackForce;

//        // 일정 시간이 지나면 레이저 삭제
//        Destroy(laser, laserLifeTime);
//    }
//}

//// 레이저 충돌 처리
//public class Laser : MonoBehaviour
//{
//    public float knockbackForce = 10f;
//    private void Start()
//    {
//        Debug.Log("sdfaasf");
//    }
//    void OnCollisionEnter(Collision other)
//    {
//        if (other.gameObject.tag == "Player") // 플레이어 맞으면 튕겨나감
//        {
//            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
//            other.rigidbody.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
//            Debug.Log("Collision!");
//        }
//        // 0.1초 정도 기다린 후 제거 (충돌 효과 적용을 위해)
//        // StartCoroutine(DestroyAfterDelay());
//        Destroy(gameObject);
//    }

//    IEnumerator DestroyAfterDelay()
//    {
//        yield return new WaitForSeconds(0.1f); // 약간의 지연을 줘서 힘을 적용할 시간 확보
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