using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private PlayerState _playerState;
    private Animator _animator;

    private bool isWallDetected = false;
    private Vector3 wallNormal;

    public float climbSpeed = 3f;
    public float jumpOffForce = 5f;

    // ����Ŭ�� ���� ����
    private float lastWPressTime = -1f;
    private float doubleClickTime = 0.3f; // ����Ŭ�� ��� �ð� (0.3��)

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerState = GetComponent<PlayerState>();  // �Է� �ý��� ��������
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        WallCheck();  // �� ����

        // W Ű ����Ŭ�� ����
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime <= doubleClickTime)
            {
                if (isWallDetected && _playerState.currentState != PlayerState.State.Climbing)
                {
                    StartWallClimb();
                }
            }
            lastWPressTime = Time.time; // ������ �Է� �ð� ������Ʈ
        }

        if (_playerState.currentState == PlayerState.State.Climbing)
        {
            ClimbWall();  // �� Ÿ�� ����

            if (_playerState.keyJump)  // ���� Ű �Է� Ȯ��
            {
                JumpOffWall();
            }
        }
    }

    void WallCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, 1f))  // �� ���� ��
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
        _rigidbody.linearVelocity = Vector3.zero;  // ���� �ӵ� �ʱ�ȭ
    }

    void ClimbWall()
    {
        Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up).normalized;
        Vector3 wallUp = Vector3.Cross(wallTangent, wallNormal).normalized;

        // �Է°� ������� �� Ÿ�� �̵� ����
        Vector3 desiredMovement = (wallUp * _playerState.vAxis + wallTangent * _playerState.hAxis) * climbSpeed;
        _rigidbody.linearVelocity = desiredMovement;
    }

    void JumpOffWall()
    {
        _playerState.SetState(PlayerState.State.Falling);
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = wallNormal * jumpOffForce + Vector3.up * jumpOffForce;  // ������ ����
    }

    void EndWallClimb()
    {
        _playerState.SetState(PlayerState.State.Falling);
        _rigidbody.useGravity = true;
    }
}
