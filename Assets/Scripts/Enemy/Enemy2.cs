using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public GameObject laserPrefab; // ������ �߻�ü ������
    public Transform firePoint;    // �������� ������ ��ġ

    public float fireRate = 3f;    // �߻� ���� (��)
    public float laserSpeed = 20f; // ������ �ӵ�
    public float laserLifeTime = 3f; // ������ ���� �ð�

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
        // ������ �������� firePoint ��ġ���� ����
        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        // ������ �̵��� ���� Rigidbody �߰�
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = laser.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.linearVelocity = firePoint.forward * laserSpeed; // �������� �̵�

        // ���� �ð��� ������ ������ ����
        Destroy(laser, laserLifeTime);
    }
}
