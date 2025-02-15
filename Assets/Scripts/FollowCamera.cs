using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;


public class FollowCamera : MonoBehaviour
{
    private Player _player;

    public Transform character; // Ÿ��(ĳ����)
    public float followSpeed = 50f;
    public float sensitivity = 500f;
    public float clampAngle = 80f;

    private float rotX;
    private float rotY;

    public Vector3 offset = new Vector3(0, 2, -7); // ĳ���� ���� ���� ��ġ�� ������
    public float minDistance = 1f;
    public float maxDistance = 5f;
    public float smoothness = 500f;

    private float finalDistance;

    void Start()
    {

        _player = character.GetComponent<Player>();

        // offset���� ī�޶� �ִ� �Ÿ� ����
        maxDistance = offset.magnitude;
        finalDistance = maxDistance;

        // ���� ī�޶��� X, Y ȸ�� ���� ��������
        Vector3 initialRotation = transform.rotation.eulerAngles;
        rotX = initialRotation.x > 180f ? initialRotation.x - 360f : initialRotation.x;
        rotY = initialRotation.y;

        // ���콺 Ŀ�� ���
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }


    void Update()
    {
        if (!_player.isRunning) // �Ȱų� ������ ���� ī�޶� ���� ����
        {
            rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
            rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(rotX, rotY, 0),
                Time.deltaTime * followSpeed);
        }
        else 
        {
            transform.position = character.position + offset;
            transform.LookAt(character.position);
        }
    }

    void LateUpdate()
    {
        if (character == null) return;

        if (!_player.isRunning)
        {





            // ī�޶� ��ǥ ��ġ ��� (ĳ������ ��ġ + offset)
            //Vector3 targetPosition = character.position + transform.rotation * offset;
            Vector3 targetPosition = character.position + offset;

            RaycastHit hit;
            Vector3 dirToTarget = targetPosition - character.position;

            if (Physics.Raycast(character.position + Vector3.up * 0.5f, dirToTarget.normalized, out hit, maxDistance))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }

            // ĳ���� ��ġ + (���� ��ġ * ������ ���� * ���� �Ÿ�)
            Vector3 finalPos = character.position + (transform.rotation * offset.normalized * finalDistance);

            // ī�޶� ��ġ ����
            transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * smoothness);

        }
    }
}
