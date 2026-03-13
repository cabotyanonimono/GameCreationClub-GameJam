using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class NavigationArrow : MonoBehaviour
{
    [SerializeField] private PlayerContoroller player_contoroller;

    [FormerlySerializedAs("navigation_trail")] [FormerlySerializedAs("gameObject")] [SerializeField]
    private GameObject navigation_trail_prefab;

    private GameObject navigation_trail;

    public float restart_timer;
    public float restart_delay;

    private void Update()
    {
        restart_timer += Time.deltaTime;
        if (restart_timer >= restart_delay && player_contoroller.IsDragging())
        {
            restart_timer = 0;

            Destroy(navigation_trail, 3.0f);

            navigation_trail = Instantiate(navigation_trail_prefab);
            navigation_trail.transform.position = player_contoroller.transform.position;
            navigation_trail.GetComponent<Rigidbody>().AddForce(player_contoroller.GetForce());

            var normalized_color = (player_contoroller.dragPower / player_contoroller.max_speed);
            float hue = (1 - normalized_color) * 0.33f;
            Color vibrant_color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            navigation_trail.GetComponent<TrailRenderer>().startColor = vibrant_color;
            navigation_trail.GetComponent<TrailRenderer>().endColor = vibrant_color;
        }
    }
}