using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Animator _animator;
    Camera _camera;
    CharacterController _controller;

    public float speed = 5f;
    public float runSpeed = 8f;
    public float finalSpeed;

    public bool altCamera;
    public float smoothness = 50f;
    public bool isRunning;

    Vector3 moveDirection = Vector3.zero; // 현재 이동 방향 저장

    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    // Animator anim;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _camera = Camera.main;
        // _controller = this.GetComponent<CharacterController>();
    }

    void Update()
    {
        //hAxis = Input.GetAxisRaw("Horizontal");
        //vAxis = Input.GetAxisRaw("Vertical");
        //wDown = Input.GetButton("Jog");

        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //// shift 누르면 3.f, 안 누르면 1.f
        //transform.position += moveVec * speed * (wDown ? 3f : 1f) * Time.deltaTime;

        //anim.SetBool("isJog", moveVec != Vector3.zero);
        //anim.SetBool("isRun", wDown);

        //transform.LookAt(transform.position + moveVec);

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            altCamera = true;
        }
        else{
            altCamera = false;
        }

        if (Input.GetButton("Run"))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        // 덧 입혀짐
        InputMovement();
    }

    void LateUpdate()
    {
        if (isRunning)
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");

            Vector3 moveVec = new Vector3(h, 0, v).normalized;

            transform.LookAt(transform.position + moveVec);

        }

        // 캐릭터 회전 기본 설정
        // alt 안눌렸을때와 안 뛸때
        if (!altCamera && !isRunning)
        {
            // 요소별 곱셈
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));

            // 플레이어가 바라볼 방향으로 천천이 구형보간
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }   
    }

    void InputMovement()
    {
        finalSpeed = isRunning ? runSpeed : speed;



        if (!isRunning)
        {
            // 로컬을 월드 좌표로
            // 현재 transform 기준으로 앞방향
            // 캐릭터가 회전하면 보고 있는 부분이 앞방향
            // 따라서 w하면 무조건 앞
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
            transform.position += moveDirection.normalized * finalSpeed * Time.deltaTime;

        }
        else
        {
            // 캐릭터가 회전하든 말든, W/S는 항상 월드 기준 앞뒤, A/D는 항상 좌우 이동
            Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;   
            transform.LookAt(transform.position + dir);
            transform.position += dir * finalSpeed * Time.deltaTime;


            //moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        }


        // 직접 transform.position을 업데이트하여 이동 처리





        // float percent = ((isRunning) ? 1 : 0.5f) * moveDirection.magnitude;
        //_animator.SetFloat("vInput", percent, 0.1f, Time.deltaTime);
        
        _animator.SetFloat("hInput", Input.GetAxis("Horizontal")    );
        _animator.SetFloat("vInput", Input.GetAxis("Vertical")      );
        _animator.SetBool("isRun", isRunning);
        
    }
}
