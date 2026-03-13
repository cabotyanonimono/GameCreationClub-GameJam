// 製作者：エイト

using UnityEngine;

/// <summary>
/// プレイヤーの頭上に配置され、ゴールの方向を指し示すスクリーンスペースUI制御クラス。
/// カメラの向き（視線）を基準に角度を計算する修正版。
/// </summary>
public class WorldSpaceNavUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("プレイヤーのTransform（基準点）")]
    public Transform player;
    [Tooltip("目標地点（ゴール）のTransform")]
    public Transform goal;
    [Tooltip("メインカメラ（スクリーン座標変換および回転基準用）")]
    public Camera cam;

    [Header("Settings")]
    [Tooltip("プレイヤーの足元からどれくらい高い位置にUIを出すか")]
    public float heightOffset = 2.0f;
    [Tooltip("矢印の初期向きのズレを調整（0で上が正解ならそのまま）")] 
    public float rotationOffset = 0f; 

    private RectTransform rect;
    private CanvasGroup canvasGroup; // 表示・非表示の制御用

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // UIの透明度を操作するためにCanvasGroupがあれば取得
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // 必須参照のチェック
        if (!player || !goal || !cam || !rect) return;

        //  1. 表示位置の計算（プレイヤーの頭上）
        Vector3 worldPos = player.position + Vector3.up * heightOffset;

        //  2. ワールド座標をスクリーン座標に変換
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        //  3. カメラの背後にある場合の処理（回り込み防止） 
        if (screenPos.z > 0)
        {
            if (canvasGroup) canvasGroup.alpha = 1f;
            rect.position = screenPos;
        }
        else
        {
            if (canvasGroup) canvasGroup.alpha = 0f;
            return; // カメラの後ろにいるときは回転計算をスキップ
        }

        //  4. ゴール方向への回転計算
        // プレイヤーから見たゴールの水平方向ベクトル
        Vector3 dir = goal.position - player.position;
        dir.y = 0;

        // カメラの前方方向を水平ベクトルとして取得 
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0; // 水平方向のみで比較するため 

        // カメラの視線を基準に、ゴール方向への角度（-180～180）を算出 
        float angle = Vector3.SignedAngle
        (
            camForward,
            dir.normalized,
            Vector3.up
        );

        //  5. UIの回転に反映
        // Z軸を回転（UnityのUIは左回りが正なので、計算されたangleを反転して適用）
        rect.localRotation = Quaternion.Euler(0, 0, -angle + rotationOffset); 
    }
}