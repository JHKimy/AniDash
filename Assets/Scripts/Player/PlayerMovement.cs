using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class PlayerMovement : MonoBehaviour
{
    // 컴포넌트
    private Camera              _camera;
    private Animator            _animator;
    private Rigidbody           _rigidbody;
    private PlayerState         _playerState;

    // 회전 관련 변수
    Vector3 moveDir   = Vector3.zero;
    public float    rotationSpeed   = 50f;

    
    // 이동 관련 변수
    public float    speed           = 5f;
    public float    runSpeed        = 8f;
    private float   finalSpeed;
  
    //private bool    isMoving        = false;
    //private bool    isRunning       = false;

  
    // 점프 관련 변수
    public float jumpForce = 7f;
    
    //private bool    isJumping       = false;


    // 슬라이딩 관련 변수
    //private bool    isSliding         = false;
    public float    knockbackForce = 30f;

    // 추락 관련 변수
    // private bool        isFalling = false;
    // private bool        isAccelFalling = false;
    // private bool        isFallingImpact = false;

    private float       groundCheckDistance = 1f;  // 발 위치에서 아래로 체크할 거리
    // public LayerMask    groundLayer;             // 바닥으로 판정할 레이어
    private bool        isGrounded = true;

    private float fallStartTime = 0f;  // 떨어지기 시작한 시간
    private float fallThreshold = 0.2f;  // 1초 이상 떨어졌을 때만 isFalling 활성화
    private float accelFallThreshold = 5f;  // 3초 이상 떨어졌을 때 isAccelFalling 활성화

    // 입력 관련 변수
    //private float   hAxis;
    //private float   vAxis;
    //private bool    keyJump;
    //private bool    keySlide;
    //private bool    keyAltCamera = false;



    void Start()
    {
        // 현재 오브젝트에 있는 컴포넌트 참조
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

        // 움직임 방향
        moveDir = (cameraForward * _playerState.vAxis + cameraRight * _playerState.hAxis).normalized;

        if((_playerState.isRunning || _playerState.isSliding) && _playerState.isMoving)
            transform.forward = moveDir;

        // 움직임
        transform.position += moveDir * finalSpeed * Time.deltaTime;

    }

    void Rotate()
    {
        if (_playerState.keyAltCamera) return;

        // 달리거나 슬라이딩시 회전방향 정의 
        Vector3 targetDir = (_playerState.isRunning || _playerState.isSliding) ? transform.forward : _camera.transform.forward;
        targetDir.y = 0;

        // 회전
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
        if (_playerState.isParkouring) return; // 파쿠르 중이면 낙하 체크 X


        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool wasGrounded = isGrounded;  // 이전 프레임의 지면 상태 저장
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance);

        // 바닥에서 떨어지는 순간 타이머 시작
        if (!isGrounded && wasGrounded)
        {
            fallStartTime = Time.time;  // 현재 시간 저장
        }

        float fallDuration = Time.time - fallStartTime;  // 떨어진 시간 계산
        Debug.Log(fallDuration);

        // 0.2초 이상 공중에 있으면 isFalling 활성화 → Falling 상태로 전이
        if (!isGrounded && !_playerState.isJumping && fallDuration >= fallThreshold)
        {
            _playerState.SetState("isFalling", true);

            // 3초 이상 공중에 있으면 isAccelFalling 활성화 → AccelFalling 상태로 전이
            if (fallDuration >= accelFallThreshold)
            {
                _playerState.SetState("isFalling", false);
                _playerState.SetState("isAccelFalling",true);
            }
        }
        //else if (isGrounded)
        //{
        //    isFalling = false;
        //    isAccelFalling = false; // 바닥에 닿으면 초기화
        //}
    }
}
