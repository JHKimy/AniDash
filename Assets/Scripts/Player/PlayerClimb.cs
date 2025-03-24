using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private PlayerState _playerState;
    private Animator _animator;
    private PlayerParkour _playerParkour;

    private bool isWallDetected = false;
    private Vector3 wallNormal;

    public float climbSpeed = 3f;
    public float jumpOffForce = 5f;
    public float wallSnapDistance = 0.1f; // 벽에 밀착할 거리

    // 벽 감지 관련 파라미터
    public float raycastWallDistance = 2f;
    public float wallCheckHeight = 1.5f;

    // 더블클릭 관련 변수
    public float doubleClickTime = 0.3f; // 허용 시간 (0.3초)
    private float lastWPressTime = -1f;

    // 파쿠르 조건 체크용 파라미터 (벽에서 파쿠르할 때)
    public float parkourRaycastHeight = 1f;
    public float parkourRaycastDistance = 1f;
    public float minParkourHeight = 0.5f;
    public float maxParkourHeight = 3f;
    public float parkourOffset = 0.03f;
    public float parkourVerticalOffset = 0.2f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerState = GetComponent<PlayerState>();
        _animator = GetComponent<Animator>();
        _playerParkour = GetComponent<PlayerParkour>(); // 같은 GameObject에 부착되어 있어야 함
    }

    void Update()
    {
        WallCheck();  // 벽 감지

        // W 키 더블클릭으로 클라이밍 시작
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime <= doubleClickTime)
            {
                if (isWallDetected && _playerState.currentState != PlayerState.State.Climbing)
                {
                    StartWallClimb();
                }
            }
            lastWPressTime = Time.time;
        }

        if (_playerState.currentState == PlayerState.State.Climbing)
        {
            if (!isWallDetected)
            {
                EndWallClimb();
            }
            else
            {
                ClimbWall();  // 벽 타기 실행

                // 클라이밍 중 점프 키 입력 시 파쿠르 조건 체크
                if (_playerState.keyJump)
                {
                    Vector3 targetPos;
                    if (CheckForParkourFromWall(out targetPos))
                    {
                        // 계산된 targetPos를 파쿠르 스크립트에 전달하여 실행
                        _playerParkour.ExecuteParkour(targetPos);
                    }
                    else
                    {
                        JumpOffWall();
                    }
                }
            }
        }
    }

    // 벽 감지: 일정 높이에서 Raycast로 벽의 존재와 법선 정보를 얻음
    void WallCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * wallCheckHeight;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, raycastWallDistance))
        {
            isWallDetected = true;
            wallNormal = hit.normal;
        }
        else
        {
            isWallDetected = false;
        }
    }

    void StartWallClimb()
    {
        _playerState.SetState(PlayerState.State.Climbing);
        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = Vector3.zero;
        // 현재 위치에서 벽의 노말 방향으로 wallSnapDistance 만큼 이동
        Vector3 targetPos = transform.position - wallNormal * wallSnapDistance;
        transform.position = targetPos;
    }

    // 벽 타기 이동: 벽의 법선을 기반으로 상향 및 측면 이동 계산
    void ClimbWall()
    {
        Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up).normalized;
        Vector3 wallUp = Vector3.Cross(wallTangent, wallNormal).normalized;
        Vector3 desiredMovement = (wallUp * _playerState.vAxis + wallTangent * _playerState.hAxis) * climbSpeed;
        _rigidbody.linearVelocity = desiredMovement;
    }

    // 벽에서 뛰어내리기
    void JumpOffWall()
    {
        _playerState.SetState(PlayerState.State.Falling);
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = wallNormal * jumpOffForce + Vector3.up * jumpOffForce;
    }

    void EndWallClimb()
    {
        _playerState.SetState(PlayerState.State.Idle);
        _rigidbody.useGravity = true;
    }

    // 벽에서 파쿠르 가능 여부를 판단하고 목표 위치 계산
    bool CheckForParkourFromWall(out Vector3 targetPos)
    {
        targetPos = Vector3.zero;
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * parkourRaycastHeight;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, parkourRaycastDistance))
        {
            //if (hit.collider.CompareTag("Floor"))
            //{
                float objectTopY = hit.collider.bounds.max.y;
                float heightDiff = objectTopY - transform.position.y;
                if (heightDiff > minParkourHeight && heightDiff < maxParkourHeight)
                {
                    targetPos = new Vector3(hit.point.x + transform.forward.x * parkourOffset,
                                            objectTopY + parkourVerticalOffset,
                                            hit.point.z + transform.forward.z * parkourOffset);
                    return true;
                }
            //}
        }
        return false;
    }
}
