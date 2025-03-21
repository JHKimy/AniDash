using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public GameObject keyPrefab; // ������ ���� �� ������ Ű ������

    public float currentHealth = 100f;
    public float maxHealth = 100f;

    public Slider healthBar; // ���� �����̽� UI
    private BossEnemy _bossEnemy;

    private void Awake()
    {
        _bossEnemy = GetComponent<BossEnemy>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        // Debug.Log("Boss health : " + currentHealth);
        // Debug.Log("healthBar.value : " + healthBar.value);
    }

    void LateUpdate()
    {
        healthBar.transform.LookAt(Camera.main.transform);
    }
    public void TakeDamage(float damage)
    {
        if (_bossEnemy.currentState == BossEnemy.State.Die) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            _bossEnemy.StartCoroutine(_bossEnemy.Die()); // ��� �ڷ�ƾ BossEnemy���� ó��
        }
    }
    //public void TakeDamage(float damage)
    //{
    //    currentHealth -= damage;
    //    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    //    UpdateHealthBar();

    //    if (currentHealth <= 0)
    //    {
    //        Die();
    //    }
    //}

    void UpdateHealthBar()
    {
        healthBar.value = currentHealth / maxHealth;
    }

    void Die()
    {
        //Debug.Log("���� ���!");
        _bossEnemy.currentState = BossEnemy.State.Die;
        // �ڷ�ƾ ����
        // StartCoroutine(SpawnKeysAndFadeOut());
    }

    //IEnumerator SpawnKeysAndFadeOut()
    //{
    //    // 1�� ���
    //    yield return new WaitForSeconds(1f);

    //    // Ű ����
    //    for (int i = 0; i < 10; i++)
    //    {
    //        Vector3 spawnPos = transform.position + Vector3.up * 2f;
    //        Vector3 realSpawnPos = spawnPos + Random.insideUnitSphere * 2f;

    //        GameObject key = Instantiate(keyPrefab, realSpawnPos, Quaternion.identity);

    //        Rigidbody rb = key.GetComponent<Rigidbody>();
    //        if (rb != null)
    //        {
    //            Vector3 randomDir = Random.onUnitSphere;
    //            rb.AddForce(randomDir * 10f, ForceMode.Impulse);
    //        }
    //    }

    //    // ������ ������� ����
    //    Destroy(gameObject);
    //}
}
