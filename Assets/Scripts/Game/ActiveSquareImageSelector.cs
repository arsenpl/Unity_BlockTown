using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSquareImageSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool updateImage = false;
    private void OnEnable()
    {
        UpdateColorByPoints();

        if (updateImage)
            GameEvents.UpdateSquareColor += UpdateSquaresColor;
    }

    private void OnDisable()
    {
        if (updateImage)
            GameEvents.UpdateSquareColor -= UpdateSquaresColor;
    }

    private void UpdateColorByPoints()
    {
        foreach(var squaretexture in squareTextureData.activesquareTextures)
        {
            if(squareTextureData.currentColor==squaretexture.squareColor)
            {
                GetComponent<Image>().sprite=squaretexture.texture;
            }
        }
    }

    private void UpdateSquaresColor(Config.SquareColor color)
    {
        foreach (var squareTexture in squareTextureData.activesquareTextures)
        {
            if(color==squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }

}
