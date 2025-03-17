using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public interface IState
{
    void Enter();    // 상태 진입 시 호출
    void Update();   // 매 프레임 호출
    void Exit();     // 상태 종료 시 호출
}




public class PlayerState : MonoBehaviour
{
    public Transform target;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private Vector3 previousPosition;
    public float speed;

    // private float groundCheckDistance = 1.1f;  // 바닥 감지 거리
    public bool isGrounded = true;  // 현재 바닥에 있는지 여부





    public float health = 100;
    public float maxHealth = 100;
    public float stamina = 100;
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



        // 스태미나 감소 처리
        if (currentState == State.Running)
        {
            stamina -= 10f * Time.fixedDeltaTime;  // 초당 10 감소 (원하는 값으로 조정 가능)
            stamina = Mathf.Max(0, stamina);  // 스태미나가 0 이하로 내려가지 않도록 방지

            //// 스태미나가 0이 되면 달리기 불가능하게 설정
            //if (stamina <= 0)
            //{
            //    SetState(State.Walking); // 다시 걷는 상태로 변경
            //}
        }
        else if (currentState == State.Walking && isGrounded)
        {
            // 걷거나 멈춰있을 때 스태미나 회복
            stamina += 5f * Time.fixedDeltaTime;  // 초당 5 회복 (원하는 값으로 조정 가능)
            stamina = Mathf.Min(maxStamina, stamina);  // 최대 스태미나를 초과하지 않도록 방지
        }




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
        // Debug.Log(currentState);
        // Debug.Log(secondaryState);
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

        if (collision.gameObject.tag == "Key") // 수정된 부분
        {
            key++;
        }
    }

    void SetInput()
    {
        // 키입력 받기
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        keyJump = Input.GetButtonDown("Jump");
        keySlide = Input.GetButton("Slide");
        keyAltCamera = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetButton("Run") && isGrounded &&
            currentState == State.Walking
            && secondaryState != SecondaryState.HoldingObject &&
            stamina > 0) 
        {   
            SetState(State.Running);
        }
        else if ((!Input.GetButton("Run") || stamina <= 0) && currentState == State.Running)
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


    public void TakeDamage(int damage)
    {
        health -= damage;
        // Debug.Log($"플레이어가 {damage}의 피해를 입음. 현재 체력: {health}");

        if (health <= 0)
        {
        }
    }

}
