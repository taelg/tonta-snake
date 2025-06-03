using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    [Header("Boost Config")]
    [SerializeField] private bool lockMovementInput = false;
    [Space]
    [Header("Boost Config")]
    [SerializeField] private float boostSpeedMultiplier = 4f;
    [SerializeField] private float boostDecayRate = 1.75f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private Color boostingColor = Color.red;

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
    [SerializeField] private RedBlockerPool redBlockerPool;

    public float moveIntervalSecs = 0.175f;
    private List<BodyPartBehavior> bodyParts = new List<BodyPartBehavior>();
    private List<SpriteRenderer> bodyPartsSprites = new List<SpriteRenderer>();
    private Dictionary<Vector2, RedBlockerBehavior> activeRedBlockers = new Dictionary<Vector2, RedBlockerBehavior>();
    private Transform currentTail;
    private Direction lockedFacingDir = Direction.NONE;
    private Direction facingDir = Direction.RIGHT;
    private Direction lastMovedDir = Direction.NONE;
    private Coroutine boostingCoroutine = null;
    private bool isBoosting = false;
    private bool alive = true;
    private int snakePoints = 0;
    private float boostOriginalMoveIntervalSecs;
    private Color boostOriginalHeadColor;

    private void Start()
    {
        LoadSpeedFromPlayerPrefs();
        LoadInputMethodFromPlayerPrefs();
        boostOriginalMoveIntervalSecs = moveIntervalSecs;
        boostOriginalHeadColor = headSprite.color;

        currentTail = this.transform;
        StartCoroutine(MovingConstantly());
    }

    private void LoadSpeedFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("SnakeSpeed")) return;
        SetMovementSpeed(PlayerPrefs.GetInt("SnakeSpeed"));
    }

    private void LoadInputMethodFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("AlternativeInput")) return;
        SetAlternativeInput(PlayerPrefs.GetInt("AlternativeInput") == 1);
    }

    private void Update()
    {
        HandleMovementInput();
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

        if (foodType != FoodType.RED)
            IncreaseSnakeBody(tailPos);

        if (foodType == FoodType.PINK)
            wallFX.EndPinkFoodEffect();

        if (foodType == FoodType.RED)
            AddRedBlocker(tailPos);
    }

    private void AddRedBlocker(Vector2 tailPos)
    {
        RedBlockerBehavior redBlocker = redBlockerPool.GetNext();
        activeRedBlockers.Add(tailPos, redBlocker);
        redBlocker.transform.position = tailPos;
        redBlocker.gameObject.SetActive(true);
        gameGrid.SetCellState(CellState.RED_BLOCKER, tailPos);

        if (isBoosting)
            redBlocker.StartShaking();
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

        if (lockMovementInput && lockedFacingDir != Direction.NONE)
            moveDir = lockedFacingDir.ToVector2();

        Vector2 currentPos = new Vector2((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2 targetPos = new Vector2(currentPos.x + (int)moveDir.x, currentPos.y + (int)moveDir.y);
        targetPos = gameGrid.MirrorPositionIfOutOfBounds(targetPos);
        return targetPos;
    }

    private void TryMoveHead()
    {
        Vector2 targetPos = GetNextMoveTargetPos();
        lockedFacingDir = Direction.NONE;
        bool isTargetCellFree = gameGrid.IsGridCellFree(targetPos);
        bool isHittingPinkBodyPart = isTargetCellFree == false && gameGrid.GetFoodType(targetPos) == FoodType.PINK;
        bool isBreakingRedBlocker = gameGrid.GetCellState(targetPos) == CellState.RED_BLOCKER && isBoosting;

        if (isTargetCellFree)
        {
            MoveHead(targetPos);
        }
        else if (isHittingPinkBodyPart)
        {
            MoveHead(targetPos);
            SplitSnakeOnSelfCollision(targetPos);
        }
        else if (isBreakingRedBlocker)
        {
            MoveHead(targetPos);
            BreakRedBlocker(targetPos);
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

    private void AddAndAnimatePoint()
    {
        snakePoints++;
        scoreLabel.UpdateScoreDelayed(snakePoints);
        VFXManager.Instance.AnimatePoint(this.transform.position);
    }

    private void SplitSnakeOnSelfCollision(Vector2 targetPos)
    {
        MoveHead(targetPos);
        AddAndAnimatePoint();
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

    private void BreakRedBlocker(Vector2 targetPos)
    {
        AddAndAnimatePoint();
        RedBlockerBehavior redBlocker = activeRedBlockers[targetPos];
        activeRedBlockers.Remove(targetPos);
        redBlocker.Break(BreakCallback);
        AudioManager.Instance.PlayOneShot(AudioId.RED_BLOCKER_BREAK, AudioType.EFFECT);
    }

    private void BreakCallback(RedBlockerBehavior redBlocker)
    {
        redBlockerPool.ReturnObjectToPool(redBlocker);
        redBlocker.gameObject.SetActive(false);
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
        gameOverPanel.ShowFinalScore(snakePoints);
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

    private void HandleMovementInput()
    {
        if (lockMovementInput && lockedFacingDir != Direction.NONE)
            return;

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool inputedMovement = false;

        if (vertical > 0 && (lastMovedDir != Direction.DOWN))
        {
            facingDir = Direction.UP;
            inputedMovement = true;
        }

        if (horizontal < 0 && (lastMovedDir != Direction.RIGHT))
        {
            facingDir = Direction.LEFT;
            inputedMovement = true;
        }

        if (vertical < 0 && (lastMovedDir != Direction.UP))
        {
            facingDir = Direction.DOWN;
            inputedMovement = true;
        }

        if (horizontal > 0 && (lastMovedDir != Direction.LEFT))
        {
            facingDir = Direction.RIGHT;
            inputedMovement = true;
        }

        if (lockMovementInput && inputedMovement)
            lockedFacingDir = facingDir;
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

        boostOriginalMoveIntervalSecs = moveIntervalSecs;
    }

    public void BoostSnakeSpeed()
    {
        if (boostingCoroutine != null)
        {
            StopCoroutine(boostingCoroutine);
            EndBoostEffect(boostOriginalMoveIntervalSecs, boostOriginalHeadColor);
        }

        boostOriginalMoveIntervalSecs = moveIntervalSecs;
        boostOriginalHeadColor = headSprite.color;
        boostingCoroutine = StartCoroutine(ExecuteSpeedBoost(boostOriginalMoveIntervalSecs, boostOriginalHeadColor));
        AudioManager.Instance.PlayOneShot(AudioId.SNAKE_BOOST, AudioType.EFFECT);
    }

    private IEnumerator ExecuteSpeedBoost(float originalSpeed, Color originalHeadColor)
    {
        isBoosting = true;
        wallFX.AnimateBoostEffect(boostDuration);
        musicFX.AnimateBoostEffect(boostDuration);
        float boostedSpeed = originalSpeed / boostSpeedMultiplier;
        moveIntervalSecs = boostedSpeed;

        float elapsedTime = 0f;
        headSprite.color = boostingColor;
        float animationInterval = 0.1f;

        while (elapsedTime < boostDuration)
        {
            float progress = 1f - Mathf.Exp(-boostDecayRate * (elapsedTime / boostDuration));
            moveIntervalSecs = Mathf.Lerp(boostedSpeed, originalSpeed, progress);

            if (progress >= 0.8f)
                BlinkColorIndicatingBoostIsEnding(originalHeadColor);


            elapsedTime += animationInterval;
            yield return new WaitForSeconds(animationInterval);
        }
        EndBoostEffect(originalSpeed, originalHeadColor);
    }

    private void BlinkColorIndicatingBoostIsEnding(Color originalHeadColor)
    {
        if (headSprite.color == boostingColor)
            headSprite.color = originalHeadColor;
        else
            headSprite.color = boostingColor;
    }

    private void EndBoostEffect(float originalSpeed, Color originalHeadColor)
    {
        isBoosting = false;
        StopShakingRedBlockers();
        moveIntervalSecs = originalSpeed;
        headSprite.color = originalHeadColor;
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

            gameGrid.SetCellState(CellState.SNAKE_AND_FOOD, currentPos, food.GetFoodType());
            food.OnEatFood();
            OnSnakeEatFood(food.GetFoodType());
        }
    }

    private void OnSnakeEatFood(FoodType foodType)
    {
        snakePoints++;
        scoreLabel.UpdateScoreDelayed(snakePoints);
        AudioManager.Instance.PlayOneShot(AudioId.SNAKE_EAT, AudioType.EFFECT);
        if (foodType == FoodType.PINK)
            wallFX.StartPinkFoodEffect();
        else if (foodType == FoodType.ORANGE)
        {
            BoostSnakeSpeed();
            ShakeRedBlockers();
        }
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

    private void ShakeRedBlockers()
    {
        foreach (KeyValuePair<Vector2, RedBlockerBehavior> kvp in activeRedBlockers)
            kvp.Value.StartShaking();
    }

    private void StopShakingRedBlockers()
    {
        foreach (KeyValuePair<Vector2, RedBlockerBehavior> kvp in activeRedBlockers)
            kvp.Value.StopShaking();
    }

    public void ResetSnake()
    {
        foreach (BodyPartBehavior part in bodyParts)
            part.gameObject.SetActive(false);

        activeRedBlockers.Clear();
        redBlockerPool.ResetAllObjects();
        currentTail = this.transform;
        snakePoints = 0;
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

    public void SetAlternativeInput(bool value)
    {
        lockMovementInput = value;
    }



}

