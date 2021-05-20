using UnityEngine;
using UnityEngine.UI;

public class TakeScore : MonoBehaviour
{
    private Text _scoreText;
    private MenuHandler mh;

    private void Awake()
    {
        mh = GameObject.FindGameObjectWithTag("UI_Menu").GetComponent<MenuHandler>();
        _scoreText = gameObject.GetComponent<Text>();
    }

    public void SetScore()
    {
        _scoreText.text = mh?.GetScore().ToString();
    }
}
