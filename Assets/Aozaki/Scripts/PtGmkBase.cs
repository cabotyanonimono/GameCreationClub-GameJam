using UnityEngine;

namespace Gimmicks
{
    /// <summary>
    /// 「Start/End の2点（到達点）」を使うギミックの共通ベース。
    ///
    /// 目的：
    /// - Move/Rot を別コンポーネントに保ちつつ、
    ///   Inspector上の共通項（Mode/Speed/Wait/StartPhase）を完全に揃える。
    ///
    /// ここで行うこと：
    /// - 位相 t（0..1）の進行計算（OneWay/PingPong/Loop）
    /// - 端での待機処理
    ///
    /// 派生側が行うこと：
    /// - Start/Endの参照（Transform）
    /// - 全体量（移動距離 or 回転角度）を返す
    /// - 現在の t を適用（MovePosition / MoveRotation）
    /// </summary>
    public abstract class PtGmkBase : RbGmkBase
    {
        public enum Mode
        {
            [Tooltip("Start→Endへ進み、Endに着いたら停止。")]
            OneWay,

            [Tooltip("Start↔Endを往復し続ける。")]
            PingPong,

            [Tooltip("Start→Endに着いたらStartへ戻って続行（位相をループ）。")]
            Loop
        }

        [Header("Common (Pt)")]
        [Tooltip("到達点ギミックの動作モード。")]
        [SerializeField] private Mode mode = Mode.PingPong;

        [Tooltip(
            "速度。\n" +
            "Moveの場合：units/sec\n" +
            "Rotの場合：deg/sec\n" +
            "※どちらも「全体量」に対して位相(t)を進めるために使用されます。"
        )]
        [Min(0f)]
        [SerializeField] private float spd = 2f;

        [Tooltip(
            "開始位相(0-1)。\n" +
            "0 = Start\n" +
            "1 = End\n" +
            "0.5 = 中間\n" +
            "シーン開始時/Reset時にこの位相へスナップします。"
        )]
        [Range(0f, 1f)]
        [SerializeField] private float t0 = 0f;

        [Tooltip(
            "端（StartまたはEnd）に到達した際の待機時間（秒）。\n" +
            "PingPong時：端で止まってから反転します。\n" +
            "Loop時：端での待機は通常発生しません（ループするため）。\n" +
            "OneWay時：End到達後は停止するため、待機は実質不要です。"
        )]
        [Min(0f)]
        [SerializeField] private float wait = 0f;

        /// <summary>現在位相(0..1)。</summary>
        protected float T01 => _t;

        private float _t;      // 0..1
        private int _dir;      // +1/-1 (PingPong用)
        private float _wTimer; // wait timer

        protected override void Awake()
        {
            base.Awake();

            _t = Mathf.Clamp01(t0);
            _dir = 1;
            _wTimer = 0f;

            // 派生側に「位相を適用」させる（開始時に見た目を合わせる）
            ApplySnap(_t);
        }

        protected override void OnReset()
        {
            _t = Mathf.Clamp01(t0);
            _dir = 1;
            _wTimer = 0f;

            ApplySnap(_t);
        }

        protected override void Tick(float dt)
        {
            // 参照点が揃っていない等、派生側の条件未達なら動かさない
            if (!IsReady()) return;

            // 速度ゼロなら停止と同義（Play状態のまま）
            if (spd <= 0f) return;

            // 端で待機中
            if (_wTimer > 0f)
            {
                _wTimer -= dt;
                return;
            }

            // Moveなら距離、Rotなら角度、のように「全体量」を派生側からもらう
            float total = GetTotalAmount();

            // 全体量がゼロ（Start=End等）なら進めない
            if (total <= 0.0001f) return;

            // 速度(amount/sec) → 位相(0..1)/sec へ変換
            // total 分進むと t が 1 進む
            float dt01 = (spd / total) * dt;

            switch (mode)
            {
                case Mode.OneWay:
                    _t = Mathf.Clamp01(_t + dt01);
                    break;

                case Mode.PingPong:
                    _t += dt01 * _dir;

                    if (_t >= 1f)
                    {
                        _t = 1f;
                        _dir = -1;
                        BeginWait();
                    }
                    else if (_t <= 0f)
                    {
                        _t = 0f;
                        _dir = 1;
                        BeginWait();
                    }
                    break;

                case Mode.Loop:
                    _t += dt01;
                    if (_t > 1f) _t -= 1f;
                    break;
            }

            // 派生側が「位相に応じたMovePosition/MoveRotation」を実施
            ApplyTick(_t);
        }

        private void BeginWait()
        {
            if (wait <= 0f) return;
            _wTimer = wait;
        }

        /// <summary>
        /// 派生側：必要な参照が揃っているか（start/endがある等）。
        /// </summary>
        protected abstract bool IsReady();

        /// <summary>
        /// 派生側：Start-Endの「全体量」を返す。
        /// Move: 距離（units）
        /// Rot : 角度（deg）
        /// </summary>
        protected abstract float GetTotalAmount();

        /// <summary>
        /// 派生側：初期化/Reset時にスナップ適用（Rb.position / Rb.rotation を直接書いてOK）。
        /// </summary>
        protected abstract void ApplySnap(float t01);

        /// <summary>
        /// 派生側：FixedUpdate内の適用（Rb.MovePosition / Rb.MoveRotation を使う）。
        /// </summary>
        protected abstract void ApplyTick(float t01);
    }
}