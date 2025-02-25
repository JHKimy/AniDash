using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    private Rigidbody   _rigidbody;
    private PlayerState _playerState;
    private Animator    _animator;


    private bool isWallDetected = false;
    private Vector3 wallNormal;

    public float climbSpeed = 3f;
    public float jumpOffForce = 5f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerState = GetComponent<PlayerState>();  // �Է� �ý��� ��������
        _animator = GetComponent<Animator>();

    }

    void Update()
    {
        WallCheck();  // �� ����

        if (isWallDetected)
        {
            if (!_playerState.isClimbing)
            {
                StartWallClimb();
            }
            ClimbWall();  // �� Ÿ�� ����
        }

        if (_playerState.isClimbing && _playerState.keyJump)  // ���� Ű �Է� Ȯ��
        {
            JumpOffWall();
        }

        _animator.SetBool("isClimbing", _playerState.isClimbing);
        _animator.SetFloat("climbInput", _playerState.vAxis);
    }

    void WallCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, 1f))  // �� ���� ��
        {
            if (hit.collider.CompareTag("Wall"))
            {
                isWallDetected = true;
                wallNormal = hit.normal;
            }
        }
        else
        {
            isWallDetected = false;
            if (_playerState.isClimbing)
            {
                EndWallClimb();
            }
        }
    }

    void StartWallClimb()
    {
        _playerState.SetState("isClimbing", true);
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
        _playerState.SetState("isClimbing", false);
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = wallNormal * jumpOffForce + Vector3.up * jumpOffForce;  // ������ ����
    }

    void EndWallClimb()
    {
        _playerState.SetState("isClimbing", false);
        _rigidbody.useGravity = true;
    }
}
