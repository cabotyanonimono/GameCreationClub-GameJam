using System;
using Benjathemaker;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    private PlayerContoroller player_controller;

    [SerializeField] 
    private Transform next_goal;

    [SerializeField] 
    private GoalNavigation goal_navi;
    
    [SerializeField] 
    private ParticleSystem particle_system;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            particle_system.Play();
            player_controller.check_point = transform.position;
            goal_navi.goal = next_goal;
            Destroy(gameObject);
        }
    }
}
