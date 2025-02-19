using TMPro;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Camera _camera;
    Animator _animator;
    Rigidbody _rigidbody;

    // 회전 관련 변수
    Vector3 moveDirection = Vector3.zero;
    public float rotationSpeed = 50f;

    // 이동 관련 변수
    public float speed = 5f;
    public float runSpeed = 8f;
    private float finalSpeed;
    private bool isMoving = false;
    private bool isJump;
    private bool isSlide;

    // 입력 관련 변수
    private float hAxis;
    private float vAxis;
    private bool spaceBar;
    private bool slideKey;

    private bool isRunning;
    private bool altCamera;
    

    // 오브젝트 상호작용
    private GameObject grabbedObject = null;
    public Transform grabPostion; // 박스를 들 위치
    public float grabRange = 2f;
    public float throwForce = 10f;

    // 벽타기
    public float climbSpeed = 3f;
    public float jumpOffForce = 5f;
    public LayerMask wallLayer;

    private bool isClimbing = false;
    private bool isWallDetected = false;
    private Vector3 wallNormal;


    public bool GetIsRunning() { return isRunning; }

    //================================================

    void Start()
    {
        _camera = Camera.main;  // 메인 카메라 참조

        // 현재 오브젝트에 있는 컴포넌트 참조
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        // grabPostion이 설정되지 않았다면 자동으로 찾기
        if (grabPostion == null)
        {
            grabPostion = GameObject.Find("grabPosition").transform;
        }

    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        HandleInput();
        Rotate();
        Jump();
        Slide();
        DetectPickupObject(); // 감지 기능 실행
        WallCheck(); // 벽 감지

        // W를 누르고 있는 동안만 벽타기 실행
        if (isWallDetected)
        {
            if (!isClimbing) // 처음 W를 눌렀을 때만 벽타기 시작
            {
                StartWallClimb();
            }
            ClimbWall(); // 계속 W를 누르고 있어야만 벽을 오름
        }
        if (isClimbing && isJump) // W를 떼면 벽타기 종료
        {
            EndWallClimb();
        }

        // 점프하면 벽타기 종료
        if (isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            JumpOffWall();
        }

        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red);

        SetAnimation();
    }


    void WallCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.5f; // 플레이어 위치 기준으로 벽 감지
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, 1f))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                isWallDetected = true;
                wallNormal = hit.normal;
            }
        }
        else
        {
            isWallDetected = false;
            if (isClimbing)
            {
                EndWallClimb();
            }
        }
    }

    void StartWallClimb()
    {
        isClimbing = true;
        _rigidbody.useGravity = false; // 중력 비활성화
        _rigidbody.linearVelocity = Vector3.zero;
    }

    void ClimbWall()
    {
        _rigidbody.linearVelocity = new Vector3(0, vAxis * climbSpeed, 0);
    }

    void JumpOffWall()
    {
        isClimbing = false;
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = wallNormal * jumpOffForce + Vector3.up * jumpOffForce; // 벽에서 튕겨나가는 점프
    }

    void EndWallClimb()
    {
        isClimbing = false;
        _rigidbody.useGravity = true;
    }



    void LateUpdate()
    {
    }

    void Rotate()
    {
        if (altCamera) return; // 슬라이딩 중엔 회전 방지

        // 기본 회전
        Vector3 playerRotate = (isRunning || isSlide) ? transform.forward : _camera.transform.forward;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(Vector3.Scale(playerRotate, new Vector3(1, 0, 1))),
            Time.deltaTime * rotationSpeed
        );


    }

    void Move()
    {
        if (isClimbing) return;

        finalSpeed = isRunning ? runSpeed : speed;

        // 카메라 기준 이동 방향 계산
        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;

        // y 값 제거 후 한 번만 정규화
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();


        // 현재 입력에 따른 이동 방향 계산
        moveDirection = (cameraForward * vAxis + cameraRight * hAxis).normalized;

        // 달리기나 슬라이딩 할때 움직이는 방향으로 캐릭터 전방벡터 설정
        if ((isRunning || isSlide) && isMoving)
        {
            transform.forward = moveDirection;
        }


        // 이동 적용
        transform.position += moveDirection * finalSpeed * Time.deltaTime;


    }

    void HandleInput()
    {
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        isMoving = (Mathf.Abs(hAxis) > 0.1f || Mathf.Abs(vAxis) > 0.1f) ? true : false;
        isRunning = Input.GetButton("Run");
        altCamera = Input.GetKey(KeyCode.LeftAlt);
        spaceBar = Input.GetButtonDown("Jump");
        slideKey = Input.GetButton("Slide");

    }

    void SetAnimation()
    {
        if (!isClimbing)
        {
            _animator.SetFloat("vInput", vAxis);
            _animator.SetFloat("hInput", hAxis);
            _animator.SetBool("isRun", isRunning && isMoving);
        }
        _animator.SetBool("isClimbing", isClimbing);
        _animator.SetFloat("climbInput", vAxis);

    }

    void Jump()
    {
        if (spaceBar && !isJump)
        {
            _rigidbody.AddForce(Vector3.up * 7, ForceMode.Impulse);
            _animator.SetTrigger("doJump");
            _animator.SetBool("isJump", true);
            isJump = true;
        }
    }

    void Slide()
    {
        if (slideKey && !isSlide && isMoving)
        {
            speed *= 4;
            _animator.SetTrigger("doSlide");
            isSlide = true;

            //// 슬라이드 시작 시 바라보는 방향을 현재 이동 방향으로 설정
            //if (moveDirection != Vector3.zero)
            //{
            //    transform.forward = moveDirection;
            //}

            Invoke("SlideOut", 0.7f);
        }
    }

    void SlideOut()
    {
        speed *= 0.25f;
        isSlide = false;

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _animator.SetBool("isJump", false);
            isJump = false;
        }
    }

    void DetectPickupObject()
    {
        if(grabbedObject == null)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
            {
                if (hit.collider.CompareTag("Box"))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        GrabObject(hit.collider.gameObject);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E)) // 다시 E키를 누르면 던짐
            {
                ThrowObject();
            }
        }
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        Rigidbody _rigidbody = grabbedObject.GetComponent<Rigidbody>();

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = true; // 물리 영향을 받지 않도록 설정
        }

        _animator.SetLayerWeight(1, 1); // Uppe_rigidbodyody 레이어 가중치 1로 설정

        // 캐릭터의 손 위치로 이동
        grabbedObject.transform.position = grabPostion.position;
        grabbedObject.transform.rotation = grabPostion.rotation;
        grabbedObject.transform.SetParent(grabPostion);
    }

    void ThrowObject()
    {
        if (grabbedObject != null)
        {
            Rigidbody _rigidbody = grabbedObject.GetComponent<Rigidbody>();

            grabbedObject.transform.SetParent(null); // 부모 해제
            _rigidbody.isKinematic = false; // 물리 적용
            _rigidbody.AddForce(transform.forward * throwForce, ForceMode.Impulse); // 앞으로 던지기

            grabbedObject = null; // 손에서 놓기

            // 던진 후에는 상체 애니메이션 비활성화g
            _animator.SetLayerWeight(1, 0); // Uppe_rigidbodyody 레이어 가중치 0으로 설정
        }
    }

}
