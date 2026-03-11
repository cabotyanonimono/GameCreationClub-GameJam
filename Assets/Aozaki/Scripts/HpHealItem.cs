using UnityEngine;

public class HpHealItem : MonoBehaviour
{
    [Header("Heal")]
    [Tooltip("回復量（Player.hp に加算されます）")]
    [Min(1)]
    [SerializeField] private int healAmount = 3;

    [Tooltip(
        "最大HPの上限をアイテム側で適用する場合の上限値。\n" +
        "0 以下なら上限クランプを行いません（加算のみ）。"
    )]
    [Min(0)]
    [SerializeField] private int clampMaxHp = 0;

    [Tooltip("取得後にアイテムを消す")]
    [SerializeField] private bool destroyOnPickup = true;

    [Tooltip("消えるまでの遅延秒数（SE/VFXを鳴らす場合に使う）")]
    [Min(0f)]
    [SerializeField] private float destroyDelay = 0f;

    [Header("Detect")]
    [Tooltip(
        "true の場合、接触したColliderの親階層も含めて Player を探します。\n" +
        "プレイヤーが子Collider構成のときに有効。"
    )]
    [SerializeField] private bool searchInParent = true;

    private bool _picked;

    private void OnTriggerEnter(Collider other)
    {
        if (_picked) return;

        Player player = searchInParent
            ? other.GetComponentInParent<Player>()
            : other.GetComponent<Player>();

        if (player == null) return;

       
        int newHp = player.hp + healAmount;

        // 上限が必要ならアイテム側でクランプ
        if (clampMaxHp > 0)
            newHp = Mathf.Min(newHp, clampMaxHp);

        player.hp = newHp;

        _picked = true;

        if (destroyOnPickup)
            Destroy(gameObject, destroyDelay);
    }
}