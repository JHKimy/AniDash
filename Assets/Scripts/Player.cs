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
            //  �̵� ���� ���
            Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            //  �̵� �Է��� ���� ���� ȸ�� (�Է��� ���� ���� ȸ�� ����)
            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 lookAtTarget = transform.position + moveDirection.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(lookAtTarget - transform.position);

                //  Slerp�� ����� �ε巴�� ȸ�� (���۽����� �ݴ� ���� ȸ�� ����)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothness);
            }
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

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");


        
        
        // ���� transform.position�� ������Ʈ�Ͽ� �̵� ó��
        transform.position += moveDirection.normalized * finalSpeed * Time.deltaTime;





        // float percent = ((isRunning) ? 1 : 0.5f) * moveDirection.magnitude;
        //_animator.SetFloat("vInput", percent, 0.1f, Time.deltaTime);
        
        _animator.SetFloat("hInput", Input.GetAxis("Horizontal")    );
        _animator.SetFloat("vInput", Input.GetAxis("Vertical")      );
        _animator.SetBool("isRun", isRunning);
        
    }
}
