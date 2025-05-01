using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] FoodTypeData foodTypeData;
    [SerializeField] GreenFoodBehavior greenFoodBehavior;
    [SerializeField] GenericSpecialFoodPool pinkFoodPool;
    [SerializeField] GenericSpecialFoodPool orangeFoodPool;

    private void Start()
    {
        greenFoodBehavior.SetOnEatFoodCallback(OnEatGreenFoodCallback);
        greenFoodBehavior.RestartFoodLifetime();
    }

    private void OnEatGreenFoodCallback()
    {
        greenFoodBehavior.RepositionFoodOnGrid();
        RollSpecialFoodSpawn();
    }

    private void RollSpecialFoodSpawn()
    {
        bool spawnPink = Random.value <= foodTypeData.GetChance(FoodType.PINK);
        bool spawnOrange = Random.value <= foodTypeData.GetChance(FoodType.ORANGE);

        if (spawnPink) SpawnSpecialFood(pinkFoodPool.GetNext());
        if (spawnOrange) SpawnSpecialFood(orangeFoodPool.GetNext());
    }

    private void SpawnSpecialFood(SpecialFood specialFood)
    {
        specialFood.ActiveSpecialFood();
        specialFood.RepositionFoodOnGrid();
    }




}
