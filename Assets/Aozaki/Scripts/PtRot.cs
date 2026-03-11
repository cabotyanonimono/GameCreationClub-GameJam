using UnityEngine;

namespace Gimmicks
{
    /// <summary>
    ///
    /// 要件：
    /// - 回転軸（World/Local + X/Y/Z）
    /// - 回転方向（+ / -）
    /// - 角速度（deg/sec）
    /// - 必要なら角度範囲で往復（PingPong）や片道停止（OneWay）
    /// - 物理押し出しのため Rigidbody.MoveRotation を FixedUpdate で使用
    ///
    /// 注意：
    /// - Rigidbody Constraints で回転がFreezeされている軸は回らない。
    /// - 物理的に押したいなら Collider は Trigger にしない。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PtRot : RbGmkBase
    {
        public enum Mode
        {
            [Tooltip("指定軸で回し続ける。")]
            Spin,

            [Tooltip("MinAngle↔MaxAngle を往復する。")]
            PingPong,

            [Tooltip("MinAngle→MaxAngle へ進み、到達したら停止する。")]
            OneWay,

            [Tooltip("MinAngle→MaxAngle→MinAngle… を周回する（位相ループ）。")]
            Loop
        }

        public enum AxisSpace
        {
            [Tooltip("世界座標の軸で回転（例：常に世界Y軸）。")]
            World,

            [Tooltip("ローカル座標の軸で回転（回転により軸も回る）。")]
            Local
        }

        public enum Axis
        {
            [Tooltip("X軸")]
            X,
            [Tooltip("Y軸")]
            Y,
            [Tooltip("Z軸")]
            Z
        }

        public enum Dir
        {
            [Tooltip("正方向（+）")]
            Positive = 1,
            [Tooltip("負方向（-）")]
            Negative = -1
        }

        [Header("Common (Rot)")]
        [Tooltip("回転モード。Spinは回し続け、PingPong/OneWay/Loopは角度範囲を使います。")]
        [SerializeField] private Mode mode = Mode.Spin;

        [Tooltip("角速度（deg/sec）。")]
        [Min(0f)]
        [SerializeField] private float spd = 90f;

        [Tooltip(
            "開始位相(0-1)。\n" +
            "PingPong/OneWay/Loop のときだけ有効。\n" +
            "0=MinAngle, 1=MaxAngle, 0.5=中間"
        )]
        [Range(0f, 1f)]
        [SerializeField] private float t0 = 0f;

        [Tooltip(
            "端に到達したときの待機秒数。\n" +
            "PingPong時：端で止まってから反転。\n" +
            "OneWay時：到達後停止（waitは実質不要）。"
        )]
        [Min(0f)]
        [SerializeField] private float wait = 0f;

        [Header("Axis")]
        [Tooltip("回転軸を世界基準にするか、ローカル基準にするか。")]
        [SerializeField] private AxisSpace axisSpace = AxisSpace.World;

        [Tooltip("回転軸（X/Y/Z）。")]
        [SerializeField] private Axis axis = Axis.Y;

        [Tooltip("回転方向（+ / -）。")]
        [SerializeField] private Dir direction = Dir.Positive;

        [Header("Angle Range (for PingPong/OneWay/Loop)")]
        [Tooltip(
            "角度範囲の最小値（deg）。\n" +
            "PingPong/OneWay/Loop のときに使用します。\n" +
            "例：-45"
        )]
        [SerializeField] private float minAngle = -45f;

        [Tooltip(
            "角度範囲の最大値（deg）。\n" +
            "PingPong/OneWay/Loop のときに使用します。\n" +
            "例：+45"
        )]
        [SerializeField] private float maxAngle = 45f;

        [Tooltip(
            "trueの場合、現在のTransform回転を基準（0度）として角度範囲を適用します。\n" +
            "falseの場合、Awake時点のRigidbody回転を基準にします（安定）。"
        )]
        [SerializeField] private bool useCurrentAsBase = false;

        // 内部状態
        private Quaternion _baseRot;
        private float _curAngle; // deg（範囲モード用）
        private int _pingDir = 1;
        private float _w;

        protected override void Awake()
        {
            base.Awake();

            _baseRot = useCurrentAsBase ? transform.rotation : Rb.rotation;

            // 初期角度（範囲モード用）
            _curAngle = Mathf.Lerp(minAngle, maxAngle, Mathf.Clamp01(t0));
            _pingDir = 1;
            _w = 0f;

            // 初期姿勢へスナップ
            if (UsesRange())
                SnapRange();
        }

        protected override void OnReset()
        {
            _baseRot = useCurrentAsBase ? transform.rotation : Rb.rotation;

            _curAngle = Mathf.Lerp(minAngle, maxAngle, Mathf.Clamp01(t0));
            _pingDir = 1;
            _w = 0f;

            if (UsesRange())
                SnapRange();
        }

        protected override void Tick(float dt)
        {
            if (spd <= 0f) return;

            if (_w > 0f)
            {
                _w -= dt;
                return;
            }

            float sign = (int)direction;

            // 1フレームで進む角度
            float dA = spd * dt * sign;

            if (mode == Mode.Spin)
            {
                // 回し続ける（角度を積む必要がないので、現在回転へ増分適用）
                Quaternion delta = Quaternion.AngleAxis(dA, GetAxisVector());
                Rb.MoveRotation(delta * Rb.rotation);
                return;
            }

            // 角度範囲モード
            switch (mode)
            {
                case Mode.PingPong:
                    _curAngle += spd * dt * _pingDir * sign;

                    if (_curAngle >= maxAngle)
                    {
                        _curAngle = maxAngle;
                        _pingDir = -1;
                        BeginWait();
                    }
                    else if (_curAngle <= minAngle)
                    {
                        _curAngle = minAngle;
                        _pingDir = 1;
                        BeginWait();
                    }
                    break;

                case Mode.OneWay:
                    _curAngle = Mathf.MoveTowards(_curAngle, maxAngle, Mathf.Abs(spd * dt));
                    if (Mathf.Abs(_curAngle - maxAngle) <= 0.001f)
                        Stop();
                    break;

                case Mode.Loop:
                    _curAngle += dA;

                    // min..max を周回
                    float len = maxAngle - minAngle;
                    if (Mathf.Abs(len) <= 0.0001f) return;

                    // 正規化して周回させる
                    float t = (_curAngle - minAngle) / len;
                    t = t - Mathf.Floor(t); // 0..1
                    _curAngle = Mathf.Lerp(minAngle, maxAngle, t);
                    break;
            }

            ApplyRange();
        }

        private bool UsesRange()
            => mode == Mode.PingPong || mode == Mode.OneWay || mode == Mode.Loop;

        private void BeginWait()
        {
            if (wait <= 0f) return;
            _w = wait;
        }

        private void SnapRange()
        {
            // 初期化/Reset時：直接Rb.rotationでOK
            Quaternion r = _baseRot * Quaternion.AngleAxis(_curAngle, GetAxisVector());
            Rb.rotation = r;
        }

        private void ApplyRange()
        {
            // 物理押し出しのため MoveRotation を使用
            Quaternion r = _baseRot * Quaternion.AngleAxis(_curAngle, GetAxisVector());
            Rb.MoveRotation(r);
        }

        private Vector3 GetAxisVector()
        {
            // World軸かLocal軸かで、回転軸ベクトルを決める
            Vector3 a = axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.up
            };

            if (axisSpace == AxisSpace.World)
                return a; // 世界軸

            // Local軸：現在のオブジェクトの向きに追従
            // transform ではなく Rigidbody の回転を基準にした方が物理的に一貫する
            return (Rb.rotation * a).normalized;
        }
    }
}