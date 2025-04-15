using System.Collections;
using UnityEngine;

public class BasicFoodBehavior : DefaultFoodBehavior
{
    [SerializeField] private SpecialFoodBehavior specialFood;
    private int ateCounter = 0;

    public override void OnEatFood()
    {
        RestartFood();
        ateCounter++;
        if (ateCounter % 10 == 0)
        {
            specialFood.ActiveSpecialFood();
        }
    }

}
