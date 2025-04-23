using System.Collections;
using UnityEngine;

public class GreenFoodBehavior : DefaultFoodBehavior
{
    [SerializeField] private PinkFoodBehavior pinkFood;
    private int ateCounter = 0;

    public override void OnEatFood()
    {
        RestartFood();
        ateCounter++;
        if (ateCounter % 10 == 0)
        {
            pinkFood.ActiveSpecialFood();
        }
    }

    public override void OnFoodRestart()
    {
        base.OnFoodRestart();
        ateCounter = 0;
    }

}
