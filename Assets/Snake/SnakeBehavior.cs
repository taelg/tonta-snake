using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject snakeBodyPrefab;

    private LinkedList<Vector2> positionsHistory = new LinkedList<Vector2>();
    private List<Transform> snakeBodyParts = new List<Transform>();
    private Direction facingDir = Direction.RIGHT;
    private Direction lastMovedDir = Direction.NONE;

    private void Start()
    {
        UpdatePositionHistory();
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
            UpdatePositionHistory();
            MoveBodies();
        }
    }

    private void Move()
    {
        Vector2 moveDir = facingDir.ToVector2();
        Vector2Int currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = new Vector2Int(currentPos.x + (int)moveDir.x, currentPos.y + (int)moveDir.y);
        targetPos = gameGrid.MirrorPositionIfOutOfBounds(targetPos);

        if (gameGrid.IsGridCellFree(targetPos))
        {
            lastMovedDir = facingDir;

            //If is the only snake part and is leaving a 'snake_and_food' state then increase the snake.
            bool isTheOnlySnakePart = snakeBodyParts.Count == 0;
            CellState currentCellState = gameGrid.GetCellState(currentPos);
            if (isTheOnlySnakePart && currentCellState == CellState.SNAKE_AND_FOOD)
            {
                IncreaseSnakeBody();
            }

            if (isTheOnlySnakePart)
                gameGrid.ClearCellState(currentPos);

            //Move to the new position and decide the new cell state between: SNAKE_AND_FOOD or SNAKE. Was there a food?
            this.transform.position = (Vector2)targetPos;
            bool isCellStateFood = gameGrid.GetCellState(targetPos) == CellState.FOOD;
            CellState newState = isCellStateFood ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;
            gameGrid.SetCellState(newState, targetPos);
        }
        else
        {
            Debug.Log("cannot move because the cell isn't free. What is in the cell: " + gameGrid.GetCellState(targetPos));
        }
    }

    private void UpdatePositionHistory()
    {
        positionsHistory.AddFirst(transform.position);

        if (positionsHistory.Count > snakeBodyParts.Count + 10)
            positionsHistory.RemoveLast();
    }

    //Adding a lot of comments because as this method is getting bigger and doing a lot of things it will get refactored in the next commits.
    private void MoveBodies()
    {
        List<Vector2> positionHistoryList = positionsHistory.ToList();

        for (int i = 0; i < snakeBodyParts.Count; i++)
        {
            bool isLastSnakePart = i + 1 == snakeBodyParts.Count;
            Vector2Int currentPos = new Vector2Int((int)positionHistoryList[i + 2].x, (int)positionHistoryList[i + 2].y);
            Vector2Int targetPos = new Vector2Int((int)positionHistoryList[i + 1].x, (int)positionHistoryList[i + 1].y);

            //If is the last snake part and is leaving a 'snake_and_food' state then increase the snake.
            CellState currentCellState = gameGrid.GetCellState(currentPos);
            if (isLastSnakePart && currentCellState == CellState.SNAKE_AND_FOOD)
            {
                IncreaseSnakeBody();
            }

            //Clear old snake position.
            if (isLastSnakePart)
                gameGrid.ClearCellState(currentPos);

            //Move to the new position while conserving the cell state if there's a "food in the belly" there: 'snake_and_food' state.
            snakeBodyParts[i].position = (Vector2)targetPos;
            bool isCellStateSnakeAndFood = gameGrid.GetCellState(targetPos) == CellState.SNAKE_AND_FOOD;

            CellState newState = isCellStateSnakeAndFood ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;
            gameGrid.SetCellState(newState, targetPos);
        }
    }

    private void HandMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && (lastMovedDir != Direction.DOWN))
            facingDir = Direction.UP;

        if (Input.GetKeyDown(KeyCode.A) && (lastMovedDir != Direction.RIGHT))
            facingDir = Direction.LEFT;

        if (Input.GetKeyDown(KeyCode.S) && (lastMovedDir != Direction.UP))
            facingDir = Direction.DOWN;

        if (Input.GetKeyDown(KeyCode.D) && (lastMovedDir != Direction.LEFT))
            facingDir = Direction.RIGHT;
    }

    private void UpdateSnakeHeadRotation()
    {
        snakeHead.transform.rotation = facingDir.ToQuaternion();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FoodBehavior food = collision.gameObject.GetComponent<FoodBehavior>();
        if (food)
        {
            Vector2Int currentPos = new Vector2Int(
                (int)this.transform.position.x,
                (int)this.transform.position.y);

            gameGrid.SetCellState(CellState.SNAKE_AND_FOOD, currentPos);
            food.Reposition();
        }
    }

    private void IncreaseSnakeBody()
    {
        GameObject snakeBody = Instantiate(snakeBodyPrefab, snakeBodiesContainer.transform);
        snakeBodyParts.Add(snakeBody.transform);
    }



}

