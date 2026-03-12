using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class NavigationArrow : MonoBehaviour
{
    [SerializeField] 
    private PlayerContoroller player_contoroller;

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
            Destroy(navigation_trail, 3.0f);
            navigation_trail = Instantiate(navigation_trail_prefab);
            restart_timer = restart_timer - restart_delay;
            navigation_trail.transform.position = player_contoroller.transform.position;
            navigation_trail.GetComponent<Rigidbody>().AddForce(player_contoroller.GetForce());
        }
    }
}