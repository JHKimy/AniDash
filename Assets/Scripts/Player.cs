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

    // ȸ�� ���� ����
    Vector3 moveDirection = Vector3.zero;
    public float rotationSpeed = 50f;

    // �̵� ���� ����
    public float speed = 5f;
    public float runSpeed = 8f;
    private float finalSpeed;
    private bool isMoving = false;
    private bool  isJump;

    // �Է� ���� ����
    private float hAxis;
    private float vAxis;
    private bool  jumpButton;

    private bool isRunning;
    private bool altCamera;

    public bool GetIsRunning() { return isRunning; }

    //================================================

    void Start()
    {
        _camera = Camera.main;  // ���� ī�޶� ����

        // ���� ������Ʈ�� �ִ� ������Ʈ ����
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

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
        SetAnimation();
    }

    void LateUpdate()
    {
    }

    void Rotate()
    {
        if (altCamera) return;

        Vector3 playerRotate = isRunning ? transform.forward : _camera.transform.forward;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(Vector3.Scale(playerRotate, new Vector3(1, 0, 1))),
            Time.deltaTime * rotationSpeed
        );


    }

    void Move()
    {
        finalSpeed = isRunning ? runSpeed : speed;

        // ī�޶� ���� �̵� ���� ���
        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;

        cameraForward.y = 0; // ���� �̵� ����
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // ���� �Է¿� ���� �̵� ���� ���
        Vector3 inputDirection = (cameraForward * vAxis + cameraRight * hAxis).normalized;

        if (isRunning && inputDirection != Vector3.zero)
        {
            transform.forward = inputDirection;
            moveDirection = transform.forward;
        }
        else
        {
            moveDirection = inputDirection;
        }


        // �̵� ����
        transform.position += moveDirection * finalSpeed * Time.deltaTime;


    }

    void HandleInput()
    {
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        isMoving = (Mathf.Abs(hAxis) > 0.1f || Mathf.Abs(vAxis) > 0.1f) ? true : false;
        isRunning = Input.GetButton("Run");
        altCamera = Input.GetKey(KeyCode.LeftAlt);
        jumpButton = Input.GetButtonDown("Jump");
    }

    void SetAnimation()
    {
        _animator.SetFloat("vInput", vAxis);
        _animator.SetFloat("hInput", hAxis);
        _animator.SetBool("isRun", isRunning && isMoving);

    }

    void Jump()
    {
        if (jumpButton && !isJump)
        {
            _rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
            _animator.SetTrigger("doJump");
            _animator.SetBool("isJump", true);
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            _animator.SetBool("isJump", false);
            isJump = false;
        }
    }

}
