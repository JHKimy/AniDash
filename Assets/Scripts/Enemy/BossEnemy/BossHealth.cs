using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Slider healthBar; // 월드 스페이스 UI
    public Transform healthBarAnchor; // 보스 머리 위 체력바 위치 기준

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if (healthBar != null && healthBarAnchor != null)
        {
            // 1. 체력바를 보스 머리 위에 위치
            healthBar.transform.position = healthBarAnchor.position;

            // 2. 체력바가 항상 카메라를 바라보게 함 (2가지 방법 중 선택)
            healthBar.transform.LookAt(Camera.main.transform);  // 방법 1: 카메라를 직접 바라봄
            // healthBar.transform.forward = Camera.main.transform.forward; // 방법 2: 카메라 방향을 따라감
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
        Debug.Log("보스 사망!");
        Destroy(gameObject);
    }
}
