using System;
using UnityEngine;

public class GreenFoodBehavior : DefaultFoodBehavior
{
    private Action OnEatFoodCallback;

    private void Start()
    {
        RepositionFoodOnGrid();
    }

    public void SetOnEatFoodCallback(Action callback)
    {
        OnEatFoodCallback = callback;
    }

    public override void OnEatFood()
    {
        base.OnEatFood();
        RestartFoodLifetime();
        OnEatFoodCallback.Invoke();
    }

}
