using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    [Header("General Config")]
    [SerializeField] private float moveIntervalSecs = 0.35f;
    [SerializeField] private int positionHistorySize = 10;
    [SerializeField] private Color defaultSnakeColor = new Color(0.2f, 0.6f, 0);
    [SerializeField] private Color snakeAndFoodColor = new Color(0.12f, 0.37f, 0);

    [Space]
    [Header("Boost Config")]
    [SerializeField] private float boostSpeedMultiplier = 4f;
    [SerializeField] private float boostDecayRate = 1.75f;
    [SerializeField] private float boostDuration = 3f;

    [Space]
    [Header("Internal")]
    [SerializeField] private GameGridBehavior gameGrid;
    [SerializeField] private SpriteRenderer headSprite;
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private GameObject snakeBodiesContainer;
    [SerializeField] private GameObject snakeBodyPrefab;
    [SerializeField] private GameOverPanelBehavior gameOverPanel;
    [SerializeField] private TMP_Text currentScore;
    [SerializeField] private WallsEffectBehavior wallFX;

    private LinkedList<Vector2> positionsHistory = new LinkedList<Vector2>();
    private List<Transform> snakeBodyParts = new List<Transform>();
    private List<SpriteRenderer> snakeBodyPartsSprites = new List<SpriteRenderer>();
    private Direction facingDir = Direction.RIGHT;
    private Direction lastMovedDir = Direction.NONE;
    private bool isBoosting = false;
    private bool alive = true;
    private int foodAteCount = 0;

    private void Start()
    {
        currentScore.text = "0";
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
            gameOverPanel.ShowFinalScore(foodAteCount);
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
            SpriteRenderer snakePartSprite = snakeBodyPartsSprites[i];
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

            if (snakePartSprite.color != defaultSnakeColor)
                snakePartSprite.color = defaultSnakeColor;

            if (isThereFoodInTargetPos)
                snakePartSprite.color = snakeAndFoodColor;

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
        wallFX.AnimateBoostEffect(boostDuration);
        float originalSpeed = moveIntervalSecs;
        float boostedSpeed = originalSpeed / boostSpeedMultiplier;
        moveIntervalSecs = boostedSpeed;

        float elapsedTime = 0f;
        Color originalColor = headSprite.color;
        headSprite.color = new Color(1f, 0.5f, 0.5f);

        while (elapsedTime < boostDuration)
        {
            elapsedTime += 0.1f;

            float progress = 1f - Mathf.Exp(-boostDecayRate * (elapsedTime / boostDuration));
            moveIntervalSecs = Mathf.Lerp(boostedSpeed, originalSpeed, progress);

            yield return new WaitForSeconds(0.1f);
        }

        isBoosting = false;
        moveIntervalSecs = originalSpeed;
        headSprite.color = originalColor;
    }

    private void UpdateSnakeHeadRotation()
    {
        snakeHead.transform.rotation = facingDir.ToQuaternion();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DefaultFoodBehavior food = collision.gameObject.GetComponent<DefaultFoodBehavior>();
        if (food)
        {
            Vector2Int currentPos = new Vector2Int(
                (int)this.transform.position.x,
                (int)this.transform.position.y);

            IncreaseFoodAteScore();
            gameGrid.SetCellState(CellState.SNAKE_AND_FOOD, currentPos);
            food.OnEatFood();
        }
    }

    private void IncreaseFoodAteScore()
    {
        currentScore.text = $"{++foodAteCount}";
    }

    private void IncreaseSnakeBody()
    {
        GameObject snakeBody = Instantiate(snakeBodyPrefab, snakeBodiesContainer.transform);
        snakeBodyParts.Add(snakeBody.transform);
        snakeBodyPartsSprites.Add(snakeBody.GetComponent<SpriteRenderer>());
    }

    public void ResetSnake()
    {
        foreach (Transform child in snakeBodiesContainer.transform)
            Destroy(child.gameObject);

        currentScore.text = "0";
        this.transform.position = Vector2.zero;
        positionsHistory.Clear();
        snakeBodyParts.Clear();
        snakeBodyPartsSprites.Clear();
        facingDir = Direction.RIGHT;
        lastMovedDir = Direction.NONE;
        isBoosting = false;
        alive = true;

        UpdatePositionHistory();
        StartCoroutine(MovingConstantly());
    }




}

