using UnityEngine;

public class HpHealItem : MonoBehaviour
{
    [Header("Heal")]
    [Tooltip("回復量（Player.hp に加算）")]
    [Min(1)]
    [SerializeField] private int healAmount = 3;

    [Tooltip(
        "最大HPの上限（アイテム側でクランプ）。\n" +
        "0以下なら上限クランプしない（加算のみ）。"
    )]
    [Min(0)]
    [SerializeField] private int clampMaxHp = 0;

    [Header("Detect")]
    [Tooltip(
        "true: 接触したColliderの親階層から Player を探す（子Collider構成に強い）。\n" +
        "false: 接触Collider自身からのみ探す。"
    )]
    [SerializeField] private bool searchInParent = true;

    [Header("SFX (SE)")]
    [Tooltip("取得時に鳴らすSE。未指定なら鳴らしません。")]
    [SerializeField] private AudioClip seClip;

    [Tooltip("SEの音量（0-1）。")]
    [Range(0f, 1f)]
    [SerializeField] private float seVolume = 1f;

    [Tooltip(
        "SEを鳴らす位置。\n" +
        "true: アイテム位置\n" +
        "false: プレイヤー位置（回収した場所）"
    )]
    [SerializeField] private bool playSeAtItemPosition = false;

    [Header("VFX")]
    [Tooltip(
        "取得時に出すVFXのPrefab（ParticleSystem等）。未指定なら出しません。\n" +
        "※Prefabには自己再生/自己破棄（ParticleSystemのStopActionなど）を設定すると便利です。"
    )]
    [SerializeField] private GameObject vfxPrefab;

    [Tooltip(
        "VFXのスケール倍率。\n" +
        "1=等倍, 2=2倍。\n" +
        "PrefabのTransform.localScaleに対して倍率を掛けます。"
    )]
    [Min(0f)]
    [SerializeField] private float vfxScale = 1f;

    [Tooltip(
        "VFXを出す位置。\n" +
        "true: アイテム位置\n" +
        "false: プレイヤー位置"
    )]
    [SerializeField] private bool spawnVfxAtItemPosition = false;

    [Tooltip(
        "VFXを自動で破棄する秒数。\n" +
        "0以下なら破棄しません（Prefab側で自己破棄する想定）。"
    )]
    [Min(0f)]
    [SerializeField] private float vfxAutoDestroySeconds = 2f;

    [Header("Pickup")]
    [Tooltip("取得後にアイテムを消す")]
    [SerializeField] private bool destroyOnPickup = true;

    [Tooltip(
        "アイテムが消えるまでの遅延秒数。\n" +
        "SE/VFXだけ先に出して、見た目を残したい場合に使います。"
    )]
    [Min(0f)]
    [SerializeField] private float destroyDelay = 0f;

    [Tooltip(
        "true の場合、取得後に見た目/当たり判定を即無効化します（多重取得防止がより確実）。\n" +
        "DestroyDelay を使う場合に推奨。"
    )]
    [SerializeField] private bool disableVisualAndColliderOnPickup = true;

    private bool _picked;

    private void OnTriggerEnter(Collider other)
    {
        if (_picked) return;

        Player player = searchInParent
            ? other.GetComponentInParent<Player>()
            : other.GetComponent<Player>();

        if (player == null) return;

        _picked = true;

        // --- Heal（Playerコードをいじらないので public hp を直接加算） ---
        int newHp = player.hp + healAmount;
        if (clampMaxHp > 0)
            newHp = Mathf.Min(newHp, clampMaxHp);
        player.hp = newHp;

        // SE/VFXの発生位置を決める
        Vector3 playerPos = player.transform.position;
        Vector3 itemPos = transform.position;

        Vector3 sePos = playSeAtItemPosition ? itemPos : playerPos;
        Vector3 vfxPos = spawnVfxAtItemPosition ? itemPos : playerPos;

        // --- SE ---
        PlaySe(sePos);

        // --- VFX ---
        SpawnVfx(vfxPos);

        // --- 取得後処理（多重取得防止 & 消滅） ---
        if (disableVisualAndColliderOnPickup)
            DisableVisualAndCollider();

        if (destroyOnPickup)
            Destroy(gameObject, destroyDelay);
    }

    private void PlaySe(Vector3 pos)
    {
        if (seClip == null) return;
        if (seVolume <= 0f) return;

        // ワンショット再生（AudioSource不要）
        // ※3Dサウンドにしたい場合は、クリップの設定/Audio設定で調整してください
        AudioSource.PlayClipAtPoint(seClip, pos, seVolume);
    }

    private void SpawnVfx(Vector3 pos)
    {
        if (vfxPrefab == null) return;

        GameObject vfx = Instantiate(vfxPrefab, pos, Quaternion.identity);

        // Prefabの元スケールを保持しつつ倍率適用
        vfx.transform.localScale = vfx.transform.localScale * vfxScale;

        // 必要なら一定時間後に破棄
        if (vfxAutoDestroySeconds > 0f)
            Destroy(vfx, vfxAutoDestroySeconds);
    }

    private void DisableVisualAndCollider()
    {
        // 当たり判定を無効化（多重取得防止）
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 見た目を無効化（MeshRenderer等）
        // 子もまとめて消したいなら GetComponentsInChildren にする
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
            r.enabled = false;

        // 光など
        var lights = GetComponentsInChildren<Light>(true);
        foreach (var l in lights)
            l.enabled = false;
    }
}