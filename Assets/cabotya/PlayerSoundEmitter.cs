using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSoundEmitter : MonoBehaviour
{
    [SerializeField]
    private AudioSource audio_source;
    private Rigidbody rigid_body;

    private void Start()
    {
        rigid_body = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        //TODO : 速度で音をいじるようにしたほうがいいかも
        audio_source.PlayOneShot(audio_source.clip);
    }
}
