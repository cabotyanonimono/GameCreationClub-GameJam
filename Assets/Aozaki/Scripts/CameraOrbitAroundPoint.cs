using UnityEngine;

public class CameraOrbitAroundPoint : MonoBehaviour
{
    [Header("回転させるカメラ（ステージ側カメラ）")]
    [SerializeField] private Transform cameraTransform;

    [Header("中心（OrbitPivot自身でもOK）")]
    [SerializeField] private Transform lookAtTarget;

    [Header("回転速度（度/秒）")]
    [SerializeField] private float degreesPerSecond = 10f;

    [Header("Y軸水平回転のみ")]
    [SerializeField] private bool worldUp = true;

    [Header("timeScale=0でも回すならON")]
    [SerializeField] private bool useUnscaledTime = true;

    private void Reset()
    {
        // Pivotに付ける想定
        lookAtTarget = transform;
        if (Camera.main) cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (!cameraTransform || !lookAtTarget) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // Pivot（このオブジェクト）をY軸回転
        transform.Rotate(0f, degreesPerSecond * dt, 0f, Space.World);

        // 常に中心を見る（水平回転方式なのでUpはワールド上方向固定が無難）
        if (worldUp)
            cameraTransform.LookAt(lookAtTarget.position, Vector3.up);
        else
            cameraTransform.LookAt(lookAtTarget.position);
    }
}