// 製作者：エイト

using UnityEngine;

/// <summary>
/// プレイヤーから見たゴールの位置をレーダー（UI）に表示するクラス。
/// プレイヤーの回転に合わせてアイコンの位置と向きを更新します。
/// </summary>
public class CompassRadar : MonoBehaviour
{
    [Header("References (ドラッグ&ドロップ)")]
    [Tooltip("プレイヤーのTransformを指定してください")]
    public Transform player;
    [Tooltip("ゴールのTransformを指定してください")]
    public Transform goal;

    [Header("UI References")]
    [Tooltip("レーダーの背景（中心点）となるRectTransform")]
    public RectTransform radarCenter;
    [Tooltip("レーダー上に表示するゴールアイコンのRectTransform")]
    public RectTransform targetIcon;

    [Header("Radar Settings")]
    [Tooltip("どれくらいの距離までをレーダーに収めるか")]
    public float radarRange = 100f;
    [Tooltip("UI上のレーダーの半径（ピクセル単位）")]
    public float radarRadius = 100f;

    /// <summary>
    /// 毎フレーム、ゴールへの相対位置を計算してUIを更新します。
    /// </summary>
    void Update()
    {
        // 参照が一つでも欠けていたら何もしない
        if (player == null || goal == null || targetIcon == null) return;

        // 1. プレイヤーから見たゴールの世界座標での相対位置
        Vector3 relativePosition = goal.position - player.position;

        // 2. プレイヤーの向き（回転）を考慮して相対位置を変換
        // Quaternion.Inverse を使うことで、手動のSin/Cos計算より正確で高速に処理します
        Vector3 localRelativePos = Quaternion.Inverse(player.rotation) * relativePosition;

        // 3. レーダーの範囲内に収めるために正規化 (-1.0 ～ 1.0)
        float normalizedX = Mathf.Clamp(localRelativePos.x / radarRange, -1f, 1f);
        float normalizedZ = Mathf.Clamp(localRelativePos.z / radarRange, -1f, 1f);

        // 4. UIの座標に変換 (正規化値 × UI上の半径)
        Vector2 uiPosition = new Vector2
        (
            normalizedX * radarRadius,
            normalizedZ * radarRadius
        );

        targetIcon.anchoredPosition = uiPosition;

        // 5. アイコン自体の向きを調整
        // プレイヤーがゴールを向いている時、アイコンが上を向くように回転させます
        float angle = Mathf.Atan2(localRelativePos.x, localRelativePos.z) * Mathf.Rad2Deg;
        targetIcon.localRotation = Quaternion.Euler(0, 0, -angle);

        // デバッグ用：距離が近すぎる時のログ表示（任意）
        if (relativePosition.magnitude < 5f)
        {
            // ゴール直前のみログを出すなどの応用が可能です
        }
    }
}