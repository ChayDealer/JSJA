using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuHandler : MonoBehaviour
{
    private Snake snake;

    public GameObject mainMenu, gameMenu, shopMenu, looseGameMenu, winGameMenu, lastChanceMenu;
    public GameObject shopButton;

    [SerializeField]
    private AudioClip _menuMusic, _gameMusic, _winMusic, _loseMusic, _buttonSound, _purchaseSound;

    private Text _lastChanceTextTip;
    public Text immunLeftText;

    [SerializeField]
    private SoundButton _sb;
    [SerializeField]
    private MusicButton _mb;

    private int _highScore = 0, _score = 0;
    private bool _isShopAwaileble = true;
    private float _timeoutValue = 2.0f, _timeoutReal;
    private string _noAdsText = "You Pay\r\nYou Play"
        ,_adsText = "Watch ads to continue";

    public GameObject scoreContainer;
    public AdsController adsController;
    public Text scoreText, highScoreText, tailLengthText;

    public static Text debugTextObj;

    public static bool isTipsEnable = true;

    public delegate void MusicChange(AudioClip song, float volume);
    public static event MusicChange SetMusic;

    public delegate void PlaySound(AudioClip song);
    public static event PlaySound playSound;

    private void OnEnable()
    {
        Snake.AddScore += AddScore;
        Snake.SetImunTimeText += SetImunTimeText;
    }

    private void OnDisable()
    {
        Snake.AddScore -= AddScore;
        Snake.SetImunTimeText -= SetImunTimeText;
    }

    private void Awake()
    {
        snake = GameObject.FindGameObjectWithTag("Player").GetComponent<Snake>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _timeoutReal = Time.time + _timeoutValue;
        
        mainMenu.SetActive(true);

        SetMusic?.Invoke(_menuMusic, 1f);

        _score = 0;
        scoreText.text = _score.ToString();
        tailLengthText.text = "0";
        immunLeftText.text = "0";

        _lastChanceTextTip = lastChanceMenu.transform.Find("To Continue").GetComponent<Text>();

        _highScore = PlayerPrefs.GetInt("HighScore");
        if ( _highScore != 0)
        {            
            highScoreText.text = _highScore.ToString();
            scoreContainer.SetActive(true);
        }
        //Debug.Log($"Happy = {PlayerPrefs.GetInt("happy")}");
        if (PlayerPrefs.HasKey("happy"))
        {
            if (PlayerPrefs.GetInt("happy") != 0)
            {
                HideShopButton();
            }
        }

        debugTextObj = transform.Find("Menu").Find("Debug Text Obj").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (mainMenu.activeSelf && !shopMenu.activeSelf)
        //{
        //    if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() 
        //        || Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
        //    {
        //        StartGame();
        //    }
        //}
        //else if (winGameMenu.activeSelf)
        //{
        //    if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()
        //        || Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
        //    {
        //        winGameMenu.SetActive(false);
        //        mainMenu.SetActive(true);
        //        SetMusic?.Invoke(_menuMusic, 1f);
        //    }
        //}

        foreach (Touch touch in Input.touches)
        {
            if (mainMenu.activeSelf && !shopMenu.activeSelf && Time.time > _timeoutReal)
            {
                if (touch.phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject(touch.fingerId)
                || touch.phase == TouchPhase.Ended && EventSystem.current.currentSelectedGameObject == null)
                {
                        StartGame(); 
                }
            }
            else if (winGameMenu.activeSelf)
            {
                if (touch.phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject()
                || touch.phase == TouchPhase.Ended && EventSystem.current.currentSelectedGameObject == null)
                {
                    winGameMenu.SetActive(false);
                    mainMenu.SetActive(true);
                }
            }          
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        ChangeGameStateByFocus(!focus);
    }

    private void OnApplicationPause(bool pause)
    {
        ChangeGameStateByFocus(pause);
    }

    private void ChangeGameStateByFocus(bool isPause)
    {
        if (isPause)
            snake.PauseGame();
        else
            snake.ContinueGame();
    }

    private void StartGame()
    {
        playSound?.Invoke(_buttonSound);
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        SetMusic?.Invoke(_gameMusic, 1f);
        adsController.ResetAdsState();

        if (!isTipsEnable)
            snake.ChangeGameState(true);
        else
            Snake.curGameState = Snake.GameState.TipPause;
    }

    private void SetHighScore()
    {
        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetInt("HighScore", _highScore);
            highScoreText.text = _highScore.ToString();
            scoreContainer.SetActive(true);
        }
    }

    //Collback function
    public void RestartGame()
    {
        playSound?.Invoke(_buttonSound);
        SetMusic?.Invoke(_gameMusic, 1f);
        ResetScore();
        snake.SetSpriteRendererActive(true);
        looseGameMenu.SetActive(false);
        gameMenu.SetActive(true);
        adsController.ResetAdsState();
        snake.ChangeGameState(true);
        Debug.Log("Game Restart");
    }

    public void WinGame()
    {
        SetMusic?.Invoke(_winMusic, 0.8f);
        gameMenu.SetActive(false);
        winGameMenu.SetActive(true);
        SetHighScore();
        Debug.Log("Game Win");
    }

    public void LooseGame()
    {
        ShowUsualAdd();
        SetMusic?.Invoke(_loseMusic, 1f);
        gameMenu.SetActive(false);
        looseGameMenu.SetActive(true);
        SetHighScore();
        Debug.Log("Your are Losser!");
    }

    public void ShowMenu()
    {
        looseGameMenu.SetActive(false);
        playSound?.Invoke(_buttonSound);
        snake.SetSpriteRendererActive(true);
        SetHighScore();
        ResetScore();
        _timeoutReal = Time.time + _timeoutValue;
        mainMenu.SetActive(true);
        SetMusic?.Invoke(_menuMusic, 1f);
        Debug.Log("ShowMenu");
    }

    public void ShowlastChanceMenu()
    {
        if (lastChanceMenu.activeSelf)
        {
            lastChanceMenu.SetActive(false);
            return;
        }
            
        if ((AdsController.CheckAddVideo(AdsController.rewVideo) 
            && AdsController.CheckAddVideo(AdsController.video)) 
            || adsController.RemoveAds)
        {            
            if (adsController.RemoveAds)
            {
                _lastChanceTextTip.text = _noAdsText;
                lastChanceMenu.SetActive(true);
            }
            else
            {
                _lastChanceTextTip.text = _adsText;
                lastChanceMenu.SetActive(true);
            }       
        }
        else
        {
            snake.ChangeGameState(false);
        }
    }

    public void ShowShop()
    {
        _timeoutReal = Time.time + _timeoutValue;
        playSound?.Invoke(_buttonSound);
        shopMenu.SetActive(!shopMenu.activeSelf);
        Debug.Log("ShowShop");
    }

    public void SetSoundCondition()
    {
        _timeoutReal = Time.time + _timeoutValue;
        playSound?.Invoke(_buttonSound);
        _sb.ChangeState();
        Debug.Log("SetSoundCondition");
    }

    public void SetMusicCondition()
    {
        _timeoutReal = Time.time + _timeoutValue;
        playSound?.Invoke(_buttonSound);
        _mb.ChangeState();
        Debug.Log("SetMusicCondition");
    }

    public void ShowRewAdd()
    {
        adsController.ShowRewardedVideo();
    }

    public void ShowUsualAdd()
    {
        adsController.ShowVideo();
    }

    public void AddScore(int value)
    {
        _score += value;
        scoreText.text = _score.ToString();
        tailLengthText.text = (Snake.tailLength + 1).ToString();
    }

    public void SetImunTimeText(string time)
    {
        immunLeftText.text = time;
    }

    public int GetScore()
    {
        return _score;
    }

    public void ResetScore()
    {
        _score = 0;
        scoreText.text = _score.ToString();
        tailLengthText.text = "0";
        immunLeftText.text = "0";
    }

    public void HideShopButton()
    {
        _isShopAwaileble = false;
        shopButton.SetActive(_isShopAwaileble);

        _timeoutReal = Time.time + _timeoutValue;
        shopMenu.SetActive(false);
    }
}
