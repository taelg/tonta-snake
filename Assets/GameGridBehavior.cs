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

                // Define as coordenadas do canto inferior esquerdo da célula
                Vector3 bottomLeft = new Vector3(x * cellSize - (cellSize / 2), y * cellSize - (cellSize / 2), 0);
                Vector3 bottomRight = bottomLeft + new Vector3(cellSize, 0, 0);
                Vector3 topLeft = bottomLeft + new Vector3(0, cellSize, 0);
                Vector3 topRight = bottomLeft + new Vector3(cellSize, cellSize, 0);

                // Desenha as linhas das células
                Debug.DrawLine(bottomLeft, bottomRight, Color.green, 100f);
                Debug.DrawLine(bottomRight, topRight, Color.green, 100f);
                Debug.DrawLine(topRight, topLeft, Color.green, 100f);
                Debug.DrawLine(topLeft, bottomLeft, Color.green, 100f);
            }
        }
    }

    private List<GridCell> GetFreeCells()
    {
        List<GridCell> freeCells = new List<GridCell>();
        foreach (GridCell cell in grid.Values)
        {
            if (cell.objectInCell == CellObject.EMPTY)
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
        CellObject objectInTheCell = grid[new Vector2Int(pos.x, pos.y)].objectInCell;
        return objectInTheCell == CellObject.EMPTY || objectInTheCell == CellObject.FOOD;
    }

    public void ClearCell(Vector2Int pos)
    {
        grid[new Vector2Int(pos.x, pos.y)].objectInCell = CellObject.EMPTY;
    }

    public void OcupyCell(CellObject cellObject, Vector2Int pos)
    {
        grid[new Vector2Int(pos.x, pos.y)].objectInCell = cellObject;
    }


}

public class GridCell
{
    public Vector2Int position;
    public CellObject objectInCell = CellObject.EMPTY;

    public GridCell(int x, int y)
    {
        this.position = new Vector2Int(x, y);
    }

}