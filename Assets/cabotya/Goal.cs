using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle_system;

    private AudioSource audio_source;
    
    [SerializeField]
    private float change_scene_delay;
    
    private float timer = 0.0f;
    private bool is_goal = false;
    
    private void Start()
    {
        audio_source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("player"))
            return;
        
        is_goal = true;
        particle_system.Play();
        audio_source.PlayOneShot(audio_source.clip);
        
        Destroy(GetComponent<MeshRenderer>());
    }

    private void Update()
    {
        if(!is_goal)
            return;
        
        timer += Time.deltaTime;

        if (timer >= change_scene_delay)
            SceneManager.LoadScene("ResultScene");
    }
}
