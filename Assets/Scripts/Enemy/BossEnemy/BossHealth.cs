using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public GameObject keyPrefab; // 보스가 죽을 때 생성할 키 프리팹

    public float currentHealth = 100f;
    public float maxHealth = 100f;

    public Slider healthBar; // 월드 스페이스 UI
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
            _bossEnemy.StartCoroutine(_bossEnemy.Die()); // 사망 코루틴 BossEnemy에서 처리
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
        //Debug.Log("보스 사망!");
        _bossEnemy.currentState = BossEnemy.State.Die;
        // 코루틴 시작
        // StartCoroutine(SpawnKeysAndFadeOut());
    }

    //IEnumerator SpawnKeysAndFadeOut()
    //{
    //    // 1초 대기
    //    yield return new WaitForSeconds(1f);

    //    // 키 생성
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

    //    // 완전히 사라지면 제거
    //    Destroy(gameObject);
    //}
}
