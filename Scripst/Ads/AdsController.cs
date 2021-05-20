using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour, IUnityAdsListener
{
    private string gameId;
    public static readonly string video = "video", rewVideo = "rewardedVideo";

    private bool _wasRewAdd = false;
    private ushort _tailLength;
    private bool _removeAds = false;

    private static bool _afterRevAddVideo;

    public UnityEvent continueGame, endGame, pauseGame;

    public static bool AfterRevAddVideo { get => _afterRevAddVideo; set => _afterRevAddVideo = value; }
    public bool RemoveAds { get => _removeAds;}

    void Start()
    {
        Advertisement.AddListener(this);

        if (PlayerPrefs.HasKey("happy"))
        {
            if (PlayerPrefs.GetInt("happy") != 0)
            {
                _removeAds = true;
            }
            else
                _removeAds = false;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            gameId = "3853957";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameId = "3853956";
        }
        else
        {
            gameId = "3853957";
        }
        Advertisement.Initialize(gameId);

        if (Advertisement.isInitialized)
        {
            Debug.Log("Оно ЖИВОЕ!!!");
        } else
        {
            Debug.Log("Оно мертво!!!");
        }
    }

    public void ShowVideo()
    {
        if (!RemoveAds)
        {
            Snake.GameState state = Snake.curGameState;
            if (CheckAddVideo(video) && !_wasRewAdd && Snake.tailLength >= 20)
            {
                Advertisement.Show(video);
            }
            else
            {
                Debug.Log("Все плохо!!!!!");
            }
        }
    }

    public void ShowRewardedVideo()
    {
        if (!RemoveAds)
        {
            if (CheckAddVideo(rewVideo))
            {
                Advertisement.Show(rewVideo);
            }
            else
            {
                Debug.Log("Все плохо с _rewVideo!!!!!");
                endGame?.Invoke();
            }
        }
        else
        {
            _wasRewAdd = true;
            _afterRevAddVideo = true;
            continueGame?.Invoke();
            Debug.Log("After Purchase!");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if ((placementId == rewVideo) && showResult == ShowResult.Finished)
        {
            _wasRewAdd = true;
            _afterRevAddVideo = true;
            continueGame?.Invoke();
            Debug.Log("Finish Reward Add");
        }
        else if ((placementId == rewVideo) && showResult == ShowResult.Skipped)
        {
            endGame?.Invoke();
            Debug.Log("Skip Reward Add");
        }
        else if ((placementId == rewVideo) && showResult == ShowResult.Failed)
        {
            endGame?.Invoke();
            Debug.Log("Fail Reward Add");
        }


        if (placementId == video && showResult == ShowResult.Finished)
        {
            continueGame?.Invoke();
            Debug.Log("Finish Not Reward Add");
        }
        else if (placementId == video && showResult == ShowResult.Skipped)
        {
            continueGame?.Invoke();
            Debug.Log("Skip Not Reward Add");
        }
        else if (placementId == video && showResult == ShowResult.Failed)
        {
            continueGame?.Invoke();
            Debug.Log("Fail Not Reward Add");
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        pauseGame?.Invoke();
        Debug.Log("VideoStart");
    }

    public void OnUnityAdsReady(string placementId)
    {

    }

    private void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }

    public void ResetAdsState()
    {
        _wasRewAdd = false;
    }

    public static bool CheckAddVideo(string videoId)
    {
       return Advertisement.IsReady(videoId);
    }

    public void SetRemoveAllAds()
    {
        _removeAds = true;
    }

    //public bool SetRemoveAds { set => _removeAds = value; }
}
