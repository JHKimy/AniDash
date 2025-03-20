using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Key : MonoBehaviour
{
    public float floatSpeed = 1.5f;  // 둥둥 뜨는 속도
    public float floatHeight = 0.5f; // 최대 떠오르는 높이
    public float rotationSpeed = 50f; // 회전 속도

    private Vector3 startPos; // 초기 위치 저장

    void Start()
    {
        startPos = transform.position; // 시작 위치 저장
    }

    void Update()
    {
        // 상하로 둥둥 떠다니기 (사인 함수 활용)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // 빙글빙글 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player") // 수정된 부분
        {
            PlayerState playerState = collision.gameObject.GetComponent<PlayerState>();

            Destroy(gameObject);
            playerState.SetKey(playerState.key + 1);  // 키 개수 증가

        }
    }
}
