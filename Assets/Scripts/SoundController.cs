using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private AudioClip[] availableMusic;
    [SerializeField] private MasterController masterController;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = availableMusic[0];
        PlayMusic();
    }

    public void SetCurrentMusicClip()
    {
        if(!masterController.practiceMode){
            int currentMusicKey = masterController.currentLevelKey;
            musicSource.clip = availableMusic[currentMusicKey];
        } else {
            int currentMusicKey = masterController.currentLevelKey + 1;
            musicSource.clip = availableMusic[currentMusicKey];
        }
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }
    
    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySound(AudioClip sound, float volume = 1.0f)
    {
        sfxSource.PlayOneShot(sound, volume);
    }
    
    public void AdjustMusicVolume()
    {
        musicSource.volume = musicSlider.value;
    }

    public void AdjustSfxVolume()
    {
        sfxSource.volume = sfxSlider.value;
    }
}
