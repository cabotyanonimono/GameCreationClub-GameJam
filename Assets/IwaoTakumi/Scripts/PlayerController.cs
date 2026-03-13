using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class PlayerContoroller : MonoBehaviour
{
    [SerializeField] public Transform _player;

    [SerializeField] public Transform _camera;

    [SerializeField] public float power;

    [SerializeField] public float max_speed;

    [SerializeField] public float movement_threshold;

    [SerializeField] public float dragPower;

    [SerializeField] public float endMousePosY;

    [SerializeField] public float drag;

    [SerializeField] public bool isStop;

    [SerializeField] public Vector3 check_point;

    [SerializeField] public AudioSource audio_source;

    [FormerlySerializedAs("fall_count")] public int shot_count;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        check_point = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -10.0f)
        {
            transform.position = check_point;
        }
        
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        if (IsDragging())
        {
            dragPower -= mouseDelta.y;
            Mathf.Max(dragPower, 0.0f);
        }

        // �}�E�X������
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector3 direction = _camera.forward;

            if (dragPower > 0)
            {
                ++shot_count;
                audio_source.PlayOneShot(audio_source.clip);
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
            dragPower = Mathf.Min(dragPower, max_speed);
            return direction * dragPower * power;
        }

        return Vector3.zero;
    }

    public bool IsDragging()
    {
        return Mouse.current.leftButton.isPressed && rb.linearVelocity.magnitude <= movement_threshold;
    }
}