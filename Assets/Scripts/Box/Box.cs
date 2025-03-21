using UnityEngine;

public class Box : MonoBehaviour
{
    public bool isThrown = false; // 플레이어가 던졌는지 여부

    public bool isOriginal = true;


    private void Start()
    {
        if (!isOriginal)
        {
            Destroy(gameObject, 15f); // 5초 후에 오브젝트 파괴
        }
    }
    public void SetOriginal(bool b)
    {
        isOriginal = b;
    }

    public void MarkAsThrown()
    {
        isThrown = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        // 바닥 같은 데 충돌하면 다시 false로
        if (other.gameObject.CompareTag("Floor"))
        {
            isThrown = false;
        }
    }
}
