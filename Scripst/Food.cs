using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private FoodSpawner _spawner;
    private SpriteRenderer _sr;

    [SerializeField]
    private int _scoreValue = 50;

    public int ScoreValue { get => _scoreValue;}

    private void Awake()
    {
        _spawner = GameObject.FindGameObjectWithTag("FoodSpawner").GetComponent<FoodSpawner>();
        _sr = gameObject.GetComponent<SpriteRenderer>();
        if (_spawner == null)
            Debug.LogError("Can't find the FoodSpawner");
        if (_sr == null)
            Debug.LogError("Can't find the SpriteRenderer on " + gameObject.name + ".");
    }

    private void Start()
    {
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Tail" || col.tag == "FoodPrefab" || col.tag == "Bonus")
        {
            Debug.Log("Food Respawn");
            //Очень захватывающий баг))
            //_spawner.Spawn(this.gameObject,false);
            //Destroy(this.gameObject);
        }
    }
}
