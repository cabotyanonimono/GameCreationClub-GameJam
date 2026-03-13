using UnityEngine.InputSystem;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Transform _target; // �����_�i�v���C���[�j

    [SerializeField] private float HeightOffset = 2.0f; // �����_�̍���

    [SerializeField] private float distance;

    [SerializeField] private float LerpSpeed;

    [SerializeField] private float Rotation_X = 0;

    [SerializeField] private float Rotation_Y = 0;

    [SerializeField] private float MouseSensitivity = 200.0f; // �}�E�X���x

    [SerializeField] private float CameraRadius = 0.5f; // �J�����̔��a

    [SerializeField] private PlayerContoroller player_controller;
    
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ���N���b�N��FPS���_
        bool isPressLeftButton = Mouse.current.leftButton.isPressed;

        // �}�E�X����
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * MouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * MouseSensitivity * Time.deltaTime;

        // ���N���b�N���͏㉺��]���Œ�
        if (!player_controller.is_lock || Keyboard.current.spaceKey.isPressed)
        {
            Rotation_X -= mouseY; // �㉺
            Rotation_X = Mathf.Clamp(Rotation_X, -50f, 80f);
            Rotation_Y += mouseX; // ���E
        }
        
        Quaternion rotation = Quaternion.Euler(Rotation_X, Rotation_Y, 0);

        Vector3 position = _target.position - rotation * new Vector3(0, 0, distance);

        // �����_�̍��������Z
        Vector3 targetPosition = _target.position;
        targetPosition.y += HeightOffset;

        RaycastHit hit;
        Vector3 dir = position - targetPosition;
        float dist = dir.magnitude;
        if (Physics.SphereCast(targetPosition, CameraRadius, dir.normalized, out hit, dist,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            position = hit.point - dir.normalized * 0.2f;
        }
        
        _camera.transform.position = position;
        _camera.transform.LookAt(targetPosition);
    }
}