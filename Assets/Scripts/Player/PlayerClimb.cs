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
    public float wallSnapDistance = 0.1f; // ���� ������ �Ÿ�

    // �� ���� ���� �Ķ����
    public float raycastWallDistance = 2f;
    public float wallCheckHeight = 1.5f;

    // ����Ŭ�� ���� ����
    public float doubleClickTime = 0.3f; // ��� �ð� (0.3��)
    private float lastWPressTime = -1f;

    // ���� ���� üũ�� �Ķ���� (������ ������ ��)
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
        _playerParkour = GetComponent<PlayerParkour>(); // ���� GameObject�� �����Ǿ� �־�� ��
    }

    void Update()
    {
        WallCheck();  // �� ����

        // W Ű ����Ŭ������ Ŭ���̹� ����
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
                ClimbWall();  // �� Ÿ�� ����

                // Ŭ���̹� �� ���� Ű �Է� �� ���� ���� üũ
                if (_playerState.keyJump)
                {
                    Vector3 targetPos;
                    if (CheckForParkourFromWall(out targetPos))
                    {
                        // ���� targetPos�� ���� ��ũ��Ʈ�� �����Ͽ� ����
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

    // �� ����: ���� ���̿��� Raycast�� ���� ����� ���� ������ ����
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
        // ���� ��ġ���� ���� �븻 �������� wallSnapDistance ��ŭ �̵�
        Vector3 targetPos = transform.position - wallNormal * wallSnapDistance;
        transform.position = targetPos;
    }

    // �� Ÿ�� �̵�: ���� ������ ������� ���� �� ���� �̵� ���
    void ClimbWall()
    {
        Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up).normalized;
        Vector3 wallUp = Vector3.Cross(wallTangent, wallNormal).normalized;
        Vector3 desiredMovement = (wallUp * _playerState.vAxis + wallTangent * _playerState.hAxis) * climbSpeed;
        _rigidbody.linearVelocity = desiredMovement;
    }

    // ������ �پ����
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

    // ������ ���� ���� ���θ� �Ǵ��ϰ� ��ǥ ��ġ ���
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
