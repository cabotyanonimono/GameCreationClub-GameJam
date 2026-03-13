//製作者：エイト

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 10f;        // プレイヤーの移動速度
    public float jumpForce = 5f;         // ジャンプの強さ

    private Rigidbody rb;                // Rigidbodyを格納する変数
    private bool isGrounded;             // 地面に接地しているかの判定

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 自分についているRigidbodyを取得
    }

    void Update()
    {
        Move();                          // 移動処理を呼び出す
        Jump();                          // ジャンプ処理を呼び出す
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal"); // A/Dキーの入力取得
        float v = Input.GetAxis("Vertical");   // W/Sキーの入力取得

        Vector3 move = new Vector3(h, 0, v);   // 移動方向を作成

        rb.AddForce(move * moveSpeed);         // 力を加えて移動させる
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // スペースキー＆地面接地時のみ
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 上方向にジャンプ力を加える
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 接触しているオブジェクトがGroundタグなら
        {
            isGrounded = true;                        // 接地状態にする
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Groundから離れたら
        {
            isGrounded = false;                       // 非接地状態にする
        }
    }
}