using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioInstance;
    public Sound[] musicsounds, sfxsounds;
    public AudioSource musicsource, sfxsource;

    private void Awake()
    {
        if (audioInstance == null)
        {
            audioInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name, float? volume=null)
    {
        Sound s = Array.Find(musicsounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("No Sound Found");
        }
        else
        {
            if (volume != null)
            {
                musicsource.volume = volume.Value;
            }
            
            if(musicsource.clip != null && musicsource.clip != s.clip || musicsource.clip == null)
            {
                musicsource.clip = s.clip;
                musicsource.Play();
            }
        }
    }

    public void PlaySFX(string name, Vector3? position = null, float? volume = null, float? pitch = null)
    {
        Sound s = Array.Find(sfxsounds, x => x.name == name);
        if (s == null)
        {
            //Debug.Log("No Sound Found");
        }
        else
        {
            //Debug.Log(" Sound Found" + s.name);
            if(s.clip==null)
            {
                Debug.Log("No Clip Found");
                return;
            }
            if (position.HasValue)
            {
                AudioSource.PlayClipAtPoint(s.clip, position.Value, sfxsource.volume);
            }
            else
            {
                sfxsource.PlayOneShot(s.clip);
            }
            if (pitch.HasValue)
            {
                sfxsource.pitch = pitch.Value;
            }
            else
            {
                sfxsource.pitch = 1f;
            }
            if (volume.HasValue)
            {
                sfxsource.volume = volume.Value;
            }
            else
            {
                //sfxsource.volume = 1f;
            }


        }
    }

    public void SetMusicVolume(Slider slider)
    {
        
        musicsource.volume = slider.value;
        
    }
    public void SetSFXVolume(Slider slider)
    {
        sfxsource.volume = slider.value;
    }
    public void MuteMusic()
    {
        AudioManager.audioInstance.PlaySFX("UIButton", volume:1, pitch:UnityEngine.Random.Range(0.8f, 1.2f));
        musicsource.mute = !musicsource.mute;
        //instigator.transform.GetChild(0).gameObject.SetActive(!instigator.transform.GetChild(0).gameObject.activeInHierarchy);
        //instigator.transform.GetChild(1).gameObject.SetActive(!instigator.transform.GetChild(1).gameObject.activeInHierarchy);
    }

    public void MuteSFX()
    {
        AudioManager.audioInstance.PlaySFX("UIButton", volume:1, pitch:UnityEngine.Random.Range(0.8f, 1.2f));
        sfxsource.mute = !sfxsource.mute;
        //instigator.transform.GetChild(0).gameObject.SetActive(!instigator.transform.GetChild(0).gameObject.activeInHierarchy);
        //instigator.transform.GetChild(1).gameObject.SetActive(!instigator.transform.GetChild(1).gameObject.activeInHierarchy);
    }

}