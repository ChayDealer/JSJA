using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [SerializeField]
    private float _lifeTime = 4.0f, _addLifeTime = 0.3f;

    private float _lifeTimeLeft, _dieTime, _enableTime = 0.5f, _disableTime = 0.1f;
    [SerializeField]
    private int _scoreValue = 100;

    private SpriteRenderer _sr;
    private FoodSpawner _spawner;
    private GameSetup _gs;

    [HideInInspector]
    public static bool _isExists = false;

    public ArrayPosHandler spawnPosInArray = new ArrayPosHandler();

    public int ScoreValue { get => _scoreValue;}

    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        _gs = GameObject.FindGameObjectWithTag("SetupObj").GetComponent<GameSetup>();
        if (_gs == null)
            Debug.Log("Can't find the Setup Obj");

        _spawner = GameObject.FindGameObjectWithTag("FoodSpawner").GetComponent<FoodSpawner>();
        if (_spawner == null)
            Debug.Log("Can't find the FoodSpawner");
    }

    private void Start()
    {
        _isExists = true;

        ushort tailLength = Snake.tailLength;

        _lifeTime = Random.Range(_lifeTime - 1, _lifeTime + 1);
        if (tailLength > 20)
            _lifeTime = _lifeTime + (tailLength - 20) * _addLifeTime;


        _lifeTimeLeft = _lifeTime;
        _dieTime = Time.time + _lifeTime;
        _enableTime = _lifeTime / 10.0f;
        _disableTime = _enableTime / 5.0f;

        StartCoroutine(Die(_lifeTime));

        StartCoroutine(AnimationRoutine());
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Tail" || col.tag == "FoodPrefab" || col.tag == "Bonus")
        {
            Debug.Log("Food Respawn");
            _spawner.Spawn(this.gameObject, false);
            Destroy(this.gameObject);
        }
    }

    IEnumerator AnimationRoutine()
    {
        while (gameObject)
        {
            _lifeTimeLeft = _dieTime - (Time.time);
            _enableTime = _lifeTimeLeft / 10;

            if (_enableTime <= 0.1f)
            {
                _disableTime = _enableTime;
            }
            else
            {
                _disableTime = _enableTime / 7.0f;
            }

            _sr.enabled = true;
            yield return new WaitForSeconds(_enableTime);
            _sr.enabled = false;
            yield return new WaitForSeconds(_disableTime);
        } 
    }

    IEnumerator Die(float time)
    {
        yield return new WaitForSeconds(time);
        _gs.ChangGameArray(spawnPosInArray, 0);
        _isExists = false;
        Destroy(this.gameObject);
    }
}
