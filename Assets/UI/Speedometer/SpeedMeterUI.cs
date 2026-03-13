// 製製作：エイト

using UnityEngine;
using TMPro;

/// <summary> 
/// プレイヤーの速度を取得し、アナログメーターの針を右回り（時計回り）に制御するクラス
/// UnityのZ軸回転特性（負の方向＝右回り）を利用して、最短距離移動（ワープ）を防止した実装。
/// </summary> 
public class SpeedMeterUI : MonoBehaviour
{
    [Header("参照設定")]
    public Rigidbody playerRb;              // 速度を計算する対象のRigidbody。
    public RectTransform needle;            // 回転させる針のUI（RectTransform）。
    public TextMeshProUGUI speedText;       // 現在の速度を表示するUIテキスト。

    [Header("メーター角度設定")]
    public float maxSpeedKmh = 180f;        // メーターの最大目盛りとなる速度（km/h）。
    [Tooltip("0km/hの時のZ軸角度。")]
    public float minAngle = 125f;           // 速度が0の時の開始角度。
    [Tooltip("最高速度時のZ軸角度。右回りにしたい場合、minAngleより小さい数値を設定します。")]
    public float maxAngle = -30f;           // 速度が最大（maxSpeedKmh）の時の到達角度。 
    public bool ignoreYSpeed = true;        // ジャンプ中などの垂直方向の速度を計算に含めない設定。

    [Header("強制回転の設定")]
    [Tooltip("針が目標角度を追いかけるスピード（度/秒）。")]
    public float rotateSpeed = 300f;        // 1秒間に針が最大で何度動くかの設定。

    private float currentAngleValue;        // 現在の針の論理的な角度を保持する内部変数。
    private float angleStep;                // 1km/h増えるごとに変化させるべき角度の比率。

    /// <summary>
    /// 初期設定：開始時の角度をセットし、速度1km/hあたりの回転角を計算する。
    /// </summary>
    void Start()
    {
        // ゲーム開始時の針の角度をminAngle（0km/h）に初期化する。 
        currentAngleValue = minAngle;
        // 初期状態の角度をUIのTransformに反映させる。 
        needle.localEulerAngles = new Vector3(0, 0, currentAngleValue);

        // メーターがカバーする全角度の幅を最大速度で割り、1km/hあたりの変化量を算出する。 
        // 常に一定方向へ動かすため、絶対値（Mathf.Abs）で増分を計算する。 
        angleStep = Mathf.Abs(maxAngle - minAngle) / maxSpeedKmh; 
    }

    /// <summary>
    /// 毎フレームの更新：現在の速度に基づき、針を目標角度へ向かって一定速度で回転させる。
    /// </summary>
    void Update()
    {
        // Rigidbodyや針の参照が設定されていない場合は、エラー回避のため処理をスキップする。 
        if (!playerRb || !needle) return;

        // Rigidbodyから現在の世界空間での速度ベクトルを取得。 
        Vector3 vel = playerRb.linearVelocity;
        // 水平方向のみの速度を測る設定の場合、Y成分（上下移動）を0にする。 
        if (ignoreYSpeed) vel.y = 0;
        // Unityの単位（m/s）に3.6を掛けて、時速（km/h）に変換する。 
        float speedKmh = vel.magnitude * 3.6f;

        // 【右回り強制ロジック】 
        // 開始角度(minAngle)から、現在の速度に応じた回転量を「減算」する。 
        // これにより、速度が上がるほどZ軸の数値が減少し、UIは必ず右方向（時計回り）へ回転する。 
        float targetAngle = minAngle - (speedKmh * angleStep); 

        // Mathf.MoveTowardsを使用し、現在の角度から目標角度まで一定の速度（rotateSpeed）で近づける。 
        // これは最短距離（360度跨ぎ）のワープを無視し、数値の大小のみに従って移動する。 
        currentAngleValue = Mathf.MoveTowards(currentAngleValue, targetAngle, rotateSpeed * Time.deltaTime);

        // 計算によって求められた論理角度を、実際の針のローカル回転（Z軸）に適用する。 
        needle.localEulerAngles = new Vector3(0, 0, currentAngleValue);

        // スピード表示用テキストが存在する場合、現在の時速を整数でUIに表示する。 
        if (speedText)
        {
            speedText.text = $"{Mathf.RoundToInt(speedKmh)} ";
        }
    }
}