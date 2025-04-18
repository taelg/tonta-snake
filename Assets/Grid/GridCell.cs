using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private int posX;
    private int posY;
    public CellState state = CellState.EMPTY;

    public GridCell(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

}