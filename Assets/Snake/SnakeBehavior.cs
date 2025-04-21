using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<SpriteRenderer> bodyPartsSprites = new List<SpriteRenderer>();
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
            TryMoveHead();
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
            FoodType foodType = gameGrid.GetFoodType(tailPos);
            gameGrid.SetCellState(CellState.SNAKE, tailPos);
            OnTailLeaveFoodType(foodType, tailPos);
        }
    }

    private void OnTailLeaveFoodType(FoodType foodType, Vector2 tailPos)
    {
        //For now all food types have the same behavior, but I left this method prepared for diferent food type behaviors.
        if (foodType == FoodType.GREEN || foodType == FoodType.PINK)
            IncreaseSnakeBody(tailPos);
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

    private Vector2Int GetNextMoveTargetPos()
    {
        Vector2 moveDir = facingDir.ToVector2();
        Vector2Int currentPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = new Vector2Int(currentPos.x + (int)moveDir.x, currentPos.y + (int)moveDir.y);
        targetPos = gameGrid.MirrorPositionIfOutOfBounds(targetPos);
        return targetPos;
    }

    private void TryMoveHead()
    {
        Vector2Int targetPos = GetNextMoveTargetPos();
        bool isTargetCellFree = gameGrid.IsGridCellFree(targetPos);
        bool isHittingPinkBodyPart = isTargetCellFree == false && gameGrid.GetFoodType(targetPos) == FoodType.PINK;

        if (isTargetCellFree)
        {
            MoveHead(targetPos);
        }
        else if (isHittingPinkBodyPart)
        {
            MoveHead(targetPos);
            SplitSnakeOnSelfCollision(targetPos);
        }
        else
        {
            Die();
        }
    }

    private void MoveHead(Vector2Int targetPos)
    {
        lastMovedDir = facingDir;
        bool isThereFoodInTargetPos = gameGrid.GetCellState(targetPos) == CellState.FOOD;
        FoodType foodType = gameGrid.GetFoodType(targetPos);
        CellState targetPosNewState = isThereFoodInTargetPos ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;

        this.transform.position = (Vector2)targetPos;
        gameGrid.SetCellState(targetPosNewState, targetPos, foodType);
    }

    private void SplitSnakeOnSelfCollision(Vector2Int targetPos)
    {
        MoveHead(targetPos);
        int splitOnIndex = FindBodyPartIndexOnPos(targetPos);
        int removalSize = bodyParts.Count - splitOnIndex;

        currentTail = bodyParts[splitOnIndex - 1].transform;

        for (int i = splitOnIndex; i < bodyParts.Count; i++)
        {
            Vector2 clearPos = bodyParts[i].transform.position;
            gameGrid.ClearCellData(new Vector2Int((int)clearPos.x, (int)clearPos.y));
            Destroy(bodyParts[i].transform.gameObject);
            Destroy(bodyPartsSprites[i].transform.gameObject);
        }
        gameGrid.SetCellState(CellState.SNAKE, targetPos);

        bodyParts.RemoveRange(splitOnIndex, removalSize);
        bodyPartsSprites.RemoveRange(splitOnIndex, removalSize);
    }

    private void ClearBoardCellsOnSplitSnake(int splitOnIndex, int snakeSize)
    {
    }

    private int FindBodyPartIndexOnPos(Vector2Int pos)
    {
        int indexFound = 0;
        for (int i = 0; i < bodyParts.Count; i++)
        {
            BodyPartBehavior body = bodyParts[i];
            Vector2Int bodyPos = new Vector2Int((int)body.transform.position.x, (int)body.transform.position.y);
            if (bodyPos == pos)
            {
                indexFound = i;
                break;
            }
        }

        return indexFound;
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
            FoodType foodType = gameGrid.GetFoodType(bodyPart.transform.position);
            bodyPart.UpdateColor(state, foodType);
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
            gameGrid.SetCellState(CellState.SNAKE_AND_FOOD, currentPos, food.GetFoodType());
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
        bodyPartsSprites.Add(snakeBody.GetComponent<SpriteRenderer>());
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
        bodyPartsSprites.Clear();
        facingDir = Direction.RIGHT;
        lastMovedDir = Direction.NONE;
        isBoosting = false;
        alive = true;

        StartCoroutine(MovingConstantly());
    }




}

