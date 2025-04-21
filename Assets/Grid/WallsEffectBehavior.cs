using System.Collections;
using UnityEngine;

public class WallsEffectBehavior : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField] private float defaultScrollSpeed = 0.1f;
    [SerializeField] private Color defaultColor1;
    [SerializeField] private Color defaultColor2;
    [SerializeField] private Color defaultColor3;

    [Space]
    [Header("Snake Boost Speed FX")]
    [SerializeField] private float boostingScrollSpeed;
    [SerializeField] private Color boostingColor3;

    [Space]
    [Header("Snake Pink Food FX")]
    [SerializeField] private Color pinkFoodColor2;

    [Space]
    [Header("Internal")]
    [SerializeField] private Material teleportMaterial;

    private void Start()
    {
        ResetToDefaults();
    }

    private void ResetToDefaults()
    {
        SetScrollSpeed(defaultScrollSpeed);
        SetColor(1, defaultColor1);
        SetColor(2, defaultColor2);
        SetColor(3, defaultColor3);
    }

    public void AnimateBoostEffect(float boostDuration)
    {
        SetScrollSpeed(0.75f);
        SetColor(3, boostingColor3);
        StartCoroutine(EndBoostFXAfterDelay(boostDuration));
    }

    private IEnumerator EndBoostFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndBoostFX();
    }

    private void EndBoostFX()
    {
        SetScrollSpeed(defaultScrollSpeed);
        SetColor(3, defaultColor3);

    }

    public void StartPinkFoodEffect()
    {
        SetColor(2, pinkFoodColor2);
    }

    public void EndPinkFoodEffect()
    {
        SetColor(2, defaultColor2);
    }

    private void SetScrollSpeed(float speed)
    {
        int scrollSpeedProperty = Shader.PropertyToID("_ScrollSpeed");
        teleportMaterial.SetFloat(scrollSpeedProperty, speed);
    }

    private void SetColor(int colorId, Color color)
    {
        int color1Property = Shader.PropertyToID("_MainColor");
        int color2Property = Shader.PropertyToID("_Color2");
        int color3Property = Shader.PropertyToID("_Color3");

        switch (colorId)
        {
            case 1:
                teleportMaterial.SetColor(color1Property, color);
                break;
            case 2:
                teleportMaterial.SetColor(color2Property, color);
                break;
            case 3:
                teleportMaterial.SetColor(color3Property, color);
                break;
        }
    }

}