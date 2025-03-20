using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Slider healthBar; // ���� �����̽� UI
    public Transform healthBarAnchor; // ���� �Ӹ� �� ü�¹� ��ġ ����

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if (healthBar != null && healthBarAnchor != null)
        {
            // 1. ü�¹ٸ� ���� �Ӹ� ���� ��ġ
            healthBar.transform.position = healthBarAnchor.position;

            // 2. ü�¹ٰ� �׻� ī�޶� �ٶ󺸰� �� (2���� ��� �� ����)
            healthBar.transform.LookAt(Camera.main.transform);  // ��� 1: ī�޶� ���� �ٶ�
            // healthBar.transform.forward = Camera.main.transform.forward; // ��� 2: ī�޶� ������ ����
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("���� ���!");
        Destroy(gameObject);
    }
}
