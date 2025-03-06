using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // 컴포넌트
    private Camera _camera;
    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerState _playerState;

    // 회전 관련 변수
    Vector3 moveDir = Vector3.zero;
    public float rotationSpeed = 50f;

    // 이동 관련 변수
    public float speed = 5f;
    public float runSpeed = 8f;
    private float finalSpeed;

    // 점프
    public float jumpForce = 7f;

    // 슬라이딩
    private float slideSpeed = 15f;
    public float knockbackForce = 30f;
    private float slideCooldown = 2f;
    private float lastSlideTime = -1f; // 마지막 슬라이드 실행 시간 (초기값을 음수로 설정해 첫 실행 허용)

    // 추락 관련 변수
    private float groundCheckDistance = 5f;  // 발 위치에서 아래로 체크할 거리
    private bool isGrounded = true;

    private float fallStartTime = 0f;  // 떨어지기 시작한 시간
    private float fallThreshold = 0.1f;  // 1초 이상 떨어졌을 때만 isFalling 활성화
    private float accelFallThreshold = 3.5f;  // 3초 이상 떨어졌을 때 isAccelFalling 활성화ㄴ



    void Start()
    {
        // 현재 오브젝트에 있는 컴포넌트 참조
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

        // 뛰는 상태 아니면 Walking 상태로
        if (_playerState.isGrounded)
        {
            if (_playerState.currentState != PlayerState.State.Running)
            {
                _playerState.SetState(PlayerState.State.Walking);
            }
        }

        // 물체 들고 있으면 속도 낮추기
        float holdSpeed = _playerState.IsHoldingObject() ? 0.6f : 1f;
        finalSpeed = (_playerState.currentState == PlayerState.State.Running ? runSpeed : speed) * holdSpeed;


        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 움직임 방향
        moveDir = (cameraForward * _playerState.vAxis + cameraRight * _playerState.hAxis).normalized;

        // 달리거나 슬라이딩할때 그 방향으로 캐릭터 회전
        if (_playerState.currentState == PlayerState.State.Running
            || _playerState.currentState == PlayerState.State.Sliding)
        {
            transform.forward = moveDir;
        }

        // 움직임
        transform.position += moveDir * finalSpeed * Time.deltaTime;
        // _rigidbody.MovePosition(transform.position + moveDir * finalSpeed * Time.fixedDeltaTime);

    }

    void Rotate()
    {
        if (_playerState.keyAltCamera) return;
        // if (_playerState.currentState == PlayerState.State.Running) return;
        // 달리거나 슬라이딩시 회전방향 정의 
        // Vector3 targetDir = (_playerState.isRunning || _playerState.isSliding) ? transform.forward : _camera.transform.forward;
        // targetDir.y = 0;
        //if (_playerState.currentState == PlayerState.State.Running) return;
        //Debug.Log("dfsdf");

        Vector3 forward =
        (_playerState.currentState == PlayerState.State.Running
            || _playerState.currentState == PlayerState.State.Sliding)
            ? transform.forward : _camera.transform.forward;
        forward.y = 0;

        // 회전
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
            _playerState.keySlide = false; // 중복 실행 방지
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
            //  앞으로 강하게 밀기 (몸통 박치기)
            // 0.2초 후 슬라이드 속도 증가
            //Invoke("IncreaseSlideSpeed", 0.2f);

            Invoke("SlideOut", 0.25f);
            // _playerState.SetState("isSliding", true);
        }
    }
    // 속도 증가 함수 (0.2초 후 실행)
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

        // 슬라이드가 끝나면 속도 줄이기
        _rigidbody.linearVelocity = Vector3.zero;

        // 슬라이드 후 다시 걷기(Walking) 상태로 변경
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
        // if (_playerState.isParkouring) return; // 파쿠르 중이면 낙하 체크 X


        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool wasGrounded = isGrounded;  // 이전 프레임의 지면 상태 저장
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance);

        // Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, Color.red, 0.1f);

        // Debug.Log("Ground : " + isGrounded);



        // 바닥에서 떨어지는 순간 타이머 시작
        if (!isGrounded && wasGrounded)
        {
            fallStartTime = Time.time;  // 현재 시간 저장
            _playerState.isGrounded = false;
        }

        float fallDuration = Time.time - fallStartTime;  // 떨어진 시간 계산
        // Debug.Log(fallDuration);

        // 0.2초 이상 공중에 있으면 isFalling 활성화 → Falling 상태로 전이
        if (!isGrounded && _playerState.currentState != PlayerState.State.Jumping && fallDuration >= fallThreshold)
        {
            _playerState.SetState(PlayerState.State.Falling);

            // 3초 이상 공중에 있으면 isAccelFalling 활성화 → AccelFalling 상태로 전이
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