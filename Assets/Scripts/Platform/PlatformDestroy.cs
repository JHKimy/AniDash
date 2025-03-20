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
        Debug.Log("[PlatformDestroy] 총 " + platforms.Count + "개의 플랫폼이 감지됨.");
    }

    private void OnCollisionEnter(Collision other)
    {
        // 충돌한 오브젝트가 "Player" 태그인지 확인
        if (other.gameObject.CompareTag("Player"))
        {
            // 플레이어가 충돌한 오브젝트 가져오기
            Collider playerCollider = other.gameObject.GetComponent<Collider>();

            // 자식 오브젝트들을 가져와서 검사
            foreach (Transform child in transform)
            {
                // 자식 오브젝트가 콜라이더를 가지고 있고, 충돌한 콜라이더인지 확인
                Collider childCollider = child.GetComponent<Collider>();
                if (childCollider != null && childCollider.bounds.Intersects(playerCollider.bounds))
                {
                    StartCoroutine(DestroyPlatform(child.gameObject));
                    break; // 하나만 삭제 후 루프 종료
                }
            }
        }
    }

    private IEnumerator DestroyPlatform(GameObject platform)
    {
        yield return new WaitForSeconds(0.5f); // 0.2초 대기

        // 리스트에서 제거 후 삭제
        platforms.Remove(platform.transform);
        Destroy(platform);
    }
}
