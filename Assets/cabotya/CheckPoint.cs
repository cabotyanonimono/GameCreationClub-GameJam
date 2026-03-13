using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    private PlayerContoroller player_controller;

    [SerializeField] 
    private ParticleSystem particle_system;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            particle_system.Play();
            player_controller.check_point = transform.position;
            Destroy(gameObject);
        }
    }
}
