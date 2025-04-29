using System.Collections;
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

    public void AnimateDestroyPink()
    {
        StartCoroutine(AnimateDestroyPinkFX());

    }

    public IEnumerator AnimateDestroyPinkFX()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Color startColor = new Color(0.635f, 0.380f, 0.545f, 1);
        Color endColor = new Color(0.6352941f, 0.3803922f, 0.5450981f, 0.2f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            sprite.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }

        sprite.color = Color.white;
        gameObject.SetActive(false);
    }


}

