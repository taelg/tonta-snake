using System.Collections.Generic;
using UnityEngine;

public class GameGridBehavior : MonoBehaviour
{
    private Dictionary<Vector2Int, GridCell> grid = new Dictionary<Vector2Int, GridCell>();
    public float cellSize = 1.125f;
    private int mapLimitInX = 15;
    private int mapLimitInY = 8;



    private void Awake()
    {
        for (int x = -mapLimitInX; x <= mapLimitInX; x++)
        {
            for (int y = -mapLimitInY; y <= mapLimitInY; y++)
            {
                var cell = new GridCell(x, y);
                grid[new Vector2Int(x, y)] = cell;
            }
        }
    }

    public void Update()
    {
        for (int x = -mapLimitInX; x <= mapLimitInX; x++)
        {
            for (int y = -mapLimitInY; y <= mapLimitInY; y++)
            {
                GridCell cell = grid[new Vector2Int(x, y)];

                // Define as coordenadas do canto inferior esquerdo da célula
                Vector3 bottomLeft = new Vector3(x * cellSize - (cellSize / 2), y * cellSize - (cellSize / 2), 0);
                Vector3 bottomRight = bottomLeft + new Vector3(cellSize, 0, 0);
                Vector3 topLeft = bottomLeft + new Vector3(0, cellSize, 0);
                Vector3 topRight = bottomLeft + new Vector3(cellSize, cellSize, 0);

                Color color = cell.state == CellState.SNAKE ? Color.red : Color.black;
                color = cell.state == CellState.FOOD ? Color.green : color;
                color = cell.state == CellState.SNAKE_AND_FOOD ? Color.blue : color;

                if (cell.state != CellState.EMPTY)
                {
                    // Desenha as linhas das células
                    Debug.DrawLine(bottomLeft, bottomRight, color, 0f);
                    Debug.DrawLine(bottomRight, topRight, color, 0f);
                    Debug.DrawLine(topRight, topLeft, color, 0f);
                    Debug.DrawLine(topLeft, bottomLeft, color, 0f);
                }

            }
        }
    }

    private List<GridCell> GetFreeCells()
    {
        List<GridCell> freeCells = new List<GridCell>();
        foreach (GridCell cell in grid.Values)
        {
            if (cell.state == CellState.EMPTY)
                freeCells.Add(cell);
        }
        return freeCells;
    }

    public Vector2Int GetRandomFreeCellPosition()
    {
        List<GridCell> freeCells = GetFreeCells();
        return freeCells[Random.Range(0, freeCells.Count)].position;
    }

    public Vector2Int MirrorPositionIfOutOfBounds(Vector2Int pos)
    {
        pos.x = MirrorCoordinate(pos.x, mapLimitInX);
        pos.y = MirrorCoordinate(pos.y, mapLimitInY);
        return pos;
    }

    private int MirrorCoordinate(int value, int limit)
    {
        if (value > limit) return -limit;
        if (value < -limit) return limit;
        return value;
    }

    public bool IsGridCellFree(Vector2Int pos)
    {
        CellState objectInTheCell = grid[new Vector2Int(pos.x, pos.y)].state;
        return objectInTheCell == CellState.EMPTY || objectInTheCell == CellState.FOOD;
    }

    public void ClearCellState(Vector2Int pos)
    {
        grid[new Vector2Int(pos.x, pos.y)].state = CellState.EMPTY;
    }

    public void SetCellState(CellState cellObject, Vector2Int pos)
    {
        grid[new Vector2Int(pos.x, pos.y)].state = cellObject;
    }

    public CellState GetCellState(Vector2Int pos)
    {
        return grid[new Vector2Int(pos.x, pos.y)].state;
    }


}

public class GridCell
{
    public Vector2Int position;
    public CellState state = CellState.EMPTY;

    public GridCell(int x, int y)
    {
        this.position = new Vector2Int(x, y);
    }

}