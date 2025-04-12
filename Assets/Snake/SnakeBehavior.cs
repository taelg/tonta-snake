using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    [Header("General Config")]
    [SerializeField] private float moveIntervalSecs = 0.35f;
    [SerializeField] private int positionHistorySize = 10;
    [SerializeField] private Color defaulSnakeColor = new Color(0.2f, 0.6f, 0);
    [SerializeField] private Color snakeAndFoodColor = new Color(0.12f, 0.37f, 0);

    [Space]
    [Header("Boost Config")]
    [SerializeField] private float boostSpeedMultiplier = 4f;
    [SerializeField] private float boostDecayRate = 1.75f;
    [SerializeField] private float boostDuration = 3f;

    [Space]
    [Header("Internal")]
    [SerializeField] private GameGridBehavior gameGrid;
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private GameObject snakeBodiesContainer;
    [SerializeField] private GameObject snakeBodyPrefab;
    [SerializeField] private GameOverPanelBehavior gameOverPanel;

    private LinkedList<Vector2> positionsHistory = new LinkedList<Vector2>();
    private List<Transform> snakeBodyParts = new List<Transform>();
    private Direction facingDir = Direction.RIGHT;
    private Direction lastMovedDir = Direction.NONE;
    private bool isBoosting = false;
    private bool alive = true;
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
        while (alive)
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
        bool isTargetCellFree = gameGrid.IsGridCellFree(targetPos);

        if (isTargetCellFree)
        {
            lastMovedDir = facingDir;
            bool isTheLastSnakePart = snakeBodyParts.Count == 0;
            CellState currentCellState = gameGrid.GetCellState(currentPos);
            bool shouldIncreaseNow = isTheLastSnakePart && currentCellState == CellState.SNAKE_AND_FOOD;
            bool isThereFoodInTargetPos = gameGrid.GetCellState(targetPos) == CellState.FOOD;
            CellState targetPosNewState = isThereFoodInTargetPos ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;

            if (shouldIncreaseNow)
                IncreaseSnakeBody();

            if (isTheLastSnakePart)
                gameGrid.ClearCellState(currentPos);

            this.transform.position = (Vector2)targetPos;
            gameGrid.SetCellState(targetPosNewState, targetPos);
        }
        else
        {//To reach this else the snake has collided with itself.
            alive = false;
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.ShowFinalScore(snakeBodyParts.Count + 1);
        }
    }

    private void UpdatePositionHistory()
    {
        positionsHistory.AddFirst(transform.position);

        bool posHistoryNeedTrim = positionsHistory.Count > snakeBodyParts.Count + positionHistorySize;
        if (posHistoryNeedTrim)
            positionsHistory.RemoveLast();
    }

    private void MoveBodies()
    {
        List<Vector2> positionHistoryList = positionsHistory.ToList();

        for (int i = 0; i < snakeBodyParts.Count; i++)
        {
            Transform snakePart = snakeBodyParts[i];
            bool isTheLastSnakePart = i + 1 == snakeBodyParts.Count;
            Vector2Int currentPos = new Vector2Int((int)positionHistoryList[i + 2].x, (int)positionHistoryList[i + 2].y);
            Vector2Int targetPos = new Vector2Int((int)positionHistoryList[i + 1].x, (int)positionHistoryList[i + 1].y);
            CellState currentCellState = gameGrid.GetCellState(currentPos);
            bool shouldIncreaseNow = isTheLastSnakePart && currentCellState == CellState.SNAKE_AND_FOOD;
            bool isThereFoodInTargetPos = gameGrid.GetCellState(targetPos) == CellState.SNAKE_AND_FOOD;
            CellState targetPosNewState = isThereFoodInTargetPos ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;

            if (shouldIncreaseNow)
                IncreaseSnakeBody();

            if (isTheLastSnakePart)
                gameGrid.ClearCellState(currentPos);

            snakePart.GetComponent<SpriteRenderer>().color = defaulSnakeColor;
            if (isThereFoodInTargetPos)
                snakePart.GetComponent<SpriteRenderer>().color = snakeAndFoodColor;

            snakePart.position = (Vector2)targetPos;
            gameGrid.SetCellState(targetPosNewState, targetPos);
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

        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting)
            StartCoroutine(ExecuteSpeedBoost());
    }

    private IEnumerator ExecuteSpeedBoost()
    {
        isBoosting = true;
        float originalSpeed = moveIntervalSecs;
        float boostedSpeed = originalSpeed / boostSpeedMultiplier;
        moveIntervalSecs = boostedSpeed;

        float elapsedTime = 0f;
        Color originalColor = snakeHead.GetComponent<SpriteRenderer>().color;
        snakeHead.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f);

        while (elapsedTime < boostDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = 1f - Mathf.Exp(-boostDecayRate * (elapsedTime / boostDuration));
            moveIntervalSecs = Mathf.Lerp(boostedSpeed, originalSpeed, progress);

            yield return null;
        }

        isBoosting = false;
        moveIntervalSecs = originalSpeed;
        snakeHead.GetComponent<SpriteRenderer>().color = originalColor;
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

    public void ResetSnake()
    {
        foreach (Transform child in snakeBodiesContainer.transform)
            Destroy(child.gameObject);

        this.transform.position = Vector2.zero;
        positionsHistory.Clear();
        snakeBodyParts.Clear();
        facingDir = Direction.RIGHT;
        lastMovedDir = Direction.NONE;
        isBoosting = false;
        alive = true;

        UpdatePositionHistory();
        StartCoroutine(MovingConstantly());
    }



}

