using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class PlayerMovement : MonoBehaviour
{
    // ������Ʈ
    private Camera              _camera;
    private Animator            _animator;
    private Rigidbody           _rigidbody;
    private PlayerState         _playerState;

    // ȸ�� ���� ����
    Vector3 moveDir   = Vector3.zero;
    public float    rotationSpeed   = 50f;

    
    // �̵� ���� ����
    public float    speed           = 5f;
    public float    runSpeed        = 8f;
    private float   finalSpeed;
  
    //private bool    isMoving        = false;
    //private bool    isRunning       = false;

  
    // ���� ���� ����
    public float jumpForce = 7f;
    
    //private bool    isJumping       = false;


    // �����̵� ���� ����
    //private bool    isSliding         = false;
    public float    knockbackForce = 30f;

    // �߶� ���� ����
    // private bool        isFalling = false;
    // private bool        isAccelFalling = false;
    // private bool        isFallingImpact = false;

    private float       groundCheckDistance = 1f;  // �� ��ġ���� �Ʒ��� üũ�� �Ÿ�
    // public LayerMask    groundLayer;             // �ٴ����� ������ ���̾�
    private bool        isGrounded = true;

    private float fallStartTime = 0f;  // �������� ������ �ð�
    private float fallThreshold = 0.2f;  // 1�� �̻� �������� ���� isFalling Ȱ��ȭ
    private float accelFallThreshold = 5f;  // 3�� �̻� �������� �� isAccelFalling Ȱ��ȭ

    // �Է� ���� ����
    //private float   hAxis;
    //private float   vAxis;
    //private bool    keyJump;
    //private bool    keySlide;
    //private bool    keyAltCamera = false;



    void Start()
    {
        // ���� ������Ʈ�� �ִ� ������Ʈ ����
        _camera      = Camera.main;
        _rigidbody   = GetComponent<Rigidbody>();
        _animator    = GetComponent<Animator>();
        _playerState = GetComponent<PlayerState>();

    }

    void FixedUpdate()
    {
        Move();
        Slide();
    }

    void Update()
    {
        // HandleInput();
        Rotate();
        Jump();
        Fall();
        SetAnimation();
    }

    //void HandleInput()
    //{
    //    vAxis        = Input.GetAxis        ("Vertical");
    //    hAxis        = Input.GetAxis        ("Horizontal");
    //    isMoving     = Mathf.Abs(hAxis) > 0.1f || Mathf.Abs(vAxis) > 0.1f;
    //    isRunning    = Input.GetButton      ("Run");
    //    keyJump      = Input.GetButtonDown  ("Jump");
    //    keySlide     = Input.GetButton      ("Slide");
    //    keyAltCamera = Input.GetKey(KeyCode.LeftAlt);
    //}

    void Move()
    {
        if (_playerState.isClimbing) return;
        
        finalSpeed = _playerState.isRunning ? runSpeed : speed;

        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // ������ ����
        moveDir = (cameraForward * _playerState.vAxis + cameraRight * _playerState.hAxis).normalized;

        if((_playerState.isRunning || _playerState.isSliding) && _playerState.isMoving)
            transform.forward = moveDir;

        // ������
        transform.position += moveDir * finalSpeed * Time.deltaTime;

    }

    void Rotate()
    {
        if (_playerState.keyAltCamera) return;

        // �޸��ų� �����̵��� ȸ������ ���� 
        Vector3 targetDir = (_playerState.isRunning || _playerState.isSliding) ? transform.forward : _camera.transform.forward;
        targetDir.y = 0;

        // ȸ��
        transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z)),
                Time.deltaTime * rotationSpeed
                );
    }

    void Jump()
    {
        if (_playerState.keyJump && !_playerState.isJumping)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _animator.SetTrigger("doJump");
            _animator.SetBool("isJumping", true);
            _playerState.SetState("isJumping", true);
        }
    }

    void Slide()
    {
        if(_playerState.keySlide && !_playerState.isSliding && _playerState.isMoving)
        {
            speed *= 5;
            _animator.SetTrigger("doSlide");
            _playerState.SetState("isSliding", true);
            Invoke("SlideOut", 0.5f);
        }
    }

    void SlideOut()
    {
        speed *= 0.2f;
        _playerState.SetState("isSliding",false);
    }

    void SetAnimation()
    {
        _animator.SetFloat("vInput",         _playerState.vAxis);
        _animator.SetFloat("hInput",         _playerState.hAxis);
        _animator.SetBool("isRunning",       _playerState.isRunning && _playerState.isMoving);
        _animator.SetBool("isJumping",       _playerState.isJumping);
        _animator.SetBool("isFalling",       _playerState.isFalling);
        _animator.SetBool("isAccelFalling",  _playerState.isAccelFalling);
        _animator.SetBool("isFallingImpact", _playerState.isFallingImpact);
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Floor":
                _playerState.SetState("isJumping", false);
                _playerState.SetState("isFalling", false);

                if (_playerState.isAccelFalling)
                {
                    _playerState.SetState("isFallingImpact",true);
                }
                isGrounded = true;
                break;

            case "Interaction":
                if (_playerState.isSliding)
                {
                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
                    }
                }
                break;

            default:
                break;
        }

    }

    void Fall()
    {
        if (_playerState.isParkouring) return; // ���� ���̸� ���� üũ X


        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool wasGrounded = isGrounded;  // ���� �������� ���� ���� ����
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance);

        // �ٴڿ��� �������� ���� Ÿ�̸� ����
        if (!isGrounded && wasGrounded)
        {
            fallStartTime = Time.time;  // ���� �ð� ����
        }

        float fallDuration = Time.time - fallStartTime;  // ������ �ð� ���
        Debug.Log(fallDuration);

        // 0.2�� �̻� ���߿� ������ isFalling Ȱ��ȭ �� Falling ���·� ����
        if (!isGrounded && !_playerState.isJumping && fallDuration >= fallThreshold)
        {
            _playerState.SetState("isFalling", true);

            // 3�� �̻� ���߿� ������ isAccelFalling Ȱ��ȭ �� AccelFalling ���·� ����
            if (fallDuration >= accelFallThreshold)
            {
                _playerState.SetState("isFalling", false);
                _playerState.SetState("isAccelFalling",true);
            }
        }
        //else if (isGrounded)
        //{
        //    isFalling = false;
        //    isAccelFalling = false; // �ٴڿ� ������ �ʱ�ȭ
        //}
    }
}
