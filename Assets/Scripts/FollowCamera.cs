using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;


public class FollowCamera : MonoBehaviour
{
    public Transform    characterTransform;    // ĳ���� ������
    private Player      character;             // ĳ���� ��ü

    // ī�޶� ��ġ ����
    public Vector3      offset              = new Vector3(0, 2, -7);
    private float       minDistance         = 1f;
    private float       maxDistance         = 5f;
    private float       finalDistance;
    private Vector3     finalPos;

    // ī�޶� ȸ�� ����
    private float       yaw                 = 0f;
    private float       pitch               = 0f;
    private float       clampAngle          = 80f;

    // �޸��� ���� ����
    Quaternion      runRot;

    // �Է� ����
    public float        mouseSensitivity    = 500f;



    private void Start()
    {   
        // ĳ���� ����
        character = characterTransform.GetComponent<Player>();

        // ĳ���Ϳ� �Ÿ� �ʱ� ����
        maxDistance = offset.magnitude;
        finalDistance = maxDistance;

        // ���콺 Ŀ�� ���
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        Rotation();
    }

    void LateUpdate()
    {
        Move();
    }

    void Rotation()
    {
        // ���콺 �Է� ��������
        yaw     -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // ���Ʒ� 
        pitch   += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // �¿�

        yaw     = Mathf.Clamp(yaw, -clampAngle, clampAngle);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(yaw, pitch, 0),
            Time.deltaTime * 50f);
    }

    void Move()
    {
        // ����ĳ����
        RaycastHit hit;
        Vector3 dirToTarget = (characterTransform.position + transform.rotation * offset) - (characterTransform.position + Vector3.up * 0.5f);

        if (Physics.Raycast(characterTransform.position + Vector3.up * 0.5f, dirToTarget.normalized, out hit, maxDistance))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }


        if (character.GetIsRunning())
        {
            finalPos = characterTransform.position + (runRot * offset.normalized * finalDistance);
        }
        else
        {
            // ĳ���� ��ġ + (���� ȸ���� * ������ ���� * ���� �Ÿ�)
            // ȸ�� ��� * ��Į�� -> ȸ���� ��
            finalPos = characterTransform.position + (transform.rotation * offset.normalized * finalDistance);
            runRot = transform.rotation;

        }
        // ī�޶� ��ġ ����
        transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * 500f);
    }

}
