using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformRotate : MonoBehaviour, PlayerState.PlayerObserver
{
    public float rotationSpeed = 90f; // 초당 회전 속도 (기본: 90도)
    public float rotationAngle = 180f; // 회전 각도 (180도)
    private bool isRotating = false; // 현재 회전 중인지 확인
    public bool playerNearby = false; // 플레이어 감지 여부
    private Coroutine rotationCoroutine;

    private float targetRotation = 90f; // 목표 회전 각도
    private float currentRotation = 0f; // 현재 회전 각도
    private int direction = 1; // 회전 방향 (1: 정방향, -1: 역방향)

    private void Start()
    {
        isRotating = false;
    }
    void Update()
    {
        if (!playerNearby) return;

        float deltaRotation = rotationSpeed * Time.deltaTime * direction;
        currentRotation += deltaRotation;

        if (Mathf.Abs(currentRotation) >= targetRotation)
        {
            direction *= -1; // 방향 전환
            currentRotation = Mathf.Sign(currentRotation) * targetRotation; // 각도 제한
        }
        transform.Rotate(Vector3.right * deltaRotation);
    }

    public void OnPlayerStateChanged(PlayerState playerState)
    {
        if (playerState != null)
        {
            float distance = Vector3.Distance(transform.position, playerState.transform.position);

            if (distance < 10f) // 5m 이내 감지
            {
                Debug.Log("djflsjflk");

                if (!playerNearby)
                {
                    playerNearby = true;
                }
            }
            else
            {
                playerNearby = false;
            }
        }
    }

    //private void StartRotation()
    //{
    //    if (!isRotating) // 회전 중이 아니면 시작
    //    {
    //        rotationCoroutine = StartCoroutine(RotatePlatform());
    //    }
    //}

    //private IEnumerator RotatePlatform()
    //{
    //    isRotating = true;
    //    float targetAngle = transform.eulerAngles.x + rotationAngle;
    //    float startAngle = transform.eulerAngles.x;

    //    while (Mathf.Abs(transform.eulerAngles.x - targetAngle) > 1f)
    //    {
    //        float step = rotationSpeed * Time.deltaTime;
    //        transform.Rotate(Vector3.right * step);
    //        yield return null;
    //    }

    //    yield return new WaitForSeconds(0.5f); // 0.5초 대기 (전환 후)

    //    // 반대 방향 회전
    //    targetAngle = transform.eulerAngles.x - rotationAngle;
    //    while (Mathf.Abs(transform.eulerAngles.x - startAngle) > 1f)
    //    {
    //        float step = rotationSpeed * Time.deltaTime;
    //        transform.Rotate(Vector3.left * step);
    //        yield return null;
    //    }

    //    isRotating = false; // 회전 종료
    //}
}
