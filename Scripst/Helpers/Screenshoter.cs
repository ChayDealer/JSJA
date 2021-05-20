using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Screenshoter : MonoBehaviour
{
    short tapCount = 0;

    private void LateUpdate()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            tapCount += 1;
            StartCoroutine(Countdown());
        }

        if (tapCount == 3)
        {
            tapCount = 0;
            StopCoroutine(Countdown());

            // DO STUFF!
            string date = DateTime.Now.ToString();
            date = date.Replace("/", "-");
            date = date.Replace(":", "-");
            date = date.Replace(".", "-");
            date = date.Replace(" ", "_");
            
            ScreenCapture.CaptureScreenshot(@"E:\Unity Support\Snake\ScreenShots\Screenshot_" + date + ".png");
            Debug.Log("double Tap and Screenshot");
        }

    }
    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(0.3f);
        tapCount = 0;
    }
}

