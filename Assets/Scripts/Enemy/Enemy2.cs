using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy2 : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform firePoint;
    public int poolSize = 5;

    // public float fireRate = 3f;
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
            nextFireTime = Time.time + Random.Range(0.5f, 2f);
        }
    }

    void InitializeLaserPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewLaser();
        }
    }

    Laser CreateNewLaser()
    {
        GameObject laserObj = Instantiate(laserPrefab);
        Laser laser = laserObj.GetComponent<Laser>();
        if (laser == null)
        {
            laser = laserObj.AddComponent<Laser>();
        }

        laser.SetPool(this);  // 오브젝트 풀을 연결
        laserObj.SetActive(false);
        laserPool.Enqueue(laser);
        return laser;
    }

    void FireLaser()
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
            rb.linearVelocity = firePoint.forward * laserSpeed;
        }

        StartCoroutine(DisableLaserAfterTime(laser, laserLifeTime));
    }

    IEnumerator DisableLaserAfterTime(Laser laser, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool(laser);
    }

    public void ReturnToPool(Laser laser)
    {
        laser.gameObject.SetActive(false);
        laserPool.Enqueue(laser);
    }
}

public class Laser : MonoBehaviour
{
    public float knockbackForce = 30f;
    private Enemy2 enemy2;

    public void SetPool(Enemy2 enemy)
    {
        enemy2 = enemy;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody playerRb = other.rigidbody;
            if (playerRb != null && !playerRb.isKinematic)
            {
                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            }
        }

        enemy2.ReturnToPool(this);
    }
}
