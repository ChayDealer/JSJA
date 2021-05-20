using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _as;

    private int _isMusicActive = 1;

    private void OnEnable()
    {
        MenuHandler.SetMusic += PlayMusic;
        Snake.ChangeMusicState += ChangeMusicStateNotRemember;
    }

    private void OnDisable()
    {
        MenuHandler.SetMusic -= PlayMusic;
        Snake.ChangeMusicState -= ChangeMusicStateNotRemember;
    }
    private void Awake()
    {
        if (PlayerPrefs.HasKey("isMusicActive"))
        {
            _isMusicActive = PlayerPrefs.GetInt("isMusicActive");   
        }
        else
            PlayerPrefs.SetInt("isMusicActive", _isMusicActive);

        _as = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        if (_isMusicActive > 0)
            ChangeMusicState(true);
        else
            ChangeMusicState(false);

    }

    public void PlayMusic(AudioClip song, float volume)
    {
        _as.clip = song;
        if (_isMusicActive > 0)
        {
            _as.Stop();
            _as.volume = volume;
            _as.Play();
        }
    }

    public void ChangeMusicStateNotRemember(bool state)
    {
        if (_isMusicActive > 0)
        {
            if (state)
                _as.Play();
            else
                _as.Pause();
        }
    }

    public void ChangeMusicState(bool value)
    {
        if (value)
        {
            _as.Play();
            _isMusicActive = 1;
        }   
        else
        {
            _as.Pause();
            _isMusicActive = 0;
        }
        PlayerPrefs.SetInt("isMusicActive", _isMusicActive);
  
    }
    
    public int GetIsMusicActive()
    {
        return _isMusicActive;
    }
}
