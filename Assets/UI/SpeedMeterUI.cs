//　製作者：エイト

using UnityEngine;

public class SpeedMeterUI : MonoBehaviour
{
    public Rigidbody playerRb;          // プレイヤーボールのRigidbodyを参照する変数
    public RectTransform needle;        // UIの針を回転させるためのRectTransform

    public float maxSpeed = 20f;        // 最大速度（メーターのMAX）

    public float minAngle = -120f;      // メーターの最小角度（左端）
    public float maxAngle = 120f;       // メーターの最大角度（右端）

    void Update()
    {
        float speed = playerRb.linearVelocity.magnitude; // 現在の速度を取得（Unity6用）//CHANGE

        float t = Mathf.Clamp01(speed / maxSpeed);       // 現在速度を0～1の割合に変換

        float angle = Mathf.Lerp(minAngle, maxAngle, t); // 割合に応じて針の角度を決定

        needle.localRotation = Quaternion.Euler(0, 0, -angle); // Z軸方向に針を回転させる
    }
}