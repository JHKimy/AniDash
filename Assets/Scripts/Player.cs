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

    Vector3 moveDirection = Vector3.zero; // ���� �̵� ���� ����

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

        //// shift ������ 3.f, �� ������ 1.f
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

        // �� ������
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

        // ĳ���� ȸ�� �⺻ ����
        // alt �ȴ��������� �� �۶�
        if (!altCamera && !isRunning)
        {
            // ��Һ� ����
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));

            // �÷��̾ �ٶ� �������� õõ�� ��������
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }   
    }

    void InputMovement()
    {
        finalSpeed = isRunning ? runSpeed : speed;



        if (!isRunning)
        {
            // ������ ���� ��ǥ��
            // ���� transform �������� �չ���
            // ĳ���Ͱ� ȸ���ϸ� ���� �ִ� �κ��� �չ���
            // ���� w�ϸ� ������ ��
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
            transform.position += moveDirection.normalized * finalSpeed * Time.deltaTime;

        }
        else
        {
            // ĳ���Ͱ� ȸ���ϵ� ����, W/S�� �׻� ���� ���� �յ�, A/D�� �׻� �¿� �̵�
            Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;   
            transform.LookAt(transform.position + dir);
            transform.position += dir * finalSpeed * Time.deltaTime;


            //moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        }


        // ���� transform.position�� ������Ʈ�Ͽ� �̵� ó��





        // float percent = ((isRunning) ? 1 : 0.5f) * moveDirection.magnitude;
        //_animator.SetFloat("vInput", percent, 0.1f, Time.deltaTime);
        
        _animator.SetFloat("hInput", Input.GetAxis("Horizontal")    );
        _animator.SetFloat("vInput", Input.GetAxis("Vertical")      );
        _animator.SetBool("isRun", isRunning);
        
    }
}
