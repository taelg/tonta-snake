using UnityEngine;

[CreateAssetMenu(fileName = "FoodTypeColorMap", menuName = "ScriptableObjects/FoodTypeColorMap")]
public class FoodTypeColorMap : ScriptableObject
{
    [System.Serializable]
    public struct FoodTypeColorPair
    {
        public FoodType foodType;
        public Color color;
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
}
