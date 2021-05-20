using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _foodPrefab,_bonusPrefab;

    [SerializeField]
    private float _bonusSpawnTime = 10f;

    [SerializeField] [InspectorName("Game Setup")]
    private GameSetup _gs;
    
    private Vector2 _foodScale;
    private float _cellSize;
    private bool _isSpawnBonus = false;

    //private IEnumerator spawBonus;


    // Start is called before the first frame update
    void Start()
    {
        //spawBonus = SpawnBonusCorutine();
      
        _cellSize = _gs.CellSize;
        _foodScale = _gs.SnakeScale;
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        Snake.SetFoodSpawner += SetFoodSpawner;
        Snake.SpawnFood += SpawnFood;
        //Snake.StopBonusSpawn += StopBonusSpawn;
        Snake.PauseBonusSpawn += PauseBonusSpawn; 
    }

    private void OnDisable()
    {
        Snake.SetFoodSpawner -= SetFoodSpawner;
        Snake.SpawnFood -= SpawnFood;
        //Snake.StopBonusSpawn += StopBonusSpawn;
        Snake.PauseBonusSpawn -= PauseBonusSpawn;
    }

    private void SpawnFood()
    {
        Spawn(_foodPrefab, true);
    }

    private void StopBonusSpawn()
    {
        StopCoroutine(SpawnBonusCorutine());
    }

    private void PauseBonusSpawn()
    {
        if (_isSpawnBonus)
        {
            _isSpawnBonus = false;
            StopCoroutine(SpawnBonusCorutine());
            //spawBonus = SpawnBonusCorutine();
        }
        else
        {
            _isSpawnBonus = true;
            StartCoroutine(SpawnBonusCorutine());
        }
    }

    public void Spawn(GameObject objToSpawn,bool isFood = true)
    {
        int index;
        System.Random r = new System.Random();

        List<ArrayPosHandler> freeCellesPos = new List<ArrayPosHandler>();

        for (int i = 0; i < _gs.gameArray.GetLength(0); i++)
        {
            for (int j = 0; j < _gs.gameArray.GetLength(1); j++)
            {
                if (_gs.gameArray[i,j] == 0)
                {
                    freeCellesPos.Add(new ArrayPosHandler(i, j));
                }
            }
        } 

        try
        {
            index = r.Next(freeCellesPos.Count);
        }        
        catch
        {
            return;
        }
        
        Vector2 spawnPos = _gs.GetPosition(freeCellesPos[index]);

        GameObject food = Instantiate(objToSpawn, spawnPos, Quaternion.identity);
        food.transform.localScale = _foodScale;

        if (isFood)
        {
            _gs.ChangGameArray(freeCellesPos[index], 3);
        }
        else
        {
            _gs.ChangGameArray(freeCellesPos[index], 4);
            food.GetComponent<Bonus>().spawnPosInArray = freeCellesPos[index];
        }
    }

    private void SetFoodSpawner(bool isActive)
    {
        if (isActive)
        {
            _isSpawnBonus = true;
            SpawnFood();
            StartCoroutine(SpawnBonusCorutine());
        }
        else
        {
            _isSpawnBonus = false;
            StopCoroutine(SpawnBonusCorutine());
            //spawBonus = SpawnBonusCorutine();

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("FoodPrefab"))
                Destroy(obj);

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Bonus"))
                Destroy(obj);

            Bonus._isExists = false;
        }
    }

    IEnumerator SpawnBonusCorutine()
    {
        while (Snake.tailLength <= 150 && _isSpawnBonus)
        {
            yield return new WaitForSeconds(_bonusSpawnTime);
            if (!Bonus._isExists && _isSpawnBonus)
            {
                Spawn(_bonusPrefab, false);
                Bonus._isExists = true;
            }          
        }  
    }
}
