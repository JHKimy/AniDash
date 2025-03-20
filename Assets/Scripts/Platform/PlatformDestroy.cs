using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformDestroy : MonoBehaviour
{
    private List<Transform> platforms = new List<Transform>();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            platforms.Add(child);
        }
        Debug.Log("[PlatformDestroy] �� " + platforms.Count + "���� �÷����� ������.");
    }

    private void OnCollisionEnter(Collision other)
    {
        // �浹�� ������Ʈ�� "Player" �±����� Ȯ��
        if (other.gameObject.CompareTag("Player"))
        {
            // �÷��̾ �浹�� ������Ʈ ��������
            Collider playerCollider = other.gameObject.GetComponent<Collider>();

            // �ڽ� ������Ʈ���� �����ͼ� �˻�
            foreach (Transform child in transform)
            {
                // �ڽ� ������Ʈ�� �ݶ��̴��� ������ �ְ�, �浹�� �ݶ��̴����� Ȯ��
                Collider childCollider = child.GetComponent<Collider>();
                if (childCollider != null && childCollider.bounds.Intersects(playerCollider.bounds))
                {
                    StartCoroutine(DestroyPlatform(child.gameObject));
                    break; // �ϳ��� ���� �� ���� ����
                }
            }
        }
    }

    private IEnumerator DestroyPlatform(GameObject platform)
    {
        yield return new WaitForSeconds(0.5f); // 0.2�� ���

        // ����Ʈ���� ���� �� ����
        platforms.Remove(platform.transform);
        Destroy(platform);
    }
}
