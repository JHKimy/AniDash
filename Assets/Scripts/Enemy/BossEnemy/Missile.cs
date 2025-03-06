using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float rotateSpeed = 5f;
    private int damage = 20;
    private float hoverTime = 10f; // 3초 동안 Y + 1 적용

    private Rigidbody rb;
    private float timer = 0f; // 시간 측정을 위한 변수

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // 3초 동안만 Y + 1 적용
        timer += Time.fixedDeltaTime;
        Vector3 adjustedTarget = (timer < hoverTime) ?
            new Vector3(target.position.x, target.position.y + 1f, target.position.z) :
            target.position;

        // 타겟을 향하도록 방향 회전
        Vector3 direction = (adjustedTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime);

        // 미사일 이동
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerState _playerstate = collision.gameObject.GetComponent<PlayerState>();
            _playerstate.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
