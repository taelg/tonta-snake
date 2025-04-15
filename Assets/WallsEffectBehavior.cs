using System.Collections;
using UnityEngine;

public class WallsEffectBehavior : MonoBehaviour
{
    [SerializeField] private Material teleportMaterial;
    [SerializeField] private float boostingScrollSpeed;
    [SerializeField] private Color boostingThirdColor;

    private float originalScrollSpeed;
    private Color originalThirdColor;

    private void Start()
    {
        CacheOriginalValues();
    }

    private void CacheOriginalValues()
    {
        int scrollSpeedProperty = Shader.PropertyToID("_ScrollSpeed");
        int thirdColorProperty = Shader.PropertyToID("_Color3");
        originalScrollSpeed = teleportMaterial.GetFloat(scrollSpeedProperty);
        originalThirdColor = teleportMaterial.GetColor(thirdColorProperty);
    }

    public void AnimateBoostEffect(float boostDuration)
    {
        SetTeleportMaterialScrollSpeed(0.75f);
        SetTeleportMaterialThirdColor(boostingThirdColor);
        StartCoroutine(ResetFXAfterDelay(boostDuration * 0.75f));

    }

    private IEnumerator ResetFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SetTeleportMaterialScrollSpeed(0.1f);
        SetTeleportMaterialThirdColor(originalThirdColor);
    }


    private void SetTeleportMaterialScrollSpeed(float speed)
    {
        int scrollSpeedProperty = Shader.PropertyToID("_ScrollSpeed");
        teleportMaterial.SetFloat(scrollSpeedProperty, speed);
    }

    private void SetTeleportMaterialThirdColor(Color color)
    {
        int thirdColorProperty = Shader.PropertyToID("_Color3");
        teleportMaterial.SetColor(thirdColorProperty, color);

    }

}