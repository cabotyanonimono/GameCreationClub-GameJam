// 製作者：エイト

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>?
/// ゴール判定クラス。
/// タグを使わず、特定のコライダーやリジットボディを直接参照して判定する確実なエディション。
/// </summary>?
public class GoalTrigger : MonoBehaviour
{
    [Header("判定設定（ドラッグ&ドロップで指定）")]
    [Tooltip("プレイヤーに付いている『Collider』をここにドラッグしてください")]
    [SerializeField] private Collider playerCollider;

    [Tooltip("プレイヤーに付いている『Rigidbody』をここにドラッグしてください（任意）")]
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("オプション")]
    [Tooltip("このオブジェクト（ゴール）自体のタグ。空文字にするとタグ判定をスキップします")]
    [SerializeField] private string goalTag = "";

    [Header("遷移設定")]
    [Tooltip("遷移先のシーン名")]
    [SerializeField] private string resultSceneName = "ResultScene";

    private bool isLoading = false;

    /// <summary>
    /// ゴール接触判定。
    /// 指定されたコライダー、またはリジットボディ本体が触れたかどうかを厳密にチェックします。
    /// </summary>
    /// <param name="other">接触してきたコライダー</param>
    private void OnTriggerEnter(Collider other)
    {
        // 接触があるたびに必ず出力（デバッグ用）
        string status = isLoading ? "<color=red>[遷移中]</color>" : "<color=cyan>[待機中]</color>";
        Debug.Log($"{status} <color=white>【接触検知】</color> 触れた: <b>{other.gameObject.name}</b>");

        // 必須設定のチェック
        if (playerCollider == null)
        {
            Debug.LogError("<color=red>【設定ミス】</color> Inspectorの 'Player Collider' 欄が空っぽです！");
            return;
        }

        // 判定ロジック：直接参照による比較
        // 1. 触れたコライダーそのものが指定されたものか
        // 2. もしくは、触れたものが持っているRigidbodyが指定されたものか
        bool isTargetCollider = (other == playerCollider);
        bool isTargetRigidbody = (playerRigidbody != null && other.attachedRigidbody == playerRigidbody);

        bool isCorrectTag = string.IsNullOrEmpty(goalTag) || this.gameObject.CompareTag(goalTag);

        if ((isTargetCollider || isTargetRigidbody) && isCorrectTag)
        {
            if (isLoading)
            {
                Debug.Log("<color=yellow>【判定】</color> 指定プレイヤーを検知しましたが、既に遷移中です。");
                return;
            }

            isLoading = true;
            Debug.Log("<b><size=18><color=green>【判定成功】指定されたプレイヤー本体を検知！ </color></size></b>");
            SceneManager.LoadScene(resultSceneName);
        }
        else
        {
            // 失敗ログ：なぜ反応しなかったのかを具体的に出す
            string reason = "指定されたコライダー/リジットボディではありません。";
            Debug.Log($"<color=gray>【判定スキップ】</color> {other.gameObject.name} : {reason}");
        }
    }
}