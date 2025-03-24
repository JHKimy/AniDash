using UnityEngine;
using System.Collections;

public class PlayerParkour : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private PlayerState _playerState;
    private Animator _animator;

    // ���� üũ �� �̵� ���� �Ķ����
    public float raycastHeight = 1f;
    public float raycastDistance = 1f;
    public float minParkourHeight = 0.5f;
    public float maxParkourHeight = 3f;
    public float parkourOffset = 0.03f;
    public float parkourVerticalOffset = 0.2f;
    public float parkourLerpDuration = 0.3f; // �̵� ���� �ð�

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
        // Ŭ���̹� ������ ���� Ŭ���̹� ��ũ��Ʈ���� ������ ȣ���ϹǷ� ���⼱ �������� ����
        if (_playerState.currentState == PlayerState.State.Climbing)
            return;

        CheckForParkour();
        TryParkour();
    }

    // �ٴ�(�Ǵ� ��ֹ�)�� ������ �����ϴ��� üũ (�Ϲ� ����)
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

    // �Ϲ� ��Ȳ���� �����̽� Ű�� ������ �� ���� ����
    void TryParkour()
    {
        if (canParkour && Input.GetKeyDown(KeyCode.Space))
        {
            _animator.CrossFade("Parkour", 0.2f);
            _rigidbody.linearVelocity = Vector3.zero;
            StartCoroutine(ParkourMove(parkourPosition));
        }
    }

    // �ܺ�(��: PlayerClimb)���� ȣ���� �� �ֵ��� targetPos�� �Ű������� ����
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
