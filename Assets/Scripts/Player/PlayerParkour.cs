using UnityEngine;
using System.Collections;

public class PlayerParkour : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private PlayerState _playerState;
    private Animator _animator;

    // 파쿠르 체크 및 이동 관련 파라미터
    public float raycastHeight = 1f;
    public float raycastDistance = 1f;
    public float minParkourHeight = 0.5f;
    public float maxParkourHeight = 3f;
    public float parkourOffset = 0.03f;
    public float parkourVerticalOffset = 0.2f;
    public float parkourLerpDuration = 0.3f; // 이동 보간 시간

    private bool canParkour = false;
    private Vector3 parkourPosition;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        // 클라이밍 상태일 때는 클라이밍 스크립트에서 파쿠르를 호출하므로 여기선 실행하지 않음
        if (_playerState.currentState == PlayerState.State.Climbing)
            return;

        CheckForParkour();
        TryParkour();
    }

    // 바닥(또는 장애물)의 조건을 만족하는지 체크 (일반 파쿠르)
    void CheckForParkour()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * raycastHeight;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance))
        {
            //if (hit.collider.CompareTag("Floor"))
            //{
                float objectTopY = hit.collider.bounds.max.y;
                float height = objectTopY - transform.position.y;
                if (height > minParkourHeight && height < maxParkourHeight)
                {
                    canParkour = true;
                    Vector3 offset = transform.forward * parkourOffset;
                    
                    parkourPosition = 
                    new Vector3(hit.point.x + offset.x,
                    objectTopY + parkourVerticalOffset,
                    hit.point.z + offset.z);

                    return;
                }
            //}
        }
        canParkour = false;
    }

    // 일반 상황에서 스페이스 키를 눌렀을 때 파쿠르 실행
    void TryParkour()
    {
        if (canParkour && Input.GetKeyDown(KeyCode.Space))
        {
            _animator.CrossFade("Parkour", 0.2f);
            _rigidbody.linearVelocity = Vector3.zero;
            StartCoroutine(ParkourMove(parkourPosition));
        }
    }

    // 외부(예: PlayerClimb)에서 호출할 수 있도록 targetPos를 매개변수로 받음
    public void ExecuteParkour(Vector3 targetPos)
    {
        StartCoroutine(ParkourMove(targetPos));
    }

    IEnumerator ParkourMove(Vector3 targetPos)
    {
        _playerState.SetState(PlayerState.State.Parkouring);
        _animator.CrossFade("Parkour", 0.2f);
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.useGravity = false;

        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < parkourLerpDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / parkourLerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        _rigidbody.useGravity = true;

        yield return new WaitForSeconds(0.2f);
        OnParkourEnd();
    }

    public void OnParkourEnd()
    {
        _playerState.SetState(PlayerState.State.Idle);
    }
}
