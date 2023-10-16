using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dva
{
    public class AudioGameController : MonoBehaviour
    {
        [SerializeField] Toggle _volumeToggle;
        private FeaturesManager _featuresManger;

        void Start()
        {
            _featuresManger = FindObjectOfType<FeaturesManager>();

            if(PlayerPrefs.GetInt("volumeToggle") == 0)
            {
                _volumeToggle.isOn = false;
            }
            else
            {
                _volumeToggle.isOn = true;
            }
        }

        public void AudioPlay(bool toPlay, AudioSource audio)
        {

            if (PlayerPrefs.GetInt("volumeToggle") == 0)
            {
                return;
            }
            else
            {
           
                if (toPlay)
                {
                    audio.mute = false;
                    audio.Play();
                }
                else
                {
                    audio.Stop();
                    audio.mute = true;
                }
            }
               
        }

        public void SetVolumeToggle()
        {
            PlayerPrefs.SetInt("volumeToggle", (_volumeToggle.isOn ? 1 : 0));
        }
    }
}