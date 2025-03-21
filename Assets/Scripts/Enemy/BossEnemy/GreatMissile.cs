using UnityEngine;
using System.Collections;

public class GreatMissile : MonoBehaviour
{
    private ObjectPool pool; // 오브젝트 풀

    private Rigidbody rb;

    public GameObject prefabBox;
    private bool isFall;
    private float gainPower;
    private float scaleValue;
    private int damage = 30;

    private float speed;  // 속도 변수 추가

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        ////  미사일 속도를 랜덤 값으로 설정 (5 ~ 15)
        //speed = Random.Range(5f, 15f);

        ////처음부터 중력의 영향을 받아 아래로 이동
        //rb.useGravity = true;
        //rb.linearVelocity = Vector3.down * speed;

        //StartCoroutine(GainScaleTimer());
        //StartCoroutine(GainScale());
    }

    public void Initialize(ObjectPool objectPool)
    {
        pool = objectPool;

        // 미사일 초기화 (속도, 크기 리셋)
        speed = Random.Range(5f, 15f);
        transform.localScale = Vector3.one;
        rb.linearVelocity = Vector3.down * speed;
        rb.angularVelocity = Vector3.zero;
        isFall = false;
        gainPower = 0;
        scaleValue = 1f;

        StartCoroutine(GainScaleTimer());
        StartCoroutine(GainScale());
    }

    IEnumerator GainScaleTimer()
    {
        yield return new WaitForSeconds(1f);
        isFall = true;
    }

    IEnumerator GainScale()
    {
        while (!isFall)
        {
            gainPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;

            rb.AddTorque(transform.right * gainPower, ForceMode.Acceleration);
            yield return null;
        }
    }

    void FixedUpdate()
    {
        Vector3 direction = Vector3.down;
        rb.linearVelocity = direction * speed; //  랜덤 속도로 이동
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Instantiate(prefabBox, transform.position, Quaternion.identity);
            prefabBox.GetComponent<Box>().SetOriginal(false);
            pool.ReturnObject(gameObject);
            // Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerState _playerstate = collision.gameObject.GetComponent<PlayerState>();
            if (_playerstate != null)
            {
                _playerstate.TakeDamage(damage);
            }
            pool.ReturnObject(gameObject);
            // Destroy(gameObject);
        }
    }
}
