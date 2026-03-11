using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContoroller : MonoBehaviour
{
    [SerializeField]
    Transform _player;

    [SerializeField]
    Transform _camera;

    [SerializeField]
    float power;

    [SerializeField]
    float dragPower;

    [SerializeField]
    float endMousePosY;

    [SerializeField]
    float drag;

    [SerializeField]
    bool isStop;

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

        // ƒ}ƒEƒX—£‚µ‚½
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector3 direction = _camera.forward;

            if (dragPower > 0)
            {
                rb.AddForce(direction * dragPower * power);
            }
        }
    }
}
