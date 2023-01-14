using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;
    public SquareTextureData squareTextureData;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;
    private Config.SquareColor _currentActive = Config.SquareColor.NotSet;
    private List<Config.SquareColor> colorsInGrid = new List<Config.SquareColor>();
    
    private void OnEnable()
    {
        GameEvents.CheckifShapeCanBePlaced += CheckifShapeCanBePlaced;
        GameEvents.UpdateSquareColor += OnUpdateSquareColor;
    }

    private void OnDisable()
    {
        GameEvents.CheckifShapeCanBePlaced -= CheckifShapeCanBePlaced;
        GameEvents.UpdateSquareColor -= OnUpdateSquareColor;
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
        _currentActive = squareTextureData.activesquareTextures[0].squareColor;
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        _currentActive = color;
    }

    private List<Config.SquareColor> GetAllColors()
    {
        var colors = new List<Config.SquareColor>();

        foreach(var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if(gridSquare.SquareOccupied)
            {
                var color = gridSquare.GetCurrentColor();
                if(colors.Contains(color)==false)
                {
                    //Debug.Log("GET Color" + color);
                    colors.Add(color);
                }
            }
        }

        return colors;
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPosition();
    }

    private void SpawnGridSquares()
    {
        int squareindex = 0;

        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);
                _gridSquares[_gridSquares.Count-1].GetComponent<GridSquare>().SquareIndex = squareindex;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().setImage(_lineIndicator.GetGridSquareIndex(squareindex) % 2 == 0);
                
                squareindex++;
            }
        }
    }  
    private void SetGridSquaresPosition()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gapnumber = new Vector2(x: 0.0f, y: 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach(GameObject square in _gridSquares)
        {
            if (column_number+1>columns)
            {
                square_gapnumber.x = 0;
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            var pos_x_offset = _offset.x * column_number + ( square_gapnumber.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gapnumber.y * squaresGap);

            if (column_number>0 && column_number%3==0)
            {
                square_gapnumber.x++;
                pos_x_offset += squaresGap;
            }

            if(row_number>0 && row_number%3==0 && row_moved==false)
            {
                row_moved = true;
                square_gapnumber.y++;
                pos_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x+pos_x_offset, startPosition.y-pos_y_offset);

            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset, 0.0f);

            column_number++;
        }
    }

    private void CheckifShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach(var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if(gridSquare.selected && !gridSquare.SquareOccupied)
            {
                //gridSquare.ActivateSquare();
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        if(currentSelectedShape.TotalSquareNumber==squareIndexes.Count)
        {
            foreach(var squareInd in squareIndexes)
            {
                _gridSquares[squareInd].GetComponent<GridSquare>().placeShapeonBoard(_currentActive);
            }

            var shapeLeft = 0;

            foreach(var shape in shapeStorage.shapeList)
            {
                if(shape.isOnStartPosition() && shape.isAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }


            if(shapeLeft==0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfLineIsComplited();

            
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }

        //shapeStorage.GetCurrentSelectedShape().DeactivateShape();
    }

    void CheckIfLineIsComplited()
    {
        List<int[]> lines = new List<int[]>();
        //columns
        foreach(var col in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(col));
        }
        //rows
        for(var row=0;row<9;row++)
        {
            List<int> data=new List<int>(9);
            for(var ind = 0;ind<9;ind++)
            {
                data.Add(_lineIndicator.line_data[row, ind]);
            }

            lines.Add(data.ToArray());
        }
        //squares
        for(var square=0;square<9;square++)
        {
            List<int>data=new List<int>(9);
            for(var ind=0; ind<9;ind++)
            {
                data.Add(_lineIndicator.square_data[square, ind]);
            }
            lines.Add(data.ToArray());
        }

        //
        //Debug.Log("colors in Grid");
        colorsInGrid = GetAllColors();

        var completedLines = checkIfSquareComplited(lines);
        if(completedLines>=2)
        {
            //bonus
            GameEvents.ShowCongratulation();
        }
        var totalScore = 10*completedLines;
        //Debug.Log("colors Bonus");
        var bonusSc = PlayColorBonus();
        totalScore += bonusSc;
        GameEvents.AddScore(totalScore);
        Debug.Log("TOTAL SCORE"+totalScore);
        CheckifLost();
    }

    private int PlayColorBonus()
    {
        var colorsInGridAft =GetAllColors();
        Config.SquareColor colorBonus = Config.SquareColor.NotSet;
        foreach(var sqcolor in colorsInGrid)
        {

            if(colorsInGridAft.Contains(sqcolor)==false)
            {
                colorBonus = sqcolor;
                Debug.Log("Bonus color: " + sqcolor+" "+colorBonus);
            }
        }

        if(colorBonus == Config.SquareColor.NotSet)
        {
            Debug.Log("Cannot find bonus");
            return 0;
        }

        if(colorBonus==_currentActive)
        {
            return 0;
        }

        GameEvents.ShowBonusScreen(colorBonus);
        return 50;
    }

    private int checkIfSquareComplited(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;
        foreach(var line in data)
        {
            var lineCompleted = true;
            foreach(var squareindex in line)
            {
                var comp = _gridSquares[squareindex].GetComponent<GridSquare>();
                if(comp.SquareOccupied==false)
                {
                    lineCompleted = false;
                }
            }

            if(lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach(var line in completedLines)
        {
            var completed = false;
            foreach(var squareindex in line)
            {
                var comp = _gridSquares[squareindex].GetComponent<GridSquare>();
                comp.DeactivateSquare();
                completed = true;
            }
            foreach (var squareindex in line)
            {
                var comp = _gridSquares[squareindex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }
            if(completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CheckifLost()
    {
        var validShapes = 0;
        for(var ind=0;ind<shapeStorage.shapeList.Count;ind++)
        {
            var isShapeActive = shapeStorage.shapeList[ind].isAnyOfShapeSquareActive();
            if (CheckiIfShapeCanBePlaced(shapeStorage.shapeList[ind])&& isShapeActive)
            {
                shapeStorage.shapeList[ind]?.ActivateShape();
                validShapes++;
            }

        }

        if(validShapes==0)
        {
            
            GameEvents.GameOver(false);
        }
    }

    private bool CheckiIfShapeCanBePlaced(Shape currShape)
    {
        var currShapeData = currShape.currentShapedata;
        var shapeColumns = currShapeData.columns;
        var shapeRows = currShapeData.rows;

        List<int> filledSquares = new List<int>();
        var squareIndex = 0;

        for(var rowIndex=0; rowIndex<shapeRows;rowIndex++)
        {
            for(var colIndex=0;colIndex<shapeColumns;colIndex++)
            {
                if (currShapeData.board[rowIndex].column[colIndex])
                {
                    filledSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        if(currShape.TotalSquareNumber!=filledSquares.Count)
        {
            Debug.LogError("B??d. Liczba kafelek r??ni si? od oryginalnej");
        }

        var squarelist = GetAllSquaresCombinations(shapeColumns, shapeRows);

        bool canBePlaced = false;
        foreach(var num in squarelist)
        {
            bool shapeCanBePlaced = true;
            foreach(var sqInd in filledSquares)
            {
                var comp = _gridSquares[num[sqInd]].GetComponent<GridSquare>();
                if(comp.SquareOccupied)
                {
                    shapeCanBePlaced = false;
                }
            }
            if(shapeCanBePlaced)
            {
                canBePlaced = true;
            }
        }
        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombinations(int cols, int rows)
    {
        var squareList = new List<int[]>();
        var lastColIndex = 0;
        var lastRowIndex = 0;
        int safeIndex = 0;

        while(lastRowIndex+(rows-1)<9)
        {
            var rowData = new List<int>();

            for(var row=lastRowIndex;row<lastRowIndex+rows;row++)
            {
                for(var col = lastColIndex;col<lastColIndex+cols;col++)
                {
                    rowData.Add(_lineIndicator.line_data[row, col]);
                }
            }
            squareList.Add(rowData.ToArray());

            lastColIndex++;
            if(lastColIndex+(cols-1)>=9)
            {
                lastRowIndex++;
                lastColIndex=0;
            }

            safeIndex++;
            if(safeIndex>100)
            {
                break;
            }
        }

        return squareList;

    }
}
