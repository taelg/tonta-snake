using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] FoodTypeData foodTypeData;
    [SerializeField] GreenFoodBehavior greenFood;
    [SerializeField] GenericSpecialFoodPool pinkFoodPool;
    [SerializeField] GenericSpecialFoodPool orangeFoodPool;
    [SerializeField] GenericSpecialFoodPool redFoodPool;

    private void Start()
    {
        greenFood.SetOnEatFoodCallback(OnEatGreenFoodCallback);
        greenFood.RestartFoodLifetime();
    }

    private void OnEatGreenFoodCallback()
    {
        greenFood.RepositionFoodOnGrid();
        RollSpecialFoodSpawn();
    }

    private void RollSpecialFoodSpawn()
    {
        bool spawnPink = Random.value <= foodTypeData.GetChance(FoodType.PINK);
        bool spawnOrange = Random.value <= foodTypeData.GetChance(FoodType.ORANGE);
        bool spawnRed = Random.value <= foodTypeData.GetChance(FoodType.RED);

        if (spawnPink) SpawnSpecialFood(pinkFoodPool.GetNext());
        if (spawnOrange) SpawnSpecialFood(orangeFoodPool.GetNext());
        if (spawnRed) SpawnSpecialFood(redFoodPool.GetNext());
    }

    private void SpawnSpecialFood(SpecialFood specialFood)
    {
        specialFood.ActiveSpecialFood();
        specialFood.RepositionFoodOnGrid();
    }

    public void ResetAllFoods()
    {
        greenFood.RestartFoodLifetime();
        pinkFoodPool.ResetAllObjects();
        orangeFoodPool.ResetAllObjects();
        redFoodPool.ResetAllObjects();
    }




}
