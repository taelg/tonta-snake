using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
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
    [SerializeField] private ScoreLabelBehavior scoreLabel;
    [SerializeField] private WallsEffectBehavior wallFX;
    [SerializeField] private MusicEffectsBehavior musicFX;
    [SerializeField] private GameObjectPoolBehavior snakePartsPool;

    private float moveIntervalSecs = 0.175f;
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
        StartCoroutine(MovingConstantly());
    }

    private void Update()
    {
        HandMovementInput();
        HandleBoostInput();
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
        if (foodType == FoodType.NONE)
            return;

        IncreaseSnakeBody(tailPos);

        if (foodType == FoodType.PINK)
            wallFX.EndPinkFoodEffect();
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

    private Vector2 GetNextMoveTargetPos()
    {
        Vector2 moveDir = facingDir.ToVector2();
        Vector2 currentPos = new Vector2((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2 targetPos = new Vector2(currentPos.x + (int)moveDir.x, currentPos.y + (int)moveDir.y);
        targetPos = gameGrid.MirrorPositionIfOutOfBounds(targetPos);
        return targetPos;
    }

    private void TryMoveHead()
    {
        Vector2 targetPos = GetNextMoveTargetPos();
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

    private void MoveHead(Vector2 targetPos)
    {
        lastMovedDir = facingDir;
        bool isThereFoodInTargetPos = gameGrid.GetCellState(targetPos) == CellState.FOOD;
        FoodType foodType = gameGrid.GetFoodType(targetPos);
        CellState targetPosNewState = isThereFoodInTargetPos ? CellState.SNAKE_AND_FOOD : CellState.SNAKE;

        this.transform.position = (Vector2)targetPos;
        gameGrid.SetCellState(targetPosNewState, targetPos, foodType);
    }

    private void SplitSnakeOnSelfCollision(Vector2 targetPos)
    {
        MoveHead(targetPos);
        int splitOnIndex = FindBodyPartIndexOnPos(targetPos);
        int removalSize = bodyParts.Count - splitOnIndex;

        currentTail = bodyParts[splitOnIndex - 1].transform;

        ClearBoardCellsOnSplitSnake(splitOnIndex);
        gameGrid.SetCellState(CellState.SNAKE, targetPos);

        AudioManager.Instance.PlayOneShot(AudioId.SNAKE_SPLIT, AudioType.EFFECT);
        AnimateDestoyedBodyParts(splitOnIndex);
        bodyParts.RemoveRange(splitOnIndex, removalSize);
        bodyPartsSprites.RemoveRange(splitOnIndex, removalSize);
        wallFX.EndPinkFoodEffect();
    }

    private void ClearBoardCellsOnSplitSnake(int splitOnIndex)
    {
        for (int i = splitOnIndex; i < bodyParts.Count; i++)
        {
            Vector2 clearPos = bodyParts[i].transform.position;
            gameGrid.ClearCellData(new Vector2((int)clearPos.x, (int)clearPos.y));
        }
    }

    private void AnimateDestoyedBodyParts(int splitOnIndex)
    {
        for (int i = splitOnIndex; i < bodyParts.Count; i++)
            bodyParts[i].AnimateDestroyPink();
    }

    private int FindBodyPartIndexOnPos(Vector2 pos)
    {
        int indexFound = 0;
        for (int i = 0; i < bodyParts.Count; i++)
        {
            BodyPartBehavior body = bodyParts[i];
            Vector2 bodyPos = new Vector2((int)body.transform.position.x, (int)body.transform.position.y);
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
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical > 0 && (lastMovedDir != Direction.DOWN))
            facingDir = Direction.UP;

        if (horizontal < 0 && (lastMovedDir != Direction.RIGHT))
            facingDir = Direction.LEFT;

        if (vertical < 0 && (lastMovedDir != Direction.UP))
            facingDir = Direction.DOWN;

        if (horizontal > 0 && (lastMovedDir != Direction.LEFT))
            facingDir = Direction.RIGHT;
    }

    private void HandleBoostInput()
    {
        bool inputBoost = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0");

        if (inputBoost && !isBoosting)
        {
            StartCoroutine(ExecuteSpeedBoost());
        }
    }

    public void SetMovementSpeed(int speedId1to5)
    {
        switch (speedId1to5)
        {
            case 1:
                moveIntervalSecs = 0.35f;
                break;
            case 2:
                moveIntervalSecs = 0.25f;
                break;
            case 3:
                moveIntervalSecs = 0.175f;
                break;
            case 4:
                moveIntervalSecs = 0.125f;
                break;
            case 5:
                moveIntervalSecs = 0.1f;
                break;
        }
    }


    private IEnumerator ExecuteSpeedBoost()
    {
        isBoosting = true;
        wallFX.AnimateBoostEffect(boostDuration);
        musicFX.AnimateBoostEffect(boostDuration);
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
            Vector2 currentPos = new Vector2(
                (int)this.transform.position.x,
                (int)this.transform.position.y);

            IncreaseFoodAteScore();
            gameGrid.SetCellState(CellState.SNAKE_AND_FOOD, currentPos, food.GetFoodType());
            food.OnEatFood();
            OnSnakeEatFood(food.GetFoodType());
        }
    }

    private void OnSnakeEatFood(FoodType foodType)
    {
        AudioManager.Instance.PlayOneShot(AudioId.SNAKE_EAT, AudioType.EFFECT);
        if (foodType == FoodType.PINK)
            wallFX.StartPinkFoodEffect();
    }

    private void IncreaseFoodAteScore()
    {
        foodAteCount++;
        scoreLabel.UpdateScoreDelayed(foodAteCount);
    }

    private void IncreaseSnakeBody(Vector2 tailPos)
    {
        GameObject snakeBody = snakePartsPool.GetPooledObject();
        snakeBody.transform.parent = snakeBodiesContainer.transform;
        bodyParts.Add(snakeBody.GetComponent<BodyPartBehavior>());
        bodyPartsSprites.Add(snakeBody.GetComponent<SpriteRenderer>());
        snakeBody.transform.position = tailPos;
        currentTail = snakeBody.transform;
    }

    public void ResetSnake()
    {
        foreach (BodyPartBehavior part in bodyParts)
            part.gameObject.SetActive(false);

        currentTail = this.transform;
        foodAteCount = 0;
        scoreLabel.ResetScore();
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

