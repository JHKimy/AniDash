using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Enemy2 : MonoBehaviour
{
    public GameObject laserPrefab; // ������ �߻�ü ������
    public Transform firePoint;    // �������� ������ ��ġ

    public float fireRate = 3f;    // �߻� ���� (��)
    private float laserSpeed = 70f; // ������ �ӵ�
    public float laserLifeTime = 3f; // ������ ���� �ð�
    public float knockbackForce = 10f; // �÷��̾� ƨ�ܳ����� ��

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

        // ������ �������� firePoint ��ġ���� ����
        GameObject laser = Instantiate(laserPrefab, firePoint.position, laserRotation);

        // ������ �̵��� ���� Rigidbody �߰�
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = laser.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.linearVelocity = firePoint.forward * laserSpeed; // �������� �̵�

        // �������� �浹 ���� ��ũ��Ʈ �߰�
        Laser laserScript = laser.AddComponent<Laser>();
        laserScript.knockbackForce = knockbackForce;

        // ���� �ð��� ������ ������ ����
        Destroy(laser, laserLifeTime);
    }
}

// ������ �浹 ó��
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
    //    if (other.gameObject.tag == "Player") // �÷��̾� ������ ƨ�ܳ���
    //    {
    //        Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
    //        other.rigidbody.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
    //        Debug.Log("Collision!");
    //    }
    //    // 0.1�� ���� ��ٸ� �� ���� (�浹 ȿ�� ������ ����)
    //    StartCoroutine(DestroyAfterDelay());
    //}

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // �ణ�� ������ �༭ ���� ������ �ð� Ȯ��
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