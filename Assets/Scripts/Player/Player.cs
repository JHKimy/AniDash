using System.Collections;
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
    public float throwForce = 100f;

    // 벽타기
    public float climbSpeed = 3f;
    public float jumpOffForce = 5f;
    public LayerMask wallLayer;

    private bool isClimbing = false;
    private bool isWallDetected = false;
    private Vector3 wallNormal;

    // 파쿠르
    private bool canParkour;
    private bool isParkouring;
    Vector3 parkourPosition;



    public LineRenderer trajectoryLine; // 유도선을 표시할 LineRenderer
    public int trajectorySegmentCount = 100; // 궤적 점의 개수
    public float trajectoryTimeStep = 5f; // 각 점 간 시간 간격



    public float knockbackForce = 10f; // 충돌 시 오브젝트에 적용할 넉백 효과

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
        CheckForParkour();
        TryParkour();

        // 벽타기 관련 처리...
        if (isWallDetected)
        {
            if (!isClimbing)
            {
                StartWallClimb();
            }
            ClimbWall();
        }
        if (isClimbing && isJump)
        {
            EndWallClimb();
        }
        if (isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            JumpOffWall();
        }

        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red);
        SetAnimation();

        // 박스를 들고 있을 때 유도선을 갱신
        if (grabbedObject != null)
        {
            DrawTrajectory();
        }
        else if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
    }


    void CheckForParkour()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, 1f))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                float objectTopY = hit.collider.bounds.max.y; // 장애물의 최상단 높이
                float height = objectTopY - transform.position.y;

                Debug.Log("aaaaaaaadfs");
                Debug.Log(objectTopY);
                Debug.Log(height);


                if (height > 0.5f && height < 3f)
                {
                    canParkour = true;
                    Vector3 parkourOffset = transform.forward * 0.02f; // 플레이어가 보고 있는 방향으로 1m 이동
                    parkourPosition = new Vector3(hit.point.x + parkourOffset.x, objectTopY + 0.3f, hit.point.z + parkourOffset.z);
                }
                else
                {
                    canParkour = false;
                }
            }
        }
        else
        {
            canParkour = false;
        }
    }

    void TryParkour()
    {
        if (canParkour && Input.GetKeyDown(KeyCode.O))
        {
            // _animator.applyRootMotion = true; // 애니메이션이 캐릭터를 이동시키지 않도록 막음

            _animator.CrossFade("Parkour", 0.2f);
            // _animator.applyRootMotion = true;
            StartCoroutine(ParkourMove());
            // _animator.applyRootMotion = false;
        }
    }
    IEnumerator ParkourMove()
    {
        isParkouring = true;
        _rigidbody.useGravity = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = parkourPosition;

        // AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
        // float animLength = animState.length; // 애니메이션 길이 (초 단위)


        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 3; // 속도 조절
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // yield return new WaitForSeconds(animLength);


        _rigidbody.useGravity = true;
        isParkouring = false;
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
        // 벽의 노말값을 기준으로 좌우 이동 축 계산
        Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up).normalized;
        // 벽에 평행한 업벡터
        Vector3 wallUp = Vector3.Cross(wallTangent, wallNormal).normalized;

        Vector3 desiredMovement = (wallUp * vAxis + wallTangent * hAxis) * climbSpeed;

        _rigidbody.linearVelocity = desiredMovement;
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
        if (isRunning || isSlide || altCamera) return; // 슬라이딩 중엔 회전 방지

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
        if (isClimbing || isParkouring) return;

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
        if (!isClimbing || !isParkouring)
        {
            _animator.SetFloat("vInput", vAxis);
            _animator.SetFloat("hInput", hAxis);
            _animator.SetBool("isRunning", isRunning & isMoving);
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
            speed *= 5;
            _animator.SetTrigger("doSlide");
            isSlide = true;

            //// 슬라이드 시작 시 바라보는 방향을 현재 이동 방향으로 설정
            //if (moveDirection != Vector3.zero)
            //{
            //    transform.forward = moveDirection;
            //}

            Invoke("SlideOut", 0.5f);
        }
    }

    void SlideOut()
    {
        speed *= 0.2f;
        isSlide = false;

    }

    void OnCollisionEnter(Collision collision)
    {
        // Interaction 태그를 가진 오브젝트와 슬라이딩 중일 때
        if (collision.gameObject.CompareTag("Interaction") && isSlide)
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 플레이어가 바라보는 방향(슬라이딩 방향)으로 힘을 가함
                rb.AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
            }
        }
        if (collision.gameObject.tag == "Floor")
        {
            _animator.SetBool("isJump", false);
            isJump = false;
        }
        if (grabbedObject == null)
        {
            if (collision.gameObject.CompareTag("Box"))
            {
                GrabObject(collision.collider.gameObject);
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

    void DetectPickupObject()
    {
        if (grabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
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
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();

            grabbedObject.transform.SetParent(null); // 부모 해제
            rb.isKinematic = false; // 물리 적용

            Vector3 throwDirection = (transform.forward + _camera.transform.up).normalized;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            grabbedObject = null; // 집은 상태 해제

            _animator.SetLayerWeight(1, 0);
        }
    }

    void DrawTrajectory()
    {
        if (grabbedObject == null)
        {
            trajectoryLine.enabled = false;
            return;
        }

        trajectoryLine.enabled = true;

        // 시작 위치 : 박스를 잡고 있는 위치 (손 위치)
        Vector3 startPos = grabPostion.position;

        // 던지는 방향 계산 (던지기 전과 동일한 방식)
        Vector3 throwDirection = (transform.forward + _camera.transform.up).normalized;

        // Rigidbody의 질량을 고려한 초기 속도
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        float mass = rb != null ? rb.mass : 1f;
        Vector3 initialVelocity = throwDirection * (throwForce / mass);

        // Inspector에서 조절한 값 사용 (예: trajectorySegmentCount = 10, trajectoryTimeStep = 2f)
        Vector3[] trajectoryPoints = new Vector3[trajectorySegmentCount];

        for (int i = 0; i < trajectorySegmentCount; i++)
        {
            float t = i * trajectoryTimeStep;
            // 포물선 궤적 공식: startPos + initialVelocity * t + 0.5 * Physics.gravity * t^2
            Vector3 point = startPos + initialVelocity * t + 0.5f * Physics.gravity * t * t;
            trajectoryPoints[i] = point;
        }

        trajectoryLine.positionCount = trajectorySegmentCount;
        trajectoryLine.SetPositions(trajectoryPoints);
    }

}
