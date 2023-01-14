using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;

    public Image normalImage;
    public List<Sprite> normalImages;

    private Config.SquareColor currentColor = Config.SquareColor.NotSet;

    public Config.SquareColor GetCurrentColor()
    {
        return currentColor;
    }

    public bool selected { get; set;}
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get;set;}
    void Start()
    {
        selected = false;
        SquareOccupied = false;
    }
    //temp
    public bool canBePlaced()
    {
        return hoverImage.gameObject.activeSelf;
    }
    public void placeShapeonBoard(Config.SquareColor color)
    {
        currentColor = color;
        ActivateSquare();
    }
    public void ActivateSquare()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        selected = true;
        SquareOccupied=true;
    }
    public void DeactivateSquare()
    {
        currentColor = Config.SquareColor.NotSet;
        activeImage.gameObject.SetActive(false);
    }
    public void ClearOccupied()
    {
        currentColor = Config.SquareColor.NotSet;
        selected = false;
        SquareOccupied = false;
    }
    
    public void setImage(bool setFirstImage)
    {
        normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            selected = true;
            hoverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>()!=null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        selected = true;
        if (SquareOccupied == false)
        {
            hoverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            selected = false;
            hoverImage.gameObject.SetActive(false);     
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnsetOccupied();
        }
    }

}
