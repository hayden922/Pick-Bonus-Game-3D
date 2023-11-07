using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    float lastEffect;

    [SerializeField] private AudioSource effectSource, musicSource;


    void Awake() 
    
    {
        if(Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip audioClip) 
    {

        
        if(effectSource.clip == audioClip && effectSource.isPlaying)
        {
            effectSource.Stop();
        }
        else if (lastEffect > (Time.time - 0.1f))

        {
            Debug.Log("stopped");
            return;
            

        } 
        else
        {
            lastEffect = Time.time;
            effectSource.PlayOneShot(audioClip);
        }
        
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
    
    
    
    
    
}
