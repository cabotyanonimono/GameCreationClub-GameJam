using System;
using UnityEngine;

public class WaterArea : MonoBehaviour
{
    public float drag;
    public GameObject water_image;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Camera>())
            water_image.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        var component = other.GetComponent<Rigidbody>();
        if (component != null)
            component.GetComponent<Rigidbody>().AddForce(-component.linearVelocity * drag, ForceMode.Acceleration);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Camera>())
            water_image.SetActive(false);
    }
}