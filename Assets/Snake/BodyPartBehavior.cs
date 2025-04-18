using UnityEngine;

public class BodyPartBehavior : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private CellStateColorMap colorMap;

    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    public void MoveTo(Vector2 position)
    {
        this.transform.position = position;
    }


    public void UpdateColor(CellState state)
    {
        Color color = colorMap.GetColor(state);
        sprite.color = color;
    }



}

