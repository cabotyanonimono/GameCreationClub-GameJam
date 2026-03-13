using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSoundEmitter : MonoBehaviour
{
    [SerializeField]
    private AudioSource audio_source;

    private void OnCollisionEnter(Collision other)
    {
        audio_source.PlayOneShot(audio_source.clip);
    }
}
