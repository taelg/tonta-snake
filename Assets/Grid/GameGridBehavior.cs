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
        DrawDebugFoodBoxes();
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

                Color color = cell.state == CellState.SNAKE ? Color.red : Color.black;
                color = cell.state == CellState.FOOD ? Color.green : color;
                color = cell.state == CellState.SNAKE_AND_FOOD ? Color.blue : color;

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

    private void DrawDebugFoodBoxes()
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

                Color color = cell.foodType == FoodType.GREEN ? Color.green : Color.black;
                color = cell.foodType == FoodType.PINK ? Color.magenta : color;
                color = cell.foodType == FoodType.NONE ? Color.white : color;

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


    public Vector2Int GetRandomEmptyCell()
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

    public Vector2Int MirrorPositionIfOutOfBounds(Vector2Int pos)
    {
        pos.x = MirrorCoordinate(pos.x, gridWidth);
        pos.y = MirrorCoordinate(pos.y, gridHeight);
        return pos;
    }

    private int MirrorCoordinate(int value, int totalSize)
    {
        if (value >= totalSize) return 0;
        if (value < 0) return totalSize - 1;
        return value;
    }

    public bool IsGridCellFree(Vector2Int pos)
    {
        CellState objectInTheCell = grid[pos.x, pos.y].state;
        return objectInTheCell == CellState.EMPTY || objectInTheCell == CellState.FOOD;
    }

    public void ClearCellState(Vector2Int pos)
    {
        grid[pos.x, pos.y].state = CellState.EMPTY;
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

    public FoodType GetFoodTypeInCell(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y].foodType;
    }

    public void ClearGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var cell = grid[x, y];
                cell.state = CellState.EMPTY;
            }
        }
    }


}