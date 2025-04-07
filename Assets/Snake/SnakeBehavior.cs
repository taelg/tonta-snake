using System.Collections;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private float moveIntervalSecs = 1f;

    private Direction facingAt = Direction.RIGHT;

    private void Start()
    {
        StartCoroutine(MovingConstantly());
    }

    private void Update()
    {
        HandMovementInput();
        UpdateSnakeHeadRotation();
    }

    private IEnumerator MovingConstantly()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveIntervalSecs);
            Move();
        }
    }

    private void Move()
    {
        Vector2 direction = facingAt.ToVector2();
        float moveInX = direction.x * 1.125f;
        float moveInY = direction.y * 1.125f;

        this.transform.position += new Vector3(moveInX, moveInY, 0);
    }

    private void HandMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            facingAt = Direction.UP;

        if (Input.GetKeyDown(KeyCode.A))
            facingAt = Direction.LEFT;

        if (Input.GetKeyDown(KeyCode.S))
            facingAt = Direction.DOWN;

        if (Input.GetKeyDown(KeyCode.D))
            facingAt = Direction.RIGHT;
    }

    private void UpdateSnakeHeadRotation()
    {
        snakeHead.transform.rotation = facingAt.ToQuaternion();
    }




}

