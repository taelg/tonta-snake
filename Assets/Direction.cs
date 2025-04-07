using UnityEngine;

public enum Direction
{
    UP,
    LEFT,
    DOWN,
    RIGHT
}

public static class DirectionExtensions
{
    public static Vector2 ToVector2(this Direction direction) => direction switch
    {
        Direction.UP => Vector2.up,
        Direction.LEFT => Vector2.left,
        Direction.DOWN => Vector2.down,
        Direction.RIGHT => Vector2.right,
        _ => throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}