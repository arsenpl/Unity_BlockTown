using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int score=0;
}
public class Scores : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public Text scoreText;
    private bool _newbestScore = false;
    private BestScoreData _bestScore = new BestScoreData();
    private int currScore;
    private string _bestScorekey = "bestscr";

    void Awake()
    {
        if (BinaryDataStream.Exist(_bestScorekey)) ;
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        _bestScore = BinaryDataStream.Read<BestScoreData>(_bestScorekey);
        yield return new WaitForEndOfFrame();
        Debug.Log("READ BEST: "+_bestScore.score);
        GameEvents.UpdateBestScoreBar(currScore, _bestScore.score);

    }
    void Start()
    {
        currScore = 0;
        _newbestScore = false;
        squareTextureData.SetStartColor();
        UpdatteScoreText();
    }
    private void OnEnable()
    {
        GameEvents.AddScore += AddScore;
        GameEvents.GameOver += saveBestScore;
    }

    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
        GameEvents.GameOver -= saveBestScore;
    }

    public void saveBestScore(bool newbest)
    {
        BinaryDataStream.Save<BestScoreData>(_bestScore,_bestScorekey);
        GameEvents.GameScore(currScore);
    }

    private void AddScore(int score)
    {
        currScore += score;
        if(currScore > _bestScore.score)
        {
            _newbestScore = true;
            _bestScore.score = currScore;
            saveBestScore(true);
        }
        UpdateSquareColor();
        GameEvents.UpdateBestScoreBar(currScore, _bestScore.score);
        UpdatteScoreText();
    }

    private void UpdateSquareColor()
    {
        if(GameEvents.UpdateSquareColor != null && currScore>=squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColor(currScore);
            GameEvents.UpdateSquareColor(squareTextureData.currentColor);
        }
    }

    private void UpdatteScoreText()
    {
        scoreText.text = currScore.ToString();
    }
}
