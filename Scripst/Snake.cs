using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private float _speed = 0.3f, _immunTimeMax = 20.0f, _colorChangeTime = 2.0f;
    private float _colorTimer = 0f;
    [SerializeField]
    private GameObject _tailPrefab, _tailContainer;
    [SerializeField, Range(0, 1)]
    private float _tailDestroyTime = 0.3f;

    public Sprite tailForrwardSprite;
    public Sprite tailTurnSprite;

    private float _cellSize, _realTailDestroyTime;
    private ArrayPosHandler _currPosInArray;

    private int _winTailLength;

    [SerializeField]
    private GameSetup _gs;
    private SpriteRenderer _headSR;
    public GameObject immunTimeGameObj;

    private Color _defaultColor, c1, c2;
    public Color[] immunColors;

    private List<Vector2> _dirList = new List<Vector2>();
    private List<Vector2> _inputs = new List<Vector2>();
    private List<ArrayPosHandler> _steps = new List<ArrayPosHandler>();//лист из 3 позиций (1-3) №0 следующая позиция №1 текущая позиция №2 прошлая
    private List<TailClass> _tail = new List<TailClass>();
    private bool _ateFood = false, _ateBonus = false, _isDirChanged = false;

    private float _nextMove, _noMoveTime, _immunTimeLeft, _immunTimeStart, _minMoveDelay;

    public static ushort tailLength = 0;

    public enum GameState { Starting, Game, Pause, Immun, Lose, Win, GameOver, TipPause }
    [HideInInspector]
    public static GameState curGameState = GameState.Starting;
    private GameState _prevGameState;
    private ushort _loseCount = 0;

    //private IEnumerator _move;

    public UnityEvent GameWin, GameLose, LastChance, RemoveTailEvent, MoveEvent, EatEvent, BonusEvent;

    public delegate void ChangeFoodSpawner(bool isActive);
    public static event ChangeFoodSpawner SetFoodSpawner;

    public delegate void SpawnFoodEvent();
    public static event SpawnFoodEvent SpawnFood;
    public static event SpawnFoodEvent PauseBonusSpawn;

    public delegate void ScoreChange(int value);
    public static event ScoreChange AddScore;

    public delegate void ImmunTimeChange(string value);
    public static event ImmunTimeChange SetImunTimeText;

    public delegate void MusicState(bool value);
    public static event MusicState ChangeMusicState;

    private void OnEnable()
    {
        SwipeHandler.OnSwipeEnd += DirectionInput;
    }

    private void OnDisable()
    {
        SwipeHandler.OnSwipeEnd -= DirectionInput;
    }

    void Awake()
    {
        _headSR = gameObject.GetComponent<SpriteRenderer>();
        if (_headSR == null)
            Debug.Log($"Can't find \"SpriteRenderer\" on {gameObject.name}.");
    }

    void Start()
    {
        immunTimeGameObj.SetActive(false);
        _defaultColor = _headSR.color;

        _cellSize = _gs.CellSize;
        transform.localScale = _gs.SnakeScale;
        _currPosInArray = _gs.StartSnakeArrayPos;
        transform.position = _gs.GetPosition(_currPosInArray);
        _gs.ChangGameArray(_currPosInArray, 1);

        _minMoveDelay = _speed * 0.8f;
        //_move = MoveCorutine();
        //curGameState = GameState.Game;

        for (int i = 0; i < 3; i++)
        {
            _steps.Insert(0, _currPosInArray);
        }

        if (!MenuHandler.isTipsEnable)
        {
            _dirList.Insert(0, Vector2.left);
            //_inputs.Insert(0, Vector2.left);
        }

        _winTailLength = _gs.WinTailLength;
    }

    private void Update()
    {
        if (curGameState == GameState.Game || curGameState == GameState.Immun)
        {
            if (curGameState == GameState.Immun)
            {
                _immunTimeLeft = _immunTimeMax - (Time.time - _immunTimeStart);
                SetImunTimeText?.Invoke(FormatTime(_immunTimeLeft));

                _headSR.color = Color.Lerp(c1, c2, _colorTimer / _colorChangeTime);
                _colorTimer += Time.deltaTime;
                ChangeTailColor(c1, c2);

                if (_colorTimer >= _colorChangeTime)
                {
                    c1 = c2;

                    if (_immunTimeLeft <= 2.01f)
                        c2 = _defaultColor;
                    else
                        do
                        {
                            c2 = immunColors[Random.Range(0, immunColors.Length)];
                        } while (c1 == c2);

                    _colorTimer -= _colorChangeTime;
                }

                if (_immunTimeLeft <= 0)
                {
                    curGameState = GameState.Game;
                    _immunTimeLeft = _immunTimeStart = 0;
                    immunTimeGameObj.SetActive(false);
                    _headSR.color = _defaultColor;
                    foreach (var item in _tail)
                    {
                        item.transform.GetComponent<SpriteRenderer>().color = _defaultColor;
                    }
                }
            }

            //if (_isDirChanged)
            //{
            //    Move();
            //    _nextMove = Time.time + _speed;
            //    _isDirChanged = false;
            //    //Debug.Log("On Swipe");
            //}

            if (Time.time > _noMoveTime)
            {

                if (_dirList.Count > 1 && _isDirChanged)
                {
                    //Debug.Log($"Moved by (_dirList.Count > 1): Count = {_dirList.Count}. Time = {Time.time}");
                    _nextMove = Time.time + _speed;
                    _noMoveTime = Time.time + _minMoveDelay;
                    _isDirChanged = false;
                    Move();
                }
                else if (Time.time > _nextMove)
                {
                    //Debug.Log($"Moved by (Time.time > _nextMove). Time = {Time.time}");
                    _nextMove = Time.time + _speed;
                    _noMoveTime = Time.time + _minMoveDelay;
                    Move();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    //ивент смены направления движения
    public void DirectionInput(Vector2 dir)
    {
        if (curGameState == GameState.Game || curGameState == GameState.Immun || curGameState == GameState.TipPause)
        {
            if (_dirList.Count > 0)
            {
                if (dir == Vector2.right && _dirList.Contains(Vector2.left))
                    return;
                else if (dir == Vector2.up && _dirList.Contains(Vector2.down))
                    return;
                else if (dir == Vector2.left && _dirList.Contains(Vector2.right))
                    return;
                else if (dir == Vector2.down && _dirList.Contains(Vector2.up))
                    return;
                else if (dir == _dirList.Last())
                    return;
            }

            _dirList.Insert(0, dir);

            //if (_inputs.Count > 0)
            //    _inputs.Insert(0, dir);
            //else
            //{
            //    _inputs.Insert(0, dir);
            //    _inputs.Insert(0, dir);
            //}

            if (_dirList.Count > 3)
            {
                _dirList.RemoveAt(_dirList.Count - 1);
            }
            //if (_inputs.Count > 3)
            //    _inputs.RemoveAt(_inputs.Count - 1);

            //Подвинуть змейку сразу после свайпа
            _isDirChanged = true;
        }
    }

    private void Move()
    {
        if (_dirList.Count == 0) return;

        ArrayPosHandler oldPosInArray = _currPosInArray,
            newPosInArray;


        if (_dirList.Count > 1)
        {
            newPosInArray = _currPosInArray + _dirList.Last();
            _dirList.RemoveAt(_dirList.Count - 1);
        }
        else
        {
            newPosInArray = _currPosInArray + _dirList.First();
        }

        ArrayPosHandler inputValue = newPosInArray - _currPosInArray;

        if (_inputs.Count > 0)
            _inputs.Insert(0, new Vector2(inputValue.x, inputValue.y));
        else
        {
            _inputs.Insert(0, new Vector2(inputValue.x, inputValue.y));
            _inputs.Insert(0, new Vector2(inputValue.x, inputValue.y));
        }


        if (ArrayPosHandler.IsLeftBorderCrossed(newPosInArray))
        {
            _currPosInArray = new ArrayPosHandler(_currPosInArray.x, _gs.gameArray.GetLength(1) - 1);
        }
        else if (ArrayPosHandler.IsRightBorderCrossed(newPosInArray, _gs.gameArray.GetLength(1) -1))
        {
            _currPosInArray = new ArrayPosHandler(_currPosInArray.x, 0);
        }
        else if (ArrayPosHandler.IsTopBorderCrossed(newPosInArray))
        {
            _currPosInArray = new ArrayPosHandler(_gs.gameArray.GetLength(0) - 1, _currPosInArray.y);
        }
        else if (ArrayPosHandler.IsBottomBorderCrossed(newPosInArray, _gs.gameArray.GetLength(0) - 1))
        {
            _currPosInArray = new ArrayPosHandler(0, _currPosInArray.y);
        }
        else
        {
            _currPosInArray = newPosInArray;
        }

        transform.position = _gs.GetPosition(_currPosInArray);

        MoveEvent?.Invoke();
        _steps.Insert(0, _currPosInArray);
        _gs.ChangGameArray(_currPosInArray, 1);

        if (_ateFood)
        {
            EatEvent?.Invoke();
            SpawnFood?.Invoke();
            _gs.ChangGameArray(oldPosInArray, 2);
            SpawnTail(oldPosInArray);
            _ateFood = false;
        }
        else if (_ateBonus)
        {
            BonusEvent?.Invoke();
            SpawnTail(oldPosInArray);
            _gs.ChangGameArray(oldPosInArray, 2);
            _ateBonus = false;
        }
        else if (_tail.Count > 0)
        {
            TailLastToFirst(oldPosInArray);
        }
        else
        {
            _gs.ChangGameArray(oldPosInArray, 0);
        }

        if (_steps.Count > 2)
            _steps.RemoveAt(_steps.Count - 1);

        if (_inputs.Count > 2)
            _inputs.RemoveAt(_inputs.Count - 1);
    }

    private void SpawnTail(ArrayPosHandler spawnPos)
    {
        GameObject t = Instantiate(_tailPrefab
            , _gs.positionsArray[spawnPos.x, spawnPos.y]
            , Quaternion.identity);

        t.transform.localScale = _gs.SnakeScale;
        t.transform.parent = _tailContainer.transform;

        SetTailCondition(t);

        if (curGameState == GameState.Immun)
        {
            t.transform.GetComponent<SpriteRenderer>().color
                = _tail.First().transform.GetComponent<SpriteRenderer>().color;
        }

        _tail.Insert(0, new TailClass(t.transform, spawnPos));

        tailLength = (ushort)_tail.Count;
        //Debug
        //if (tail.Count == 6)
        //{
        //    WinGame();
        //}

        if (_tail.Count >= _winTailLength)
        {
            WinGame(); 
        }
    }

    private void TailLastToFirst(ArrayPosHandler newPos)
    {
        ArrayPosHandler oldPos = _tail.Last().arrayPos;
        _gs.ChangGameArray(oldPos, 0);
        if (_gs.GetGameArrayValue(newPos) != 2)
        {
            _gs.ChangGameArray(newPos, 2);
        }

        if (curGameState == GameState.Immun)
        {
            //if (_tail.Last().transform.GetComponent<SpriteRenderer>().color
            //        != _tail.First().transform.GetComponent<SpriteRenderer>().color)
            if(tailLength % 2 == 0 )
            {
                _tail.Last().transform.GetComponent<SpriteRenderer>().color = _tail.First().transform.GetComponent<SpriteRenderer>().color;
            } 
        }

        _tail.Last().arrayPos = newPos;
        _tail.Last().transform.position = _gs.GetPosition(newPos);

        SetTailCondition(_tail.Last().transform.gameObject);
        
        _tail.Insert(0, _tail.Last());
        _tail.RemoveAt(_tail.Count - 1);
    }

    //поворачивает хвост согласно движениям игрока
    private void SetTailCondition(GameObject tail)
    {
        SpriteRenderer sr = tail.transform.GetComponent<SpriteRenderer>();

        if (_inputs[1] == _inputs[0])
        {
            if (sr.sprite != tailForrwardSprite)
                sr.sprite = tailForrwardSprite;

            if (_inputs[0].x == 0)
                tail.transform.localEulerAngles = new Vector3(0, 0, 90.0f);
            else
                tail.transform.localEulerAngles = new Vector3(0, 0, 0);
            return;
        }

        if (sr.sprite != tailTurnSprite)
           sr.sprite = tailTurnSprite;

        if (_inputs[0] == Vector2.down)
        {
            if (_inputs[1] == Vector2.right)
                tail.transform.localEulerAngles = new Vector3(0, 0, 180.0f);
            else
                tail.transform.localEulerAngles = new Vector3(0, 0, -90.0f);
        }
        else if (_inputs[0] == Vector2.up)
        {
            if (_inputs[1] == Vector2.right)
                tail.transform.localEulerAngles = new Vector3(0, 0, -270.0f);
            else
                tail.transform.localEulerAngles = new Vector3(0, 0, 0.0f);
        }
        else if (_inputs[0] == Vector2.right)
        {
            if (_inputs[1] == Vector2.down)
                tail.transform.localEulerAngles = new Vector3(0, 0, 0.0f);
            else
                tail.transform.localEulerAngles = new Vector3(0, 0, -90.0f);
        }
        else if (_inputs[0] == Vector2.left)
        {
            if (_inputs[1] == Vector2.down)
                tail.transform.localEulerAngles = new Vector3(0, 0, 90.0f);
            else
                tail.transform.localEulerAngles = new Vector3(0, 0, 180.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "FoodPrefab")
        {
            _ateFood = true;
            AddScore?.Invoke(col.gameObject.GetComponent<Food>().ScoreValue);
            Destroy(col.gameObject);
        }
        else if (col.tag == "Bonus")
        {
            _ateBonus = true;
            AddScore?.Invoke(col.gameObject.GetComponent<Bonus>().ScoreValue);
            Bonus._isExists = false;
            Destroy(col.gameObject);
        }
        else if (col.tag == "Tail")
        {
            if (curGameState != GameState.Immun)
            {
                LoseGame();
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:000}", seconds, milliseconds);
    }

    private void ChangeTailColor(Color color1 ,Color color2)
    { 

        for (int i = 0; i < tailLength; i += 2)
        {
            SpriteRenderer sr = _tail[i].transform.GetComponent<SpriteRenderer>();
            sr.color = Color.Lerp(color1, color2, _colorTimer / _colorChangeTime);
        }

        for (int i = 1; i < tailLength; i += 2)
        {
            SpriteRenderer sr = _tail[i].transform.GetComponent<SpriteRenderer>();
            sr.color = _defaultColor;
        }
    }

    public void SetSpriteRendererActive(bool value)
    {
        _headSR.enabled = value;
    }

    public void ChangeGameState(bool isActive)
    {
        if (isActive)
        {
            SetFoodSpawner?.Invoke(true);
            curGameState = GameState.Game;
            //StartCoroutine(_move);
        }
        else
        {
            curGameState = GameState.GameOver;
            //StopCoroutine(_move);
            SetFoodSpawner?.Invoke(false);
            ChangeMusicState?.Invoke(false);

            _realTailDestroyTime = (_tailDestroyTime * 4) / _tail.Count;
            StartCoroutine(RemoveTail());
        }  
    }

    public void ContinueGame()
    {
        //if (curGameState == GameState.Game || curGameState == GameState.Immun) return;
        if (curGameState == _prevGameState) return;

        if (_prevGameState == GameState.Game || _prevGameState == GameState.Immun)
        {
            //StartCoroutine(_move);
            PauseBonusSpawn?.Invoke();
            curGameState = _prevGameState;
        }
        else if (AdsController.AfterRevAddVideo)
        {
            AdsController.AfterRevAddVideo = false;

            //StartCoroutine(_move);
            PauseBonusSpawn?.Invoke();

            c1 = _defaultColor; c2 = immunColors[Random.Range(0, immunColors.Length)];
            _immunTimeStart = Time.time;
            _immunTimeLeft = _immunTimeMax - (Time.time - _immunTimeStart);
            SetImunTimeText?.Invoke(FormatTime(_immunTimeLeft));
            immunTimeGameObj.SetActive(true);

            curGameState = GameState.Immun;
        }
        else 
        {
            curGameState = _prevGameState;
        }

        Time.timeScale = 1.0f;
        ChangeMusicState?.Invoke(true);  
    }

    public void PauseGame()
    {
        if (curGameState == GameState.Pause) return;

        _prevGameState = curGameState;

        Time.timeScale = 0.0f;

        if (_prevGameState == GameState.Game || _prevGameState == GameState.Immun)
        {
            //StopCoroutine(_move);
            PauseBonusSpawn?.Invoke();
        }

        curGameState = GameState.Pause;
        ChangeMusicState?.Invoke(false);
    }

    private void LoseGame()
    {
        curGameState = GameState.Lose;
        //StopCoroutine(_move);
        _loseCount++;

        if (_loseCount >=2)
            ChangeGameState(false);
        else
        {
            PauseBonusSpawn?.Invoke();
            ChangeMusicState?.Invoke(false);

            LastChance?.Invoke();
        }       
    }

    private void WinGame()
    {
        curGameState = GameState.Win;
        AddScore?.Invoke(100000);
        ChangeGameState(false);  
    }

    private void ResetGame()
    {
        curGameState = GameState.Starting;
        _dirList.Clear();
        _dirList.Insert(0, Vector2.left);

        _loseCount = 0;
        tailLength = 0;

        _gs.ClearGameArray();

        _currPosInArray = _gs.StartSnakeArrayPos;
        transform.position = _gs.GetPosition(_currPosInArray);
        _gs.ChangGameArray(_currPosInArray, 1);

        _steps.Clear();

        for (int i = 0; i < 3; i++)
        {
            _steps.Insert(0, _currPosInArray);
        }
    }

    //IEnumerator MoveCorutine()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(_speed);
    //        Move();
    //        if (curGameState == GameState.Immun)
    //            _immunTime -= _speed;

    //        //Debug.Log(immunTime);
    //        if (_immunTime <= 0 && curGameState == GameState.Immun)
    //        {
    //            curGameState = GameState.Game;
    //        }
    //    }
    //}

    IEnumerator RemoveTail()
    {
        yield return new WaitForSeconds(1.0f);
        RemoveTailEvent?.Invoke();
        while (_tail.Count != 0)
        {
            Destroy(_tail.Last().transform.gameObject);
            _tail.Remove(_tail.Last());
            yield return new WaitForSeconds(_realTailDestroyTime);
        }
        if (curGameState == GameState.Win)
            GameWin?.Invoke();
        else
            GameLose?.Invoke();

        ResetGame();
    }
}
