using System.Security.Cryptography.X509Certificates;
using UnityEngine;



public class PlayerState : MonoBehaviour
{
    public Transform target;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private Vector3 previousPosition;
    public float speed;

    // private float groundCheckDistance = 1.1f;  // 바닥 감지 거리
    public bool isGrounded = true;  // 현재 바닥에 있는지 여부




    public float health = 30;
    public float maxHealth = 100;
    public float stamina = 30;
    public float maxStamina = 100;

    public float key = 99;





    // ============================================
    // 키
    // ============================================
    public float vAxis /* W S */           { get; private set; }
    public float hAxis /* A D */           { get; private set; }
    public bool keyJump /* Space Bar */    { get; private set; }
    public bool keySlide /* C */            { get; set; }
    public bool keyAltCamera /* Alt */      { get; private set; }
    public bool keyGrab /* Mouse Left */    { get; private set; }

    // ============================================
    // 상태
    // ============================================

    public enum State   // 메인 상태
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Sliding,
        Falling,
        AccelFalling,
        FallingImpact,
        Parkouring,
        Climbing
    }
    public enum SecondaryState // 보조 상태
    {
        None,
        HoldingObject
    }

    public State currentState { get; private set; } = State.Idle;
    public SecondaryState secondaryState { get; private set; } = SecondaryState.None;

    public void SetState(State newState)
    {
        // if (currentState == State.Parkouring && newState == State.Falling) return;
        // if (currentState == State.Climbing && newState == State.Jumping) return;

        currentState = newState;
    }
    public void SetSecondaryState(SecondaryState newState)
    {
        secondaryState = newState;
    }



    void Start()
    {
        _rigidbody = target.GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        previousPosition = transform.position;

    }

    void FixedUpdate()
    {
        // 속도 계산
        speed = (transform.position - previousPosition).magnitude / Time.fixedDeltaTime;
        previousPosition = transform.position;

        // 기본 State를 Idle로 하기 떄문에 까다롭게 설정
        // 대부분 State에서 돌아오는 것을 Idle로 설정
        if (speed <= 0.5f && vAxis == 0 && hAxis == 0 &&
           currentState != State.Jumping && isGrounded
           && currentState != State.Climbing
           && currentState != State.AccelFalling
           && currentState != State.FallingImpact)
        {
            speed = 0;
            SetState(State.Idle);
            _rigidbody.linearVelocity = Vector3.zero;
        }

        // Debug.Log($"[FixedUpdate] 현재 속도: {speed:F2} m/s");
        Debug.Log(currentState);
        Debug.Log(secondaryState);
    }

    void Update()
    {
        SetInput();
        SetAnimation();
    }


    public bool CanMove() =>
        currentState != State.Parkouring
        && currentState != State.Climbing
        && currentState != State.Falling
        && currentState != State.AccelFalling;

    public bool CanJump()
        => currentState != State.Jumping &&
        currentState != State.Parkouring &&
        currentState != State.Climbing &&
        currentState != State.Falling;

    public bool CanSlide()
        => currentState != State.Parkouring
        && currentState != State.Climbing
        && currentState != State.Idle
        && (vAxis != 0 || hAxis != 0);

    public bool CanFall()
    => currentState != State.Parkouring
    && currentState != State.Climbing
        && currentState != State.Sliding;

    public bool CanGrab()
    => currentState != State.Parkouring
    && currentState != State.Climbing
    && currentState != State.Sliding;

    public bool IsHoldingObject()
        => secondaryState == SecondaryState.HoldingObject;


    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void SetInput()
    {
        // 키입력 받기
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        keyJump = Input.GetButtonDown("Jump");
        keySlide = Input.GetButton("Slide");
        keyAltCamera = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetButton("Run") && isGrounded && currentState == State.Walking && secondaryState != SecondaryState.HoldingObject)
        {
            SetState(State.Running);
        }
        else if (!Input.GetButton("Run") && currentState == State.Running)
        {
            SetState(State.Walking);
        }
    }

    void SetAnimation()
    {
        _animator.SetFloat("vInput", vAxis);
        _animator.SetFloat("hInput", hAxis);
        _animator.SetBool("isJumping", currentState == PlayerState.State.Jumping);
        _animator.SetBool("isSliding", currentState == PlayerState.State.Sliding);
        _animator.SetBool("isRunning", currentState == PlayerState.State.Running);
        _animator.SetBool("isClimbing", currentState == PlayerState.State.Climbing);
        if (currentState == State.Climbing)
        {
            _animator.SetFloat("climbInput", vAxis);
        }
        _animator.SetBool("isFalling", currentState == State.Falling);
        _animator.SetBool("isAccelFalling", currentState == State.AccelFalling);

        //_animator.SetBool("isHoldingObject", _playerState.IsHoldingObject());
        //_animator.SetFloat("vInput",         _playerState.vAxis);
        //_animator.SetFloat("hInput",         _playerState.hAxis);
        //_animator.SetBool("isRunning",       _playerState.isRunning && _playerState.isMoving);
        //_animator.SetBool("isJumping",       _playerState.isJumping);
        _animator.SetBool("isFallingImpact", currentState == State.FallingImpact);

        if (secondaryState == SecondaryState.HoldingObject)
        {
            _animator.SetLayerWeight(1, 1); // Uppe_rigidbodyody 레이어 가중치 1로 설정
        }
        if (secondaryState == SecondaryState.None)
        {
            _animator.SetLayerWeight(1, 0); // Uppe_rigidbodyody 레이어 가중치 1로 설정
        }
    }
}
