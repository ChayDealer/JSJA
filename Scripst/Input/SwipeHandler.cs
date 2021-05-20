using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class SwipeHandler : MonoBehaviour
{
    public float swipeThreshold = 20f;
    public float timeThreshold = 0.5f;

    public Snake snake;

    private Vector2 fingerDown;
    private DateTime fingerDownTime;
    private Vector2 fingerUp;
    private DateTime fingerUpTime;

    public delegate void Swipe(Vector2 dir);
    public static event Swipe OnSwipeEnd;

    public UnityEvent hideGameTip;

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    fingerDown = Input.mousePosition;
        //    fingerUp = Input.mousePosition;
        //    fingerDownTime = DateTime.Now;
        //}
        //if (Input.GetMouseButtonUp(0))
        //{
        //    fingerDown = Input.mousePosition;
        //    fingerUpTime = DateTime.Now;
        //    CheckSwipe();
        //}
        if (Snake.curGameState == Snake.GameState.Game
                || Snake.curGameState == Snake.GameState.Immun
                || Snake.curGameState == Snake.GameState.TipPause)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerDown = touch.position;
                    fingerUp = touch.position;
                    fingerDownTime = DateTime.Now;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;
                    fingerUpTime = DateTime.Now;
                    CheckSwipe();
                }
            }
        }
    }

    private void CheckSwipe()
    {
        float duration = (float)fingerUpTime.Subtract(fingerDownTime).TotalSeconds;
        if (duration > timeThreshold) return;

        float deltaX = fingerDown.x - fingerUp.x;
        float deltaY = fingerDown.y - fingerUp.y;

        if (Mathf.Abs(deltaX) < swipeThreshold && Mathf.Abs(deltaY) < swipeThreshold) return;


        if (MenuHandler.isTipsEnable)
        {
            hideGameTip?.Invoke();
            MenuHandler.isTipsEnable = false;
        }

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            if (Mathf.Abs(deltaX) >= swipeThreshold)
            {
                if (deltaX >= 0)
                    OnSwipeEnd?.Invoke(Vector2.up);
                    //snake.DirectionInput(Vector2.up);
                else
                    OnSwipeEnd?.Invoke(Vector2.down);
                    //snake.DirectionInput(Vector2.down);
            }
        }
        else if(Mathf.Abs(deltaY) >= swipeThreshold)
        {
            if (deltaY >= 0)
                OnSwipeEnd?.Invoke(Vector2.left);
                //snake.DirectionInput(Vector2.left);
            else
                OnSwipeEnd?.Invoke(Vector2.right);
                //snake.DirectionInput(Vector2.right);
        }

        fingerUp = fingerDown;
    }
}
