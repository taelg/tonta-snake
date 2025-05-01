using UnityEngine;

public class SpecialFood : DefaultFoodBehavior
{
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer spriteRenderer;

    protected override void EndLifeTime()
    {
        GameGridBehavior.Instance.ClearCellData(this.transform.position);
        DeactiveSpecialFood();
    }

    public override void OnEatFood()
    {
        base.OnEatFood();
        DeactiveSpecialFood();
    }

    public void DeactiveSpecialFood()
    {
        this.gameObject.SetActive(false);
    }

    public void ActiveSpecialFood()
    {
        this.gameObject.SetActive(true);
        RestartFoodLifetime();
    }


}
