using UnityEngine;
using System.Collections;

public class DropObject : MonoBehaviour
{
    private float knockbackForce = 20f;
    private PlatformDrop platformDrop;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetPool(PlatformDrop dropController)
    {
        platformDrop = dropController;
    }

    private void OnEnable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // 올바른 속도 초기화
            rb.angularVelocity = Vector3.zero;
        }

        StartCoroutine(ReturnToPool());
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = other.rigidbody;
            if (playerRb != null && !playerRb.isKinematic)
            {
                Vector3 knockbackDir = - other.transform.forward.normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            }
        }

        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Floor"))
        {
            ReturnToPool();
        }
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(2f);
        platformDrop.ReturnToPool(this);
    }

}
