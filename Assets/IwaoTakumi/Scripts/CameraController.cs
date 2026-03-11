using UnityEngine.InputSystem;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Transform _target;  // 注視点（プレイヤー）

    [SerializeField]
    private float HeightOffset = 2.0f;  // 注視点の高さ

    [SerializeField]
    private float distanceOffset = 10.0f;  // プレイヤーとの距離

    [SerializeField]
    private float Rotation_X = 0;

    [SerializeField]
    private float Rotation_Y = 0;

    [SerializeField]
    private float MouseSensitivity = 200.0f;    // マウス感度

    [SerializeField]
    private float CameraRadius = 0.5f;  // カメラの半径

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 左クリックでFPS視点
        bool isPressLeftButton = Mouse.current.leftButton.isPressed;

        // マウス入力
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * MouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * MouseSensitivity * Time.deltaTime;

        // 左クリック中は上下回転を固定
        if (!isPressLeftButton)
        {
            Rotation_X -= mouseY;   // 上下
            Rotation_X = Mathf.Clamp(Rotation_X, -30f, 60f);
        }

        Rotation_Y += mouseX;   // 左右

        Rotation_X = Mathf.Clamp(Rotation_X, -30f, 60f);

        // 回転
        Quaternion rotation = Quaternion.Euler(Rotation_X, Rotation_Y, 0);

        // カメラ位置
        Vector3 position = _target.position - rotation * new Vector3(0, 0, distanceOffset);

        // 注視点の高さを加算
        Vector3 targetPosition = _target.position;
        targetPosition.y += HeightOffset;

        RaycastHit hit;
        Vector3 dir = position - targetPosition;
        float dist = dir.magnitude;
        if (Physics.SphereCast(targetPosition, CameraRadius, dir.normalized, out hit, dist))
        {
            position = hit.point - dir.normalized * 0.1f;
        }

        _camera.transform.position = position;
        _camera.transform.LookAt(targetPosition);
    }
}
