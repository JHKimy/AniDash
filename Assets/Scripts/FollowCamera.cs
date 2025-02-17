using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;


public class FollowCamera : MonoBehaviour
{
    public Transform    characterTransform;    // 캐릭터 움직임
    private Player      character;             // 캐릭터 객체

    // 카메라 위치 변수
    public Vector3      offset              = new Vector3(0, 2, -7);
    private float       minDistance         = 1f;
    private float       maxDistance         = 5f;
    private float       finalDistance;
    private Vector3     finalPos;

    // 카메라 회전 변수
    private float       yaw                 = 0f;
    private float       pitch               = 0f;
    private float       clampAngle          = 80f;

    // 달리기 관련 변수
    Quaternion      runRot;

    // 입력 변수
    public float        mouseSensitivity    = 500f;



    private void Start()
    {   
        // 캐릭터 참조
        character = characterTransform.GetComponent<Player>();

        // 캐릭터와 거리 초기 설정
        maxDistance = offset.magnitude;
        finalDistance = maxDistance;

        // 마우스 커서 잠금
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        Rotation();
    }

    void LateUpdate()
    {
        Move();
    }

    void Rotation()
    {
        // 마우스 입력 가져오기
        yaw     -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // 위아래 
        pitch   += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // 좌우

        yaw     = Mathf.Clamp(yaw, -clampAngle, clampAngle);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(yaw, pitch, 0),
            Time.deltaTime * 50f);
    }

    void Move()
    {
        // 레이캐스팅
        RaycastHit hit;
        Vector3 dirToTarget = (characterTransform.position + transform.rotation * offset) - (characterTransform.position + Vector3.up * 0.5f);

        if (Physics.Raycast(characterTransform.position + Vector3.up * 0.5f, dirToTarget.normalized, out hit, maxDistance))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }


        if (character.GetIsRunning())
        {
            finalPos = characterTransform.position + (runRot * offset.normalized * finalDistance);
        }
        else
        {
            // 캐릭터 위치 + (원래 회전값 * 오프셋 방향 * 최종 거리)
            // 회전 행렬 * 스칼라 -> 회전한 값
            finalPos = characterTransform.position + (transform.rotation * offset.normalized * finalDistance);
            runRot = transform.rotation;

        }
        // 카메라 위치 변경
        transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * 500f);
    }

}
