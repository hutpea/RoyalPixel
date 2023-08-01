using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundParticle : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    private void OnEnable()
    {
        GameController.Instance.musicManager.PlayShotPiano(source);
    }
    private void OnDisable()
    {
        source.clip = null;
        source.Pause();
    }
}
