using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float rotateSpeed = 5f;
    private int damage = 20;
    private float hoverTime = 10f; // 3�� ���� Y + 1 ����

    private Rigidbody rb;
    private float timer = 0f; // �ð� ������ ���� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // 3�� ���ȸ� Y + 1 ����
        timer += Time.fixedDeltaTime;
        Vector3 adjustedTarget = (timer < hoverTime) ?
            new Vector3(target.position.x, target.position.y + 1f, target.position.z) :
            target.position;

        // Ÿ���� ���ϵ��� ���� ȸ��
        Vector3 direction = (adjustedTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime);

        // �̻��� �̵�
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
