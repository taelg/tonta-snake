using UnityEngine;

[CreateAssetMenu(fileName = "FoodTypeColorMap", menuName = "ScriptableObjects/FoodTypeColorMap")]
public class FoodTypeData : ScriptableObject
{
    [System.Serializable]
    public struct FoodTypeColorPair
    {
        public FoodType foodType;
        public Color color;
        public float chance;
        public GameObject prefab;
    }

    public FoodTypeColorPair[] foodTypeColors;

    public Color GetColor(FoodType foodType)
    {
        foreach (var pair in foodTypeColors)
        {
            if (pair.foodType == foodType)
                return pair.color;
        }

        return Color.white;
    }

    public float GetChance(FoodType foodType)
    {
        foreach (var pair in foodTypeColors)
        {
            if (pair.foodType == foodType)
                return pair.chance;
        }

        return 0f;
    }

    public GameObject GetPrefab(FoodType foodType)
    {
        foreach (var pair in foodTypeColors)
        {
            if (pair.foodType == foodType)
                return pair.prefab;
        }

        return null;
    }

}
