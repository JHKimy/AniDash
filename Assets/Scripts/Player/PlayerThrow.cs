using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    private Camera _camera;
    private Rigidbody _rigidbody;
    private PlayerState _playerState;

    private GameObject grabbedObject = null;
    public Transform grabPostion; // �ڽ��� �� ��ġ
    private float grabRange = 2f;
    private float throwForce = 10f;

    public LineRenderer trajectoryLine; // �������� ǥ���� LineRenderer
    private int trajectorySegmentCount = 30; // ���� �� ����
    private float trajectoryTimeStep = 0.05f; // �� �� �� �ð� ���� (0.05��)

    void Start()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        DetectPickupObject();

        if (grabbedObject != null)
        {
            DrawTrajectory();
        }
    }

    void DetectPickupObject()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * grabRange, Color.red, 0.1f);

        if (grabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, grabRange))
            {
                if (hit.collider.CompareTag("Box"))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        GrabObject(hit.collider.gameObject);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E)) // �ٽ� EŰ�� ������ ����
            {
                ThrowObject();
            }
        }
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true; // ���� ������ ���� �ʵ��� ����
        }

        _playerState.SetSecondaryState(PlayerState.SecondaryState.HoldingObject);

        // ĳ������ �� ��ġ�� �̵�
        grabbedObject.transform.position = grabPostion.position;
        grabbedObject.transform.rotation = grabPostion.rotation;
        grabbedObject.transform.SetParent(grabPostion);
        trajectoryLine.enabled = true; // ���� Ȱ��ȭ (����� ��)


    }

    void ThrowObject()
    {
        if (grabbedObject != null)
        {
            Box thrownBox = grabbedObject.GetComponent<Box>();
            thrownBox.MarkAsThrown(); // isThrown = true�� ����



            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();

            grabbedObject.transform.SetParent(null); // �θ� ����
            rb.isKinematic = false; // ���� ����

            // ������ ���� ���� (ī�޶� �������� ���� ����)
            Vector3 throwDirection = (_camera.transform.forward + _camera.transform.up * 0.3f).normalized;

            // ������ ����� �ӵ� ��� (������ ���� ������ �����ϰ� ����)
            Vector3 throwVelocity = throwDirection * Mathf.Sqrt(2 * throwForce * Mathf.Abs(Physics.gravity.y) / rb.mass);

            rb.linearVelocity = throwVelocity; // �ӵ� ���� ����

            grabbedObject = null; // ���� ���� ����
            _playerState.SetSecondaryState(PlayerState.SecondaryState.None);
        }
    }

    void DrawTrajectory()
    {
        if (grabbedObject == null)
        {
            trajectoryLine.enabled = false;
            return;
        }

        trajectoryLine.enabled = true;

        // ���� ��ġ : �ڽ��� ��� �ִ� ��ġ (�� ��ġ)
        Vector3 startPos = grabPostion.position;

        // ������ ���� ��� (ī�޶� �������� ���� ����)
        Vector3 throwDirection = (_camera.transform.forward + _camera.transform.up * 0.3f).normalized;

        // Rigidbody�� ������ ����� �ʱ� �ӵ�
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        float mass = rb != null ? rb.mass : 1f;

        // ������ �ӵ��� ThrowObject�� �����ϰ� ����
        Vector3 initialVelocity = throwDirection * Mathf.Sqrt(2 * throwForce * Mathf.Abs(Physics.gravity.y) / mass);

        // ������ ���� ���
        Vector3[] trajectoryPoints = new Vector3[trajectorySegmentCount + 1];

        for (int i = 0; i <= trajectorySegmentCount; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector3 point = startPos + initialVelocity * t + 0.5f * Physics.gravity * t * t;
            trajectoryPoints[i] = point;
        }

        trajectoryLine.positionCount = trajectorySegmentCount;
        trajectoryLine.SetPositions(trajectoryPoints);
    }
}
