using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Jog");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // shift 누르면 3.f, 안 누르면 1.f
        transform.position += moveVec * speed * (wDown ? 3f : 1f) * Time.deltaTime;

        anim.SetBool("isJog", moveVec != Vector3.zero);
        anim.SetBool("isRun", wDown);

        transform.LookAt(transform.position + moveVec);
    }
}
