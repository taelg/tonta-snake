using System.Collections;
using UnityEngine;

public class DefaultFoodBehavior : MonoBehaviour
{
    [SerializeField] private float foodLifeTimeSecs = 10f;
    [SerializeField] private float shrinkDurationSecs = 5f;
    [SerializeField] private float startingScale = 2.5f;
    [SerializeField] private float endingScale = 0.5f;
    [SerializeField] private float startingAlpha = 1f;
    [SerializeField] private float endingAlpha = 0.25f;
    [SerializeField] private GameGridBehavior gameGrid;
    [SerializeField] private SpriteRenderer sprite;

    private float currentLifeTime;

    private void Start()
    {
        RestartFood();
    }

    public virtual void OnEatFood() { } //Each food type implements this differently.

    protected void RestartFood()
    {
        RepositionRandonly();
        StopAllCoroutines();
        StartCoroutine(HandleLifeTime());
    }

    protected virtual void OnLifetimeEnd()
    {
        RestartFood();
    }

    private void RepositionRandonly()
    {
        Vector2Int newPos = gameGrid.GetRandomFreeCellPosition();
        gameGrid.SetCellState(CellState.FOOD, newPos);
        this.transform.position = new Vector2(newPos.x, newPos.y);
    }

    private IEnumerator HandleLifeTime()
    {
        currentLifeTime = foodLifeTimeSecs;

        this.transform.localScale = new Vector2(startingScale, startingScale);
        SetFoodSpriteAlpha(startingAlpha);

        float shrinkStartTime = Time.time + (foodLifeTimeSecs - shrinkDurationSecs);

        while (currentLifeTime > 0)
        {
            currentLifeTime -= Time.deltaTime;

            if (Time.time >= shrinkStartTime)
            {
                float elapsedShrinkTime = foodLifeTimeSecs - currentLifeTime - (foodLifeTimeSecs - shrinkDurationSecs);
                float t = Mathf.Clamp01(elapsedShrinkTime / shrinkDurationSecs);

                float currentScale = Mathf.Lerp(startingScale, endingScale, t);
                this.transform.localScale = new Vector2(currentScale, currentScale);

                float currentAlpha = Mathf.Lerp(startingAlpha, endingAlpha, t);
                SetFoodSpriteAlpha(currentAlpha);
            }

            yield return null;
        }

        OnLifetimeEnd();
    }


    private void SetFoodSpriteAlpha(float a)
    {
        Color currentColor = sprite.color;
        currentColor.a = a;
        sprite.color = currentColor;

    }


}
