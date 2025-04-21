using UnityEngine;

public class PinkFoodBehavior : DefaultFoodBehavior
{
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        DeactiveSpecialFood();
    }

    protected override void OnLifetimeEnd()
    {
        DeactiveSpecialFood();
    }

    public override void OnEatFood()
    {
        DeactiveSpecialFood();
    }

    private void DeactiveSpecialFood()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void ActiveSpecialFood()
    {
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
        RestartFood();
    }


}
