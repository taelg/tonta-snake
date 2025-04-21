using UnityEngine;

public class BodyPartBehavior : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private CellStateColorMap stateColorMap;
    [SerializeField] private FoodTypeColorMap foodTypeColorMap;

    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    public void MoveTo(Vector2 position)
    {
        this.transform.position = position;
    }


    public void UpdateColor(CellState state, FoodType foodType)
    {
        bool stateContainsFood = state == CellState.FOOD || state == CellState.SNAKE_AND_FOOD;
        Color newColor = stateContainsFood ? foodTypeColorMap.GetColor(foodType) : stateColorMap.GetColor(state);

        if (sprite.color != newColor)
            sprite.color = newColor;
    }



}

