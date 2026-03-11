using UnityEngine;

namespace Gimmicks
{
    /// <summary>
    /// Rigidbodyで動かすギミックの基底。
    ///
    /// ・物理押し出しを成立させるため、派生側は MovePosition / MoveRotation を使う前提。
    /// ・物理ステップに同期するため FixedUpdate で更新する。
    ///
    /// 推奨：
    /// ・Rigidbody: Is Kinematic = true（経路通りに安定して動かすため）
    /// ・Collider: Is Trigger = false（押し出ししたいので通常Collider）
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class RbGmkBase : MonoBehaviour
    {
        public enum OnAwake
        {
            /// <summary>シーン開始と同時に再生。</summary>
            Play,
            /// <summary>シーン開始時は停止（外部からPlayで開始）。</summary>
            Stop
        }

        [Header("Base")]
        [Tooltip("シーン開始時に自動で再生するかどうか。")]
        [SerializeField] private OnAwake onAwake = OnAwake.Play;

        [Tooltip(
            "true: Time.timeScale の影響を受けない時間で動作（ポーズ中も進む）。\n" +
            "物理ギミックは通常 false 推奨（物理との整合が取りやすい）。"
        )]
        [SerializeField] private bool unscaled = false;

        [Tooltip(
            "true の場合、Rigidbody が Kinematic でないと警告します。\n" +
            "ギミックを意図した経路で動かすなら Kinematic 推奨。"
        )]
        [SerializeField] private bool warnNonKinematic = true;

        /// <summary>再生中かどうか。</summary>
        public bool Playing { get; private set; }

        /// <summary>制御対象のRigidbody。</summary>
        protected Rigidbody Rb { get; private set; }

        // Reset用：初期姿勢（Rigidbody基準）
        private Vector3 _p0;
        private Quaternion _r0;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody>();

            if (warnNonKinematic && !Rb.isKinematic)
                Debug.LogWarning($"{name}: Kinematic Rigidbody is recommended for path-driven gimmicks.", this);

            _p0 = Rb.position;
            _r0 = Rb.rotation;

            Playing = (onAwake == OnAwake.Play);
        }

        protected virtual void FixedUpdate()
        {
            if (!Playing) return;

            float dt = unscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            Tick(dt);
        }

        /// <summary>
        /// 派生側の更新処理（FixedUpdate毎）。
        /// 物理ギミックなので、MovePosition/MoveRotationはここで行う。
        /// </summary>
        protected abstract void Tick(float dt);

        [ContextMenu("Play")]
        public void Play() => Playing = true;

        [ContextMenu("Stop")]
        public void Stop() => Playing = false;

        [ContextMenu("Reset")]
        public void ResetGmk()
        {
            Playing = false;

            // 衝突後の残り速度を消しておく（意図しないズレの防止）
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;

            // 初期姿勢へ戻す
            Rb.position = _p0;
            Rb.rotation = _r0;

            OnReset();
        }

        /// <summary>派生クラスが内部状態（位相t、待機など）を初期化するためのフック。</summary>
        protected virtual void OnReset() { }
    }
}