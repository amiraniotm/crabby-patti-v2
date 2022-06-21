using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private AudioClip[] availableMusic;
    [SerializeField] private LevelDisplay levelDisplay;
    
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
        int currentMusicKey = levelDisplay.currentLevelKey + 1;
        musicSource.clip = availableMusic[currentMusicKey];
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
