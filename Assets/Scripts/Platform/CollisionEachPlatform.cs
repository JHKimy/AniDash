using UnityEngine;

public class CollisionEachPlatform : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾ �ش� ������ �ڽ����� ����
            collision.transform.parent = transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾��� �θ� ���� ����
            collision.transform.parent = null;
        }
    }
}
