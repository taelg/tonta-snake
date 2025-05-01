using UnityEngine;

public class SpecialFood : DefaultFoodBehavior
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
        base.OnEatFood();
        DeactiveSpecialFood();
    }

    public void DeactiveSpecialFood()
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
