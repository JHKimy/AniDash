using UnityEngine;

public class CollisionEachPlatform : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어를 해당 발판의 자식으로 설정
            collision.transform.parent = transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 부모 설정 해제
            collision.transform.parent = null;
        }
    }
}
