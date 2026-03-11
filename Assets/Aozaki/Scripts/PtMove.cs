using UnityEngine;

namespace Gimmicks
{
    /// <summary>
    /// Start/Endの「位置」を到達点として、Rigidbodyを移動させるギミック。
    /// 共通パラメータ（Mode/Speed/Wait/StartPhase）は PtGmkBase に揃えてある。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PtMove : PtGmkBase
    {
        [Header("Points (Move)")]
        [Tooltip("移動の開始点（位置のみ使用）。")]
        [SerializeField] private Transform start;

        [Tooltip("移動の到達点（位置のみ使用）。")]
        [SerializeField] private Transform end;

        protected override bool IsReady()
        {
            return start != null && end != null;
        }

        protected override float GetTotalAmount()
        {
            // Start-Endの距離（units）
            return Vector3.Distance(start.position, end.position);
        }

        protected override void ApplySnap(float t01)
        {
            // 初期化/Reset時：直接 Rb.position を書いてOK（見た目を合わせる目的）
            Vector3 p = Vector3.LerpUnclamped(start.position, end.position, t01);
            Rb.position = p;
        }

        protected override void ApplyTick(float t01)
        {
            // 物理押し出しを狙うので MovePosition を使用
            Vector3 p = Vector3.LerpUnclamped(start.position, end.position, t01);
            Rb.MovePosition(p);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (start == null || end == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start.position, end.position);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(start.position, 0.08f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(end.position, 0.08f);
        }
#endif
    }
}