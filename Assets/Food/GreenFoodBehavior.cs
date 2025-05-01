using UnityEngine;

public class GreenFoodBehavior : DefaultFoodBehavior
{
    [SerializeField] private PinkFoodBehavior pinkFood;
    [SerializeField] private float pinkFoodChance = 0.05f;
    [SerializeField] private OrangeFoodBehavior OrangeFood;
    [SerializeField] private float orangeFoodChance = 0.2f;

    public override void OnEatFood()
    {
        base.OnEatFood();
        bool spawnPink = Random.value <= pinkFoodChance;
        bool spawnOrange = Random.value <= orangeFoodChance;

        if (spawnPink) pinkFood.ActiveSpecialFood();
        if (spawnOrange) OrangeFood.ActiveSpecialFood();
        RestartFood();
    }

    public override void OnResetFood()
    {
        base.OnResetFood();
    }

}
