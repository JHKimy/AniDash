using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public GameObject laserPrefab; // 레이저 발사체 프리팹
    public Transform firePoint;    // 레이저가 나가는 위치

    public float fireRate = 3f;    // 발사 간격 (초)
    public float laserSpeed = 20f; // 레이저 속도
    public float laserLifeTime = 3f; // 레이저 지속 시간

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireLaser();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireLaser()
    {
        // 레이저 프리팹을 firePoint 위치에서 생성
        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        // 레이저 이동을 위한 Rigidbody 추가
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = laser.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.linearVelocity = firePoint.forward * laserSpeed; // 전방으로 이동

        // 일정 시간이 지나면 레이저 삭제
        Destroy(laser, laserLifeTime);
    }
}
