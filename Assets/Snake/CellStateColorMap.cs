using UnityEngine;

[CreateAssetMenu(fileName = "CellStateColorMap", menuName = "ScriptableObjects/CellStateColorMap")]
public class CellStateColorMap : ScriptableObject
{
    [System.Serializable]
    public struct StateColorPair
    {
        public CellState state;
        public Color color;
    }

    public StateColorPair[] stateColors;

    public Color GetColor(CellState state)
    {
        foreach (var pair in stateColors)
        {
            if (pair.state == state)
                return pair.color;
        }

        return Color.white;
    }
}
