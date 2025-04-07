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

    public static Quaternion ToQuaternion(this Direction direction) => direction switch
    {
        Direction.UP => Quaternion.Euler(0, 0, 90),
        Direction.LEFT => Quaternion.Euler(0, 0, 180),
        Direction.DOWN => Quaternion.Euler(0, 0, -90),
        Direction.RIGHT => Quaternion.Euler(0, 0, 0),
        _ => throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}