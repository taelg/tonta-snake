using System.Collections;
using UnityEngine;

public class GreenFoodBehavior : DefaultFoodBehavior
{
    [SerializeField] private PinkFoodBehavior pinkFood;
    private int ateCounter = 0;

    public override void OnEatFood()
    {
        base.OnEatFood();
        RestartFood();
        ateCounter++;
        if (ateCounter % 10 == 0)
        {
            pinkFood.ActiveSpecialFood();
        }
    }

    public override void OnResetFood()
    {
        base.OnResetFood();
        ateCounter = 0;
    }

}
