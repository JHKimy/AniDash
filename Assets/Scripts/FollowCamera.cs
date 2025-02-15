using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;


public class FollowCamera : MonoBehaviour
{
    private Player _player;

    public Transform character; // 타겟(캐릭터)
    public float followSpeed = 50f;
    public float sensitivity = 500f;
    public float clampAngle = 80f;

    private float rotX;
    private float rotY;

    public Vector3 offset = new Vector3(0, 2, -7); // 캐릭터 뒤쪽 위에 배치할 오프셋
    public float minDistance = 1f;
    public float maxDistance = 5f;
    public float smoothness = 500f;

    private float finalDistance;

    void Start()
    {

        _player = character.GetComponent<Player>();

        // offset으로 카메라 최대 거리 설정
        maxDistance = offset.magnitude;
        finalDistance = maxDistance;

        // 현재 카메라의 X, Y 회전 각도 가져오기
        Vector3 initialRotation = transform.rotation.eulerAngles;
        rotX = initialRotation.x > 180f ? initialRotation.x - 360f : initialRotation.x;
        rotY = initialRotation.y;

        // 마우스 커서 잠금
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }


    void Update()
    {
        if (!_player.isRunning) // 걷거나 정지할 때만 카메라 조작 가능
        {
            rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
            rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(rotX, rotY, 0),
                Time.deltaTime * followSpeed);
        }
        else 
        {
            transform.position = character.position + offset;
            transform.LookAt(character.position);
        }
    }

    void LateUpdate()
    {
        if (character == null) return;

        if (!_player.isRunning)
        {





            // 카메라 목표 위치 계산 (캐릭터의 위치 + offset)
            //Vector3 targetPosition = character.position + transform.rotation * offset;
            Vector3 targetPosition = character.position + offset;

            RaycastHit hit;
            Vector3 dirToTarget = targetPosition - character.position;

            if (Physics.Raycast(character.position + Vector3.up * 0.5f, dirToTarget.normalized, out hit, maxDistance))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }

            // 캐릭터 위치 + (원래 위치 * 오프셋 방향 * 최종 거리)
            Vector3 finalPos = character.position + (transform.rotation * offset.normalized * finalDistance);

            // 카메라 위치 변경
            transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * smoothness);

        }
    }
}
