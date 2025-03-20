using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Sector1Platform : MonoBehaviour, PlayerState.PlayerObserver
{
    private Rigidbody _rigidbody;

    private Vector3 startPos;
    private Vector3 targetPos;
    public float moveDistance = 5.0f; // �̵� �Ÿ�

    public float moveSpeed = 3f;

    public bool canMove = false;
    public bool isPlayerOnPlatform = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        startPos = transform.position;
        targetPos = startPos + new Vector3(0, 0, moveDistance); // Z�� ���� �̵�

        PlayerState playerState = FindObjectOfType<PlayerState>();
        if (playerState != null)
        {
            playerState.AttachObserver(this); // ������ ���
        }
    }

    void OnDestroy()
    {
        PlayerState playerState = FindObjectOfType<PlayerState>();

        if (playerState != null)
        {
            playerState.DetachObserver(this); // ������ ����
        }
    }

    public void OnPlayerStateChanged(PlayerState playerState)
    {
        if (playerState.key >= 5)
        {
            canMove = true; // Ű 5�� �̻��̸� �̵� ����
        }
        else
        {
            canMove = false; // Ű ������ 5�� �̸��̸� �̵� ����
        }
    }

    void Update()
    {
        if (canMove && isPlayerOnPlatform)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            //_rigidbody.linearVelocity = transform.forward * moveSpeed;
        }
        //else
        //{
        //    _rigidbody.linearVelocity = Vector3.zero;
        //}
    }

    // �÷��̾ �÷����� �ö���� �� ����
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true; // �÷��̾ �ö��
            collision.transform.parent = transform;
        }
    }

    // �÷��̾ �÷������� �������� �� ����
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false; // �÷��̾ ������
            collision.transform.parent = null;
            // canMove = false; // ��� ���߱�
        }
    }
}
