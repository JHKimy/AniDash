using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Enemy2 : MonoBehaviour
{
    public GameObject laserPrefab; // 레이저 발사체 프리팹
    public Transform firePoint;    // 레이저가 나가는 위치

    public float fireRate = 3f;    // 발사 간격 (초)
    private float laserSpeed = 70f; // 레이저 속도
    public float laserLifeTime = 3f; // 레이저 지속 시간
    public float knockbackForce = 10f; // 플레이어 튕겨나가는 힘

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
        Quaternion laserRotation = firePoint.rotation * Quaternion.Euler(90, 0, 0);

        // 레이저 프리팹을 firePoint 위치에서 생성
        GameObject laser = Instantiate(laserPrefab, firePoint.position, laserRotation);

        // 레이저 이동을 위한 Rigidbody 추가
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = laser.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.linearVelocity = firePoint.forward * laserSpeed; // 전방으로 이동

        // 레이저에 충돌 감지 스크립트 추가
        Laser laserScript = laser.AddComponent<Laser>();
        laserScript.knockbackForce = knockbackForce;

        // 일정 시간이 지나면 레이저 삭제
        Destroy(laser, laserLifeTime);
    }
}

// 레이저 충돌 처리
public class Laser : MonoBehaviour
{
    public float knockbackForce;
    private void Start()
    {
        Debug.Log("sdfaasf");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            if (playerRb != null && !playerRb.isKinematic)
            {
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            }

            StartCoroutine(DestroyAfterDelay());
        }
    }
    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Player") // 플레이어 맞으면 튕겨나감
    //    {
    //        Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
    //        other.rigidbody.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
    //        Debug.Log("Collision!");
    //    }
    //    // 0.1초 정도 기다린 후 제거 (충돌 효과 적용을 위해)
    //    StartCoroutine(DestroyAfterDelay());
    //}

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // 약간의 지연을 줘서 힘을 적용할 시간 확보
        Destroy(gameObject);
    }

    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Box")
    //    {
    //        currentState = State.Hit;
    //        //Vector3 knockbackDir = (transform.position - other.transform.position).normalized;
    //        //_rigidbody.AddForce(knockbackDir * knockBackForce, ForceMode.Impulse);
    //    }
    //}

}