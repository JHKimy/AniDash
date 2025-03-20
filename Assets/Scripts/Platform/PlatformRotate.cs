using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformRotate : MonoBehaviour, PlayerState.PlayerObserver
{
    public float rotationSpeed = 90f; // �ʴ� ȸ�� �ӵ� (�⺻: 90��)
    public float rotationAngle = 180f; // ȸ�� ���� (180��)
    private bool isRotating = false; // ���� ȸ�� ������ Ȯ��
    public bool playerNearby = false; // �÷��̾� ���� ����
    private Coroutine rotationCoroutine;

    private float targetRotation = 90f; // ��ǥ ȸ�� ����
    private float currentRotation = 0f; // ���� ȸ�� ����
    private int direction = 1; // ȸ�� ���� (1: ������, -1: ������)

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
            direction *= -1; // ���� ��ȯ
            currentRotation = Mathf.Sign(currentRotation) * targetRotation; // ���� ����
        }
        transform.Rotate(Vector3.right * deltaRotation);
    }

    public void OnPlayerStateChanged(PlayerState playerState)
    {
        if (playerState != null)
        {
            float distance = Vector3.Distance(transform.position, playerState.transform.position);

            if (distance < 10f) // 5m �̳� ����
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
    //    if (!isRotating) // ȸ�� ���� �ƴϸ� ����
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

    //    yield return new WaitForSeconds(0.5f); // 0.5�� ��� (��ȯ ��)

    //    // �ݴ� ���� ȸ��
    //    targetAngle = transform.eulerAngles.x - rotationAngle;
    //    while (Mathf.Abs(transform.eulerAngles.x - startAngle) > 1f)
    //    {
    //        float step = rotationSpeed * Time.deltaTime;
    //        transform.Rotate(Vector3.left * step);
    //        yield return null;
    //    }

    //    isRotating = false; // ȸ�� ����
    //}
}
