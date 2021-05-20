using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [SerializeField]
    private Transform Info_Elements, TopDot_Line, BotDot_Line;

    [SerializeField]
    private Vector2 gridSizeInCells = new Vector2(11, 19);
    private Vector2 _screenSizeInUnits, gridCellSize;

    private int _winTailLength;
    private float _cellSize;
    private Vector2 _snakeScale;
    private ArrayPosHandler _startSnakeArrayPos;

    public Vector2 SnakeScale { get => _snakeScale; }
    public Vector2 ScreenSizeInUnits { get => _screenSizeInUnits; }
    public float CellSize { get => _cellSize;  }
    public int WinTailLength { get => _winTailLength; }
    public ArrayPosHandler StartSnakeArrayPos { get => _startSnakeArrayPos;}

    //0 - Nope, 1 - head, 2 - tail, 3- food, 4 -bonus
    public int[,] gameArray = new int[19,11];
    public Vector2[,] positionsArray = new Vector2[19, 11];


    void Start()
    {
        bool isBigOfset = Screen.height / Screen.width >= 1.78f;

        _screenSizeInUnits = Camera.main.ScreenToWorldPoint(new Vector2(0, Camera.main.rect.yMax), Camera.MonoOrStereoscopicEye.Mono);

        if (isBigOfset)
            _screenSizeInUnits.x = Mathf.Abs(_screenSizeInUnits.x) - 1.0f;
        else
            _screenSizeInUnits.x = Mathf.Abs(_screenSizeInUnits.x) - 0.9f;

        _screenSizeInUnits.y = Mathf.Abs(_screenSizeInUnits.y);

        gridCellSize.x = (ScreenSizeInUnits.x * 2) / gridSizeInCells.x;
        gridCellSize.y = (ScreenSizeInUnits.y * 2) / gridSizeInCells.y;

        _cellSize = Mathf.Min(gridCellSize.x, gridCellSize.y);
        _snakeScale = new Vector2(CellSize / 2, CellSize / 2);

        float topOfset;
        
        if (isBigOfset)
        {
            topOfset = CellSize * 2.5f;
            _startSnakeArrayPos = new ArrayPosHandler(16, 5);
        } 
        else
        {
            topOfset = CellSize * 1.5f;
            _startSnakeArrayPos = new ArrayPosHandler(15, 5);
        }

        Vector2 leftTopPointInUnits = Camera.main.ScreenToWorldPoint(new Vector2(0, 0), Camera.MonoOrStereoscopicEye.Mono);
        leftTopPointInUnits = new Vector2(leftTopPointInUnits.x + (CellSize / 2), Mathf.Abs(leftTopPointInUnits.y) - topOfset);
        //Debug.Log($"Position {leftTopPointInUnits}");

        Vector2 currVertCellPos = leftTopPointInUnits
            , currHorCellPos = leftTopPointInUnits;

        for (int i = 0; i < positionsArray.GetLength(0); i++)
        {
            for (int j = 0; j < positionsArray.GetLength(1); j++)
            {
                positionsArray[i, j] = currHorCellPos;
                currHorCellPos += new Vector2(CellSize, 0);
            }
            currVertCellPos -= new Vector2(0, CellSize);
            currHorCellPos = currVertCellPos;
        }

        _winTailLength = (int)(gameArray.GetLength(0) * gameArray.GetLength(1)) - 1;

        if (isBigOfset)
        {
            TopDot_Line.position = new Vector2(0, leftTopPointInUnits.y + (CellSize / 2) + 0.12f);
            Info_Elements.position = new Vector2(Info_Elements.position.x, leftTopPointInUnits.y + (CellSize * 1.5f) + 0.1f);
            BotDot_Line.position = new Vector2(0, currVertCellPos.y + (CellSize / 2) - 0.12f);
        }
        else
        {
            TopDot_Line.position = new Vector2(0, leftTopPointInUnits.y + (CellSize / 2) + 0.12f);
            BotDot_Line.gameObject.SetActive(false);
        }   
    }

    public int GetGameArrayValue(ArrayPosHandler pos)
    {
        return gameArray[pos.x, pos.y];
    }

    public void ChangGameArray (ArrayPosHandler changePos, int value)
    {
        gameArray[changePos.x, changePos.y] = value;
    }
    public void ChangGameArray(int x , int y, int value)
    {
        gameArray[x, y] = value;
    }
    public Vector2 GetPosition(ArrayPosHandler currPosInArray)
    {
        return positionsArray[currPosInArray.x, currPosInArray.y];
    }
    public void ClearGameArray()
    {
        Array.Clear(gameArray, 0, gameArray.Length);
    }
}
