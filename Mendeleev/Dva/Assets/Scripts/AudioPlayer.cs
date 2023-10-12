using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Toggle _volumeToggle;

    void Start()
    {
       if(PlayerPrefs.GetInt("volumeToggle") == 1)
        {
            _audio.mute = false;
            _audio.volume = PlayerPrefs.GetFloat("volumeSlider");
        }
       else
        {
            _audio.mute = true;
        }
    }

    public void SetVolume()
    {
        if(_volumeSlider == false)
        {
            _audio.mute = true;
        }
        else
        {
            _audio.mute = false;
            _audio.volume = PlayerPrefs.GetFloat("volumeSlider");
        }
    }

}
