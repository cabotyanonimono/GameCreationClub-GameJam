using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    private PlayerContoroller player_controller;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            player_controller.check_point = transform.position;
            Destroy(gameObject);
        }
    }
}
