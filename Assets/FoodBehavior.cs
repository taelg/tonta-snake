using UnityEngine;

public class FoodBehavior : MonoBehaviour
{

    [SerializeField] private GameGridBehavior gameGrid;

    void Start()
    {
        Reposition();
    }

    public void Reposition()
    {
        Debug.Log("Reposition");
        Vector2Int newPos = gameGrid.GetRandomFreeCellPosition();
        gameGrid.OcupyCell(CellObject.FOOD, newPos);
        this.transform.position = new Vector2(newPos.x, newPos.y);
    }


}
