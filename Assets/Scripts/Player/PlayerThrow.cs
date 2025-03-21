using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    private Camera _camera;
    private Rigidbody _rigidbody;
    private PlayerState _playerState;

    private GameObject grabbedObject = null;
    public Transform grabPostion; // 박스를 들 위치
    private float grabRange = 2f;
    private float throwForce = 10f;

    public LineRenderer trajectoryLine; // 유도선을 표시할 LineRenderer
    private int trajectorySegmentCount = 30; // 궤적 점 개수
    private float trajectoryTimeStep = 0.05f; // 각 점 간 시간 간격 (0.05초)

    void Start()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        DetectPickupObject();

        if (grabbedObject != null)
        {
            DrawTrajectory();
        }
    }

    void DetectPickupObject()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * grabRange, Color.red, 0.1f);

        if (grabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, grabRange))
            {
                if (hit.collider.CompareTag("Box"))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        GrabObject(hit.collider.gameObject);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E)) // 다시 E키를 누르면 던짐
            {
                ThrowObject();
            }
        }
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true; // 물리 영향을 받지 않도록 설정
        }

        _playerState.SetSecondaryState(PlayerState.SecondaryState.HoldingObject);

        // 캐릭터의 손 위치로 이동
        grabbedObject.transform.position = grabPostion.position;
        grabbedObject.transform.rotation = grabPostion.rotation;
        grabbedObject.transform.SetParent(grabPostion);
        trajectoryLine.enabled = true; // 궤적 활성화 (잡았을 때)


    }

    void ThrowObject()
    {
        if (grabbedObject != null)
        {
            Box thrownBox = grabbedObject.GetComponent<Box>();
            thrownBox.MarkAsThrown(); // isThrown = true로 설정



            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();

            grabbedObject.transform.SetParent(null); // 부모 해제
            rb.isKinematic = false; // 물리 적용

            // 던지는 방향 조정 (카메라 기준으로 위쪽 보정)
            Vector3 throwDirection = (_camera.transform.forward + _camera.transform.up * 0.3f).normalized;

            // 질량을 고려한 속도 계산 (던지는 힘을 궤적과 동일하게 맞춤)
            Vector3 throwVelocity = throwDirection * Mathf.Sqrt(2 * throwForce * Mathf.Abs(Physics.gravity.y) / rb.mass);

            rb.linearVelocity = throwVelocity; // 속도 직접 설정

            grabbedObject = null; // 집은 상태 해제
            _playerState.SetSecondaryState(PlayerState.SecondaryState.None);
        }
    }

    void DrawTrajectory()
    {
        if (grabbedObject == null)
        {
            trajectoryLine.enabled = false;
            return;
        }

        trajectoryLine.enabled = true;

        // 시작 위치 : 박스를 잡고 있는 위치 (손 위치)
        Vector3 startPos = grabPostion.position;

        // 던지는 방향 계산 (카메라 기준으로 위쪽 보정)
        Vector3 throwDirection = (_camera.transform.forward + _camera.transform.up * 0.3f).normalized;

        // Rigidbody의 질량을 고려한 초기 속도
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        float mass = rb != null ? rb.mass : 1f;

        // 던지는 속도를 ThrowObject와 동일하게 설정
        Vector3 initialVelocity = throwDirection * Mathf.Sqrt(2 * throwForce * Mathf.Abs(Physics.gravity.y) / mass);

        // 포물선 궤적 계산
        Vector3[] trajectoryPoints = new Vector3[trajectorySegmentCount + 1];

        for (int i = 0; i <= trajectorySegmentCount; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector3 point = startPos + initialVelocity * t + 0.5f * Physics.gravity * t * t;
            trajectoryPoints[i] = point;
        }

        trajectoryLine.positionCount = trajectorySegmentCount;
        trajectoryLine.SetPositions(trajectoryPoints);
    }
}
