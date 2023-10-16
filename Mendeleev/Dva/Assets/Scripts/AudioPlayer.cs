using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Toggle _volumeToggle;

    void Start()
    {
       if(PlayerPrefs.GetInt("volumeToggle") == 1)
        {
            _audio.Play();
        }
       else
        {
            _audio.Stop();
        }
    }

    public void SetVolume()
    {
        if(_volumeToggle == false)
        {
            _audio.Stop();
        }
        else
        {
            _audio.Play();
        }
    }

}
