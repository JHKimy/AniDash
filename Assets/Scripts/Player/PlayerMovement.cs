using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // ������Ʈ
    private Camera _camera;
    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerState _playerState;

    // ȸ�� ���� ����
    Vector3 moveDir = Vector3.zero;
    public float rotationSpeed = 50f;

    // �̵� ���� ����
    public float speed = 5f;
    public float runSpeed = 8f;
    private float finalSpeed;

    // ����
    public float jumpForce = 7f;

    // �����̵�
    private float slideSpeed = 15f;
    public float knockbackForce = 30f;
    private float slideCooldown = 2f;
    private float lastSlideTime = -1f; // ������ �����̵� ���� �ð� (�ʱⰪ�� ������ ������ ù ���� ���)

    // �߶� ���� ����
    private float groundCheckDistance = 5f;  // �� ��ġ���� �Ʒ��� üũ�� �Ÿ�
    private bool isGrounded = true;

    private float fallStartTime = 0f;  // �������� ������ �ð�
    private float fallThreshold = 0.1f;  // 1�� �̻� �������� ���� isFalling Ȱ��ȭ
    private float accelFallThreshold = 3.5f;  // 3�� �̻� �������� �� isAccelFalling Ȱ��ȭ��



    void Start()
    {
        // ���� ������Ʈ�� �ִ� ������Ʈ ����
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerState = GetComponent<PlayerState>();

    }

    void FixedUpdate()
    {
        Move();
        Slide();
    }

    void Update()
    {
        // Debug.Log(_playerState.currentState);


        Rotate();
        Jump();
        Fall();
        //SetAnimation();
    }

    void Move()
    {
        if (!_playerState.CanMove()) return;

        // �ٴ� ���� �ƴϸ� Walking ���·�
        if (_playerState.isGrounded)
        {
            if (_playerState.currentState != PlayerState.State.Running)
            {
                _playerState.SetState(PlayerState.State.Walking);
            }
        }

        // ��ü ��� ������ �ӵ� ���߱�
        float holdSpeed = _playerState.IsHoldingObject() ? 0.6f : 1f;
        finalSpeed = (_playerState.currentState == PlayerState.State.Running ? runSpeed : speed) * holdSpeed;


        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // ������ ����
        moveDir = (cameraForward * _playerState.vAxis + cameraRight * _playerState.hAxis).normalized;

        // �޸��ų� �����̵��Ҷ� �� �������� ĳ���� ȸ��
        if (_playerState.currentState == PlayerState.State.Running
            || _playerState.currentState == PlayerState.State.Sliding)
        {
            transform.forward = moveDir;
        }

        // ������
        transform.position += moveDir * finalSpeed * Time.deltaTime;
        // _rigidbody.MovePosition(transform.position + moveDir * finalSpeed * Time.fixedDeltaTime);

    }

    void Rotate()
    {
        if (_playerState.keyAltCamera) return;
        // if (_playerState.currentState == PlayerState.State.Running) return;
        // �޸��ų� �����̵��� ȸ������ ���� 
        // Vector3 targetDir = (_playerState.isRunning || _playerState.isSliding) ? transform.forward : _camera.transform.forward;
        // targetDir.y = 0;
        //if (_playerState.currentState == PlayerState.State.Running) return;
        //Debug.Log("dfsdf");

        Vector3 forward =
        (_playerState.currentState == PlayerState.State.Running
            || _playerState.currentState == PlayerState.State.Sliding)
            ? transform.forward : _camera.transform.forward;
        forward.y = 0;

        // ȸ��
        transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(forward),
                Time.deltaTime * rotationSpeed
                );
    }

    void Jump()
    {
        if (!_playerState.CanJump() || !_playerState.isGrounded) return;

        if (_playerState.keyJump)
        {

            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _animator.SetTrigger("doJump");

            _playerState.SetState(PlayerState.State.Jumping);
            _playerState.isGrounded = false;
            // _animator.SetBool("isJumping", true);

        }
    }

    void Slide()
    {
        if (!_playerState.CanSlide()) return;

        if (_playerState.keySlide)
        // if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.time < lastSlideTime + slideCooldown)
            {
                return;
            }
            _playerState.keySlide = false; // �ߺ� ���� ����
            _playerState.SetState(PlayerState.State.Sliding);


            lastSlideTime = Time.time;


            _animator.SetTrigger("doSlide");
            transform.forward = moveDir;
            Vector3 slideDirection = transform.forward;
            _rigidbody.linearVelocity = new Vector3(
                slideDirection.x * slideSpeed,
                _rigidbody.linearVelocity.y,
                slideDirection.z * slideSpeed);
            // speed = slideSpeed;
            //  ������ ���ϰ� �б� (���� ��ġ��)
            // 0.2�� �� �����̵� �ӵ� ����
            //Invoke("IncreaseSlideSpeed", 0.2f);

            Invoke("SlideOut", 0.25f);
            // _playerState.SetState("isSliding", true);
        }
    }
    // �ӵ� ���� �Լ� (0.2�� �� ����)
    //void IncreaseSlideSpeed()
    //{
    //    _animator.SetTrigger("doSlide");
    //    transform.forward = moveDir;
    //    Vector3 slideDirection = transform.forward;
    //    _rigidbody.linearVelocity = new Vector3(
    //        slideDirection.x * slideSpeed,
    //        _rigidbody.linearVelocity.y,
    //        slideDirection.z * slideSpeed
    //    );
    //}
    void SlideOut()
    {
        // speed = (_playerState.currentState == PlayerState.State.Running) ? runSpeed : speed;
        // _playerState.SetState("isSliding",false);
        //_playerState.SetState(PlayerState.State.Idle);

        // �����̵尡 ������ �ӵ� ���̱�
        _rigidbody.linearVelocity = Vector3.zero;

        // �����̵� �� �ٽ� �ȱ�(Walking) ���·� ����
        // _playerState.speed = 0;
        // _playerState.SetState(PlayerState.State.Idle);
    }

    //void SetAnimation()
    //{
    //    _animator.SetFloat("vInput", _playerState.vAxis);
    //    _animator.SetFloat("hInput", _playerState.hAxis);
    //    //_animator.SetBool("isHoldingObject", _playerState.IsHoldingObject());
    //    _animator.SetBool("isJumping", _playerState.currentState == PlayerState.State.Jumping);
    //    _animator.SetBool("isSliding", _playerState.currentState == PlayerState.State.Sliding);

    //    _animator.SetBool("isRunning", _playerState.currentState == PlayerState.State.Running);

    //    //_animator.SetFloat("vInput",         _playerState.vAxis);
    //    //_animator.SetFloat("hInput",         _playerState.hAxis);
    //    //_animator.SetBool("isRunning",       _playerState.isRunning && _playerState.isMoving);
    //    //_animator.SetBool("isJumping",       _playerState.isJumping);
    //    //_animator.SetBool("isFalling",       _playerState.isFalling);
    //    //_animator.SetBool("isAccelFalling",  _playerState.isAccelFalling);
    //    //_animator.SetBool("isFallingImpact", _playerState.isFallingImpact);
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    switch (collision.gameObject.tag)
    //    {
    //        case "Floor":
    //            _playerState.SetState("isJumping", false);
    //            _playerState.SetState("isFalling", false);

    //            if (_playerState.isAccelFalling)
    //            {
    //                _playerState.SetState("isFallingImpact", true);
    //            }
    //            isGrounded = true;
    //            break;

    //        case "Interaction":
    //            if (_playerState.isSliding)
    //            {
    //                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
    //                if (rb != null)
    //                {
    //                    rb.AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
    //                }
    //            }
    //            break;

    //        default:
    //            break;
    //    }

    //}

    void Fall()
    {
        if (!_playerState.CanFall()) return;
        // if (_playerState.isParkouring) return; // ���� ���̸� ���� üũ X


        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool wasGrounded = isGrounded;  // ���� �������� ���� ���� ����
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance);

        // Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, Color.red, 0.1f);

        // Debug.Log("Ground : " + isGrounded);



        // �ٴڿ��� �������� ���� Ÿ�̸� ����
        if (!isGrounded && wasGrounded)
        {
            fallStartTime = Time.time;  // ���� �ð� ����
            _playerState.isGrounded = false;
        }

        float fallDuration = Time.time - fallStartTime;  // ������ �ð� ���
        // Debug.Log(fallDuration);

        // 0.2�� �̻� ���߿� ������ isFalling Ȱ��ȭ �� Falling ���·� ����
        if (!isGrounded && _playerState.currentState != PlayerState.State.Jumping && fallDuration >= fallThreshold)
        {
            _playerState.SetState(PlayerState.State.Falling);

            // 3�� �̻� ���߿� ������ isAccelFalling Ȱ��ȭ �� AccelFalling ���·� ����
            if (fallDuration >= accelFallThreshold)
            {

                _playerState.SetState(PlayerState.State.AccelFalling);

            }
        }
        //if (_playerState.isGrounded)
        //{
        //    _playerState.SetState(PlayerState.State.FallingImpact);

        //}
        if (isGrounded && _playerState.currentState == PlayerState.State.AccelFalling)
        {
            _playerState.SetState(PlayerState.State.FallingImpact);
        }
    }
}