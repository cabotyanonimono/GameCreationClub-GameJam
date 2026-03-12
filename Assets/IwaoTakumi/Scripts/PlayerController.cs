using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContoroller : MonoBehaviour
{
    [SerializeField]
    public Transform _player;

    [SerializeField]
    public Transform _camera;

    [SerializeField]
    public float power;

    [SerializeField]
    public float dragPower;

    [SerializeField]
    public float endMousePosY;

    [SerializeField]
    public float drag;

    [SerializeField]
    public bool isStop;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb  = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        if(Mouse.current.leftButton.isPressed)
        {
            dragPower -= mouseDelta.y;
        }

        // �}�E�X������
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector3 direction = _camera.forward;

            if (dragPower > 0)
            {
                rb.AddForce(direction * dragPower * power);
            }

            dragPower = 0;
        }
    }

    public Vector3 GetForce()
    {
        Vector3 direction = _camera.forward;

        if (dragPower > 0)
        {
            return direction * dragPower * power;
        }

        return Vector3.zero;
    }
}
