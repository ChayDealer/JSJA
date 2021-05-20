using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{
    private AudioSource _as;

    private int _isSoundActive = 1;

    private void OnEnable()
    {
        MenuHandler.playSound += PlaySound;
    }

    private void OnDisable()
    {
        MenuHandler.playSound -= PlaySound;
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey("isSoundActive"))
        {
            _isSoundActive = PlayerPrefs.GetInt("isSoundActive");
        }
        else
            PlayerPrefs.SetInt("isSoundActive", _isSoundActive);
    }

    void Start()
    {
        _as = gameObject.GetComponent<AudioSource>();

        if (_isSoundActive > 0)
            ChangeSoundState(true);
        else
            ChangeSoundState(false);
    }

    public void PlaySound(AudioClip clip)
    {
        _as.clip = clip;
        _as.Play();
        
    }

    public void ChangeSoundState(bool value)
    {
        if (value)
        {
            AudioListener.volume = 1.0f;
            _isSoundActive = 1;
        }
        else
        {
            AudioListener.volume = 0.0f;
            _isSoundActive = 0;
        }
        PlayerPrefs.SetInt("isSoundActive", _isSoundActive);
        //Debug.Log(PlayerPrefs.GetInt("isSoundActive") + " isSoundActive");
    }

    public int GetIsSoundActive()
    {
        return _isSoundActive;
    }
}
