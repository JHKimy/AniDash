using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Key : MonoBehaviour
{
    public float floatSpeed = 1.5f;  // �յ� �ߴ� �ӵ�
    public float floatHeight = 0.5f; // �ִ� �������� ����
    public float rotationSpeed = 50f; // ȸ�� �ӵ�

    private Vector3 startPos; // �ʱ� ��ġ ����

    void Start()
    {
        startPos = transform.position; // ���� ��ġ ����
    }

    void Update()
    {
        // ���Ϸ� �յ� ���ٴϱ� (���� �Լ� Ȱ��)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // ���ۺ��� ȸ��
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player") // ������ �κ�
        {
            PlayerState playerState = collision.gameObject.GetComponent<PlayerState>();

            Destroy(gameObject);
            playerState.SetKey(playerState.key + 1);  // Ű ���� ����

        }
    }
}
