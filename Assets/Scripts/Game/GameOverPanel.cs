using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public GameObject GameOver;
    public GameObject Loose;
    public GameObject NewBest;
    public Text ScoreText;



    public void Start()
    {
        GameOver.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
        GameEvents.GameScore += UpdateScore;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
        GameEvents.GameScore -= UpdateScore;

    }

    private void OnGameOver(bool newbestScore)
    {
        GameOver.SetActive(true);
        if(newbestScore)
        {
            NewBest.SetActive(true);
            Loose.SetActive(false);
        }
        else
        {
            NewBest.SetActive(false);
            Loose.SetActive(true);
        }
            
    }

    private void UpdateScore(int sc)
    {
        ScoreText.text = sc.ToString();
    }
}
