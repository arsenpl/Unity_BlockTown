using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public Config.SquareColor squareColor;
    }

    public int tresholdVal;
    private const int startTreshholdVal = 50;
    public List<TextureData> activesquareTextures;

    public Config.SquareColor currentColor;
    private Config.SquareColor nextColor;

    private int GetCurrentColorIndex()
    {
        var currentIndex = 0;
        for(int i=0;i< activesquareTextures.Count;i++)
        {
            if(activesquareTextures[i].squareColor == currentColor)
            {
                currentIndex = i;
            }
        }
        return currentIndex;
    }

    public void UpdateColor(int current_score)
    {
        currentColor = nextColor;
        var currentColorIndex = GetCurrentColorIndex();

        if(currentColorIndex==activesquareTextures.Count-1)
        {
            nextColor = activesquareTextures[0].squareColor;
        }
        else
        {
            nextColor = activesquareTextures[currentColorIndex + 1].squareColor;
        }
        tresholdVal = startTreshholdVal + current_score;
    }

    public void SetStartColor()
    {
        tresholdVal=startTreshholdVal;
        currentColor = activesquareTextures[0].squareColor;
        nextColor = activesquareTextures[1].squareColor;
    }

    private void Awake()
    {
        SetStartColor();
    }

    private void OnEnable()
    {
        SetStartColor();
    }
}
