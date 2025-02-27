using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.HighDefinition;
using Unity.VisualScripting;

public class PlayerParkour : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private PlayerState _playerState;
    private Animator _animator;

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
        CheckForParkour();
        TryParkour();
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

                //Debug.Log("aaaaaaaadfs");
                //Debug.Log(objectTopY);
                //Debug.Log(height);


                if (height > 0.5f && height < 3f)
                {
                    canParkour = true;
                    Vector3 parkourOffset = transform.forward * 0.03f; // 플레이어가 보고 있는 방향으로 1m 이동
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
        // _playerState.SetState("isParkouring", true);
        _playerState.SetState(PlayerState.State.Parkouring);
        //_playerState.SetState("isFalling", false); // 파쿠르 중 낙하 상태 비활성화
        //_playerState.SetState("isAccelFalling", false);
        _rigidbody.useGravity = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = parkourPosition;

        AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
        float animLength = animState.length; // 애니메이션 길이 (초 단위)

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 3; // 속도 조절
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        _rigidbody.useGravity = true;

        yield return new WaitForSeconds(animLength);


        // _playerState.SetState("isParkouring", false);
        _playerState.SetState(PlayerState.State.Idle);


    }
}
