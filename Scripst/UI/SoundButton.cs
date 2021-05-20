using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField]
    private Sprite _soundOn, _soundOff;
    [SerializeField]
    private SoundsController _soundPlayer;

    private Image curImg;


    void Start()
    {
        curImg = gameObject.GetComponent<Image>();

        if (_soundPlayer.GetIsSoundActive() > 0)
            curImg.sprite = _soundOn;
        else
            curImg.sprite = _soundOff;
    }

    public void ChangeState()
    {
        if (curImg.sprite == _soundOn)
        {
            curImg.sprite = _soundOff;
            _soundPlayer.ChangeSoundState(false);
        }
        else
        {
            curImg.sprite = _soundOn;
            _soundPlayer.ChangeSoundState(true);
        }
    }
}
