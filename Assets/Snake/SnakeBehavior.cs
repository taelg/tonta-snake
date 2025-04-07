using System.Collections;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    [Header("Configurable")]
    [SerializeField] private float moveIntervalSecs = 1f;

    [Space]
    [Header("Internal")]
    [SerializeField] private GameGridBehavior gameGrid;
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private GameObject snakeBodiesContainer;

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
        Vector2 moveDir = facingAt.ToVector2();
        Vector2Int currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = new Vector2Int(currentPos.x + (int)moveDir.x, currentPos.y + (int)moveDir.y);
        targetPos = gameGrid.MirrorPositionIfOutOfBounds(targetPos);



        if (gameGrid.IsGridCellFree(targetPos))
        {
            gameGrid.ClearCell(currentPos);
            this.transform.position = (Vector2)targetPos;
            gameGrid.OcupyCell(CellObject.SNAKE, targetPos);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger enter 2d!!!!!");

        FoodBehavior food = collision.gameObject.GetComponent<FoodBehavior>();
        if (food)
        {
            //snake.cresceDeTamanho
            food.Reposition();

        }


    }



}

