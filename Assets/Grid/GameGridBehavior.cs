using System.Collections.Generic;
using UnityEngine;

public class GameGridBehavior : MonoBehaviour
{
    private int gridWidth = 31;
    private int gridHeight = 17;
    private GridCell[,] grid;

    private void Awake()
    {
        InitializeGrid();
        PopulateGridWithCells();
    }

    private void InitializeGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];
    }

    private void PopulateGridWithCells()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var cell = new GridCell(x, y);
                grid[x, y] = cell;
            }
        }
    }

    private void Update()
    {
        //DrawDebugBoxes();
    }

    private void DrawDebugBoxes()
    {
        int cellSize = 1;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GridCell cell = grid[x, y];

                Vector3 bottomLeft = new Vector3((x * cellSize) - (cellSize / 2f), (y * cellSize) - (cellSize / 2f), 0);
                Vector3 bottomRight = bottomLeft + new Vector3(cellSize, 0, 0);
                Vector3 topLeft = bottomLeft + new Vector3(0, cellSize, 0);
                Vector3 topRight = bottomLeft + new Vector3(cellSize, cellSize, 0);

                FoodType foodType = cell.foodType;
                Color color = cell.state == CellState.SNAKE ? Color.red : Color.black;
                bool isFoodInCell = cell.state == CellState.SNAKE_AND_FOOD || cell.state == CellState.FOOD;
                if (isFoodInCell)
                {
                    color = foodType == FoodType.GREEN ? Color.green : color;
                    color = foodType == FoodType.PINK ? Color.magenta : color;
                }

                if (cell.state != CellState.EMPTY)
                {
                    Debug.DrawLine(bottomLeft, bottomRight, color, 0f);
                    Debug.DrawLine(bottomRight, topRight, color, 0f);
                    Debug.DrawLine(topRight, topLeft, color, 0f);
                    Debug.DrawLine(topLeft, bottomLeft, color, 0f);
                }
            }
        }
    }

    public Vector2 GetRandomEmptyCell()
    {
        List<GridCell> freeCells = GetAllEmptyCells();
        return freeCells[Random.Range(0, freeCells.Count)].GetPosition();
    }

    private List<GridCell> GetAllEmptyCells()
    {
        List<GridCell> emptyCells = new List<GridCell>();
        foreach (GridCell cell in grid)
        {
            if (cell.state == CellState.EMPTY)
                emptyCells.Add(cell);
        }
        return emptyCells;
    }

    public Vector2 MirrorPositionIfOutOfBounds(Vector2 pos)
    {
        pos.x = MirrorCoordinate(pos.x, gridWidth);
        pos.y = MirrorCoordinate(pos.y, gridHeight);
        return pos;
    }

    private int MirrorCoordinate(float value, int totalSize)
    {
        if (value >= totalSize) return 0;
        if (value < 0) return totalSize - 1;
        return (int)value;
    }

    public bool IsGridCellFree(Vector2 pos)
    {
        CellState objectInTheCell = GetGridCell(pos.x, pos.y).state;
        return objectInTheCell == CellState.EMPTY || objectInTheCell == CellState.FOOD;
    }

    public void ClearCellData(Vector2 pos)
    {
        var cell = GetGridCell(pos.x, pos.y);
        cell.state = CellState.EMPTY;
        cell.foodType = FoodType.NONE;
    }

    public void SetCellState(CellState cellObject, Vector2 pos, FoodType foodType = FoodType.NONE)
    {
        GridCell cell = grid[(int)pos.x, (int)pos.y];
        cell.state = cellObject;
        cell.foodType = foodType;
    }

    public CellState GetCellState(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y].state;
    }

    public FoodType GetFoodType(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y].foodType;
    }

    private GridCell GetGridCell(float x, float y)
    {
        return grid[(int)x, (int)y];
    }

    public void ClearGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var cell = grid[x, y];
                cell.state = CellState.EMPTY;
                cell.foodType = FoodType.NONE;
            }
        }
    }


}