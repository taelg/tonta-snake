using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    [Header("General Config")]
    [SerializeField] private float moveIntervalSecs = 0.35f;

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

    private List<BodyPartBehavior> bodyParts = new List<BodyPartBehavior>();
    private List<SpriteRenderer> snakeBodyPartsSprites = new List<SpriteRenderer>();
    private Transform currentTail;
    private Direction facingDir = Direction.RIGHT;
    private Direction lastMovedDir = Direction.NONE;
    private bool isBoosting = false;
    private bool alive = true;
    private int foodAteCount = 0;

    private void Start()
    {
        currentTail = this.transform;
        currentScore.text = "0";
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

            OnTailLeaveCell();
            MoveBodies();
            MoveHead();
            RecolorSnakeParts();
        }
    }

    private void OnTailLeaveCell()
    {
        Vector2 tailPos = currentTail.transform.position;
        CellState tailState = gameGrid.GetCellState(tailPos);

        if (tailState == CellState.SNAKE)
        {
            gameGrid.SetCellState(CellState.EMPTY, tailPos);
        }
        else if (tailState == CellState.SNAKE_AND_FOOD)
        {
            gameGrid.SetCellState(CellState.SNAKE, tailPos);
            IncreaseSnakeBody(tailPos);
        }

    }

    private void MoveBodies()
    {
        Vector2 snakeHeadPos = this.transform.position;
        Vector2 bodyPartTargetPos = snakeHeadPos;

        foreach (BodyPartBehavior bodyPart in bodyParts)
        {
            Vector2 bodyPartOldPos = bodyPart.transform.position;
            bodyPart.MoveTo(bodyPartTargetPos);
            bodyPartTargetPos = bodyPartOldPos;
        }
    }

    private void MoveHead()
    {
        Vector2 moveDir = facingDir.ToVector2();
        Vector2Int currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = new Vector2Int(currentPos.x + (int)moveDir.x, currentPos.y + (int)moveDir.y);
        targetPos = gameGrid.MirrorPositionIfOutOfBounds(targetPos);
        bool isTargetCellFree = gameGrid.IsGridCellFree(targetPos);

        if (isTargetCellFree)
        {
            lastMovedDir = facingDir;
            bool isThereFoodInTargetPos = gameGrid.GetCellState(targetPos) == CellState.FOOD;
            CellState targetPosNewState = isThereFoodInTargetPos ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;

            this.transform.position = (Vector2)targetPos;
            gameGrid.SetCellState(targetPosNewState, targetPos);
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        alive = false;
        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.ShowFinalScore(foodAteCount);
    }

    private void RecolorSnakeParts()
    {
        foreach (BodyPartBehavior bodyPart in bodyParts)
        {
            CellState state = gameGrid.GetCellState(bodyPart.transform.position);
            bodyPart.UpdateColor(state);
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

    private void IncreaseSnakeBody(Vector2 tailPos)
    {
        GameObject snakeBody = Instantiate(snakeBodyPrefab, snakeBodiesContainer.transform);
        bodyParts.Add(snakeBody.GetComponent<BodyPartBehavior>());
        snakeBodyPartsSprites.Add(snakeBody.GetComponent<SpriteRenderer>());
        snakeBody.transform.position = tailPos;
        currentTail = snakeBody.transform;
    }

    public void ResetSnake()
    {
        foreach (Transform child in snakeBodiesContainer.transform)
            Destroy(child.gameObject);

        currentTail = this.transform;
        currentScore.text = "0";
        this.transform.position = Vector2.zero;
        bodyParts.Clear();
        snakeBodyPartsSprites.Clear();
        facingDir = Direction.RIGHT;
        lastMovedDir = Direction.NONE;
        isBoosting = false;
        alive = true;

        StartCoroutine(MovingConstantly());
    }




}

