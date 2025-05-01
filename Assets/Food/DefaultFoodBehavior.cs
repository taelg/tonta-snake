using System;
using System.Collections;
using UnityEngine;

public class DefaultFoodBehavior : MonoBehaviour
{
    [SerializeField] private FoodType foodType;
    [SerializeField] private float foodLifeTimeSecs = 10f;
    [SerializeField] private float shrinkDurationSecs = 5f;
    [SerializeField] private float startingScale = 2.5f;
    [SerializeField] private float endingScale = 0.5f;
    [SerializeField] private float startingAlpha = 1f;
    [SerializeField] private float endingAlpha = 0.25f;
    [SerializeField] private SpriteRenderer sprite;

    private float currentLifeTime;

    public virtual void OnEatFood()
    {
        PointVFXManager.Instance.AnimatePoint(this.transform.position);
    }

    public void RestartFoodLifetime()
    {
        StopAllCoroutines();
        StartCoroutine(HandleLifeTime());
    }

    public void RepositionFoodOnGrid(bool clearGridData = false)
    {
        if (clearGridData)
            GameGridBehavior.Instance.ClearCellData(this.transform.position);

        Vector2 newPos = GameGridBehavior.Instance.GetRandomEmptyCell();
        GameGridBehavior.Instance.SetCellState(CellState.FOOD, newPos, FoodType.GREEN);
        this.transform.position = new Vector2(newPos.x, newPos.y);
    }

    protected virtual void EndLifeTime()
    {
        Vector2 foodPos = this.transform.position;
        GameGridBehavior.Instance.ClearCellData(foodPos);
        RestartFoodLifetime();
        RepositionFoodOnGrid();
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

        EndLifeTime();
    }


    private void SetFoodSpriteAlpha(float a)
    {
        Color currentColor = sprite.color;
        currentColor.a = a;
        sprite.color = currentColor;

    }

    public FoodType GetFoodType()
    {
        return foodType;
    }


}
