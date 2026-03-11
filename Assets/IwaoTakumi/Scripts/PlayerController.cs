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
    float startMousePosY;

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
        Vector2 mouseDelta = Mouse.current.position.ReadValue();

        // マウス押した
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startMousePosY = mouseDelta.y;
        }

        // マウス離した
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            endMousePosY = mouseDelta.y;

            drag = endMousePosY - startMousePosY;

            Vector3 direction = _camera.forward;

            rb.AddForce(direction * drag * power);
        }
    }
}
