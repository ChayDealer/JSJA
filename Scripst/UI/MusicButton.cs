using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField]
    private Sprite _musicOn, _musicOff;
    [SerializeField]
    private MusicController _musicPlayer;

    private Image curImg;

    
    void Start()
    {
        curImg = gameObject.GetComponent<Image>();
        if (_musicPlayer.GetIsMusicActive() > 0)
            curImg.sprite = _musicOn;
        else
            curImg.sprite = _musicOff;
    }

    public void ChangeState()
    {
        if (curImg.sprite == _musicOn)
        {
            curImg.sprite = _musicOff;
            _musicPlayer.ChangeMusicState(false);
        }
        else
        {
            curImg.sprite = _musicOn;
            _musicPlayer.ChangeMusicState(true);
        }
    }
}
