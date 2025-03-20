using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Sector1Platform : MonoBehaviour, PlayerState.PlayerObserver
{
    private Rigidbody _rigidbody;

    private Vector3 startPos;
    private Vector3 targetPos;
    public float moveDistance = 5.0f; // 이동 거리

    public float moveSpeed = 3f;

    public bool canMove = false;
    public bool isPlayerOnPlatform = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        startPos = transform.position;
        targetPos = startPos + new Vector3(0, 0, moveDistance); // Z축 방향 이동

        PlayerState playerState = FindObjectOfType<PlayerState>();
        if (playerState != null)
        {
            playerState.AttachObserver(this); // 옵저버 등록
        }
    }

    void OnDestroy()
    {
        PlayerState playerState = FindObjectOfType<PlayerState>();

        if (playerState != null)
        {
            playerState.DetachObserver(this); // 옵저버 해제
        }
    }

    public void OnPlayerStateChanged(PlayerState playerState)
    {
        if (playerState.key >= 5)
        {
            canMove = true; // 키 5개 이상이면 이동 시작
        }
        else
        {
            canMove = false; // 키 개수가 5개 미만이면 이동 멈춤
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

    // 플레이어가 플랫폼에 올라왔을 때 감지
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true; // 플레이어가 올라옴
            collision.transform.parent = transform;
        }
    }

    // 플레이어가 플랫폼에서 내려갔을 때 감지
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false; // 플레이어가 내려감
            collision.transform.parent = null;
            // canMove = false; // 즉시 멈추기
        }
    }
}
