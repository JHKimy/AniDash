using UnityEngine;

public class Box : MonoBehaviour
{
    public bool isThrown = false; // �÷��̾ �������� ����

    public bool isOriginal = true;


    private void Start()
    {
        if (!isOriginal)
        {
            Destroy(gameObject, 15f); // 5�� �Ŀ� ������Ʈ �ı�
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
        // �ٴ� ���� �� �浹�ϸ� �ٽ� false��
        if (other.gameObject.CompareTag("Floor"))
        {
            isThrown = false;
        }
    }
}
