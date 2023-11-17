using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip soundClip;
    void Start()
    {
        SoundManager.Instance.PlaySound(soundClip);
    }

}

    
