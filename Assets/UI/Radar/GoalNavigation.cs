// 製作者：エイト

using UnityEngine;

/// <summary>
/// プレイヤーの頭上に配置され、ゴールの方向（高さを含む）を常に指し示す3Dオブジェクト用制御クラス。
/// アニメーションスクリプトとの併用を考慮した修正版。
/// </summary>
namespace Benjathemaker
{
    public class GoalNavigation : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("追従対象となるプレイヤーのTransform")]
        public Transform player;
        [Tooltip("指し示す目標地点のTransform")]
        public Transform goal;

        [Header("Settings")]
        [Tooltip("プレイヤーの足元から配置位置までの高さ")]
        public float height = 2.0f;

        private SimpleGemsAnim anim;

        void Start()
        {
            // 同じオブジェクトにアタッチされているSimpleGemsAnimを取得
            anim = GetComponent<SimpleGemsAnim>();
        }

        void LateUpdate()
        {
            // 必須参照のチェック
            if (player == null || goal == null)
            {
                return;
            }

            //----------------------------------
            // ① プレイヤーの頭上に固定
            //----------------------------------
            // SimpleGemsAnimから浮遊分の値をもらって高さに加算する 
            float verticalOffset = (anim != null) ? anim.GetFloatOffset() : 0f;
            Vector3 headPos = player.position + Vector3.up * (height + verticalOffset);
            transform.position = headPos;

            //----------------------------------
            // ② ゴールの方向を取得
            //----------------------------------
            // 現在のナビの位置から、ゴールへの直接的な向きを計算 
            Vector3 direction = goal.position - transform.position; 

            //----------------------------------
            // ③ 回転させる
            //----------------------------------
            // 方向ベクトルがゼロ（ゴールと重なっている状態）でなければ回転を更新
            if (direction != Vector3.zero)
            {
                // directionが高さを含んでいるため、そのままLookRotationに渡す 
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up); 
            }
        }
    }
}