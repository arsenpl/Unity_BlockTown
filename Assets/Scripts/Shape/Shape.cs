using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f);

    [HideInInspector]
    public ShapeData currentShapedata;

    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentShapes = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapaDraggable = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;
    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform=this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapaDraggable = true;
        _startPosition = _transform.localPosition;
        _shapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    public void DeactivateShape()
    {
        if(_shapeActive)
        {
            foreach(var square in _currentShapes)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }
        _shapeActive = false;
    }

    private void SetShapeInactive()
    {
        if(isOnStartPosition()==false && isAnyOfShapeSquareActive())
        {
            foreach(var square in _currentShapes)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateShape()
    {
        if(!_shapeActive)
        {
            foreach(var square in _currentShapes)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }  
        }
        _shapeActive=true;
    }
    public bool isOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool isAnyOfShapeSquareActive()
    {
        foreach(var square in _currentShapes)
        {
            if(square.gameObject.activeSelf)
                return true;
        }
        return false;
    }
    public void RequestShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }
    public void CreateShape(ShapeData shapeData)
    {
        currentShapedata = shapeData;
        TotalSquareNumber = getNumberofSquares(shapeData);

        while(_currentShapes.Count<=TotalSquareNumber)
        {
            _currentShapes.Add(Instantiate(squareShapeImage, transform)as GameObject);
        }

        foreach(var square in _currentShapes)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var square_rect = squareShapeImage.GetComponent<RectTransform>();   
        var moveDistance = new Vector2(square_rect.rect.width*square_rect.localScale.x,
            square_rect.rect.height*square_rect.localScale.y);
        int currecntIndexList = 0;

        for(var row = 0; row<shapeData.rows; row++)
        {
            for (var column=0; column<shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShapes[currecntIndexList].SetActive(true);
                    _currentShapes[currecntIndexList].GetComponent<RectTransform>().localPosition =
                        new Vector2(getXPositionforShapeSquare(shapeData, column, moveDistance), getYPositionforShapeSquare(shapeData, row, moveDistance));
                    currecntIndexList++;
                }
            }
        }
    }
    private float getYPositionforShapeSquare(ShapeData shapeData, int row, Vector2 movedistance)
    {
        float shiftonY = 0f;
        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middeSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;
                if (row < middeSquareIndex)
                {
                    shiftonY = movedistance.y * 1;
                    shiftonY *= multiplier;
                }
                else if (row > middeSquareIndex)
                {
                    shiftonY = movedistance.x * -1;
                    shiftonY *= multiplier;
                }
            }
            else
            {
                var middlesquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middlesquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 1;
                var multiplier = shapeData.rows / 2;

                if (row == middlesquareIndex1 || row == middlesquareIndex2)
                {
                    if (row == middlesquareIndex2)
                        shiftonY = movedistance.y / 2 * -1;
                    if (row == middlesquareIndex1)
                        shiftonY = movedistance.y / 2;
                }

                if (row < middlesquareIndex1 && row < middlesquareIndex2)
                {
                    shiftonY = movedistance.y*1;
                    shiftonY *= multiplier;
                }
                else if (row > middlesquareIndex1 && row > middlesquareIndex2)
                {
                    shiftonY = movedistance.y *-1;
                    shiftonY *= multiplier;
                }
            }
        }
        return shiftonY;
    }

    private float getXPositionforShapeSquare(ShapeData shapeData, int column, Vector2 movedistance)
    {
        float shiftonX = 0f;
        if(shapeData.columns>1)
        {
            if (shapeData.columns % 2 != 0)
            {
                var middeSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middeSquareIndex)
                {
                    shiftonX = movedistance.x * -1;
                    shiftonX *= multiplier;
                }
                else if (column > middeSquareIndex)
                {
                    shiftonX = movedistance.x * 1;
                    shiftonX *= multiplier;
                }
            }
            else
            {
                var middlesquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middlesquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns / 2;

                if(column == middlesquareIndex1 || column== middlesquareIndex2)
                {
                    if (column == middlesquareIndex2)
                        shiftonX = movedistance.x / 2;
                    if (column == middlesquareIndex1)
                        shiftonX = movedistance.x / 2 * -1;
                }

                if(column<middlesquareIndex1 && column<middlesquareIndex2)
                {
                    shiftonX = movedistance.x / 2 * -1;
                    shiftonX *= multiplier;
                }
                else if(column>middlesquareIndex1 && column>middlesquareIndex2)
                {
                    shiftonX = movedistance.x * 1;
                    shiftonX *= multiplier;
                }
                
            }
           
        }
        return shiftonX;
    }

    private int getNumberofSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach(var rowData in shapeData.board)
        {
            foreach(var active in rowData.column)
            {
                if(active)
                    number++;
            }
        }
        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckifShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }
}
