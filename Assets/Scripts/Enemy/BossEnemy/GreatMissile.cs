using UnityEngine;
using System.Collections;

public class GreatMissile : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFall;
    private float gainPower;
    private float scaleValue;
    private int damage = 30;

    private float speed;  // 속도 변수 추가

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //  미사일 속도를 랜덤 값으로 설정 (5 ~ 15)
        speed = Random.Range(5f, 15f);

        //처음부터 중력의 영향을 받아 아래로 이동
        rb.useGravity = true;
        rb.linearVelocity = Vector3.down * speed;

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
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerState _playerstate = collision.gameObject.GetComponent<PlayerState>();
            if (_playerstate != null)
            {
                _playerstate.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
