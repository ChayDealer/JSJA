using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FulFillment : MonoBehaviour
{
    [SerializeField]
    private UnityEvent HideShop;

    public void CompleteRemovingAds()
    {
        Debug.Log("We happy?");
        PlayerPrefs.SetInt("happy", 1);
        HideShop?.Invoke();
        MenuHandler.debugTextObj.text = "Купленно";
    }

    public void FaileRemovingAds()
    {
        Debug.Log("Oops!");
        MenuHandler.debugTextObj.text = "НЕ купленно";
    }
}
