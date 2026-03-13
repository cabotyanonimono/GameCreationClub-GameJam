using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle_system;

    [SerializeField] 
    private PlayerContoroller player_controller;
    
    [SerializeField]
    private float change_scene_delay;

    [SerializeField]
    private GameObject result_screen;
    
    private AudioSource audio_source;
    
    private float timer = 0.0f;
    private bool is_goal = false;
    
    private void Start()
    {
        ScoreManager.Instance.StartTimeMeasure();
        audio_source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("player"))
            return;
        
        is_goal = true;
        particle_system.Play();
        audio_source.PlayOneShot(audio_source.clip);
        ScoreManager.Instance.StopTimeMeasure();
        
        int base_score = 100000;
        ScoreManager.Instance.AddScore(Mathf.Max(base_score - player_controller.shot_count * 198 - (int)ScoreManager.Instance.GetCurrentElapseTime(), 0));
        result_screen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
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
