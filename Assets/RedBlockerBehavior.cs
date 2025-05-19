using System;
using System.Collections;
using UnityEngine;

public class RedBlockerBehavior : MonoBehaviour
{

    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private GameObject crackMask;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color shakingColor;
    private bool shaking = false;

    public void Break(Action<RedBlockerBehavior> breakCallback)
    {
        StartCoroutine(AnimateBreaking(breakCallback));
    }

    private IEnumerator AnimateBreaking(Action<RedBlockerBehavior> breakCallback)
    {
        float duration = 0.35f;
        float elapsedTime = 0f;
        Color color = sprite.color;
        Color startColor = new Color(color.r, color.g, color.b, 1);
        Color endColor = new Color(color.r, color.g, color.b, 0.2f);
        float startScale = 1f;
        float endScale = 1.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            sprite.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            gameObject.transform.localScale = Vector3.Lerp(new Vector3(startScale, startScale, 1), new Vector3(endScale, endScale, 1), elapsedTime / duration);
            yield return null;
        }

        sprite.color = color;
        gameObject.SetActive(false);
        breakCallback?.Invoke(this);
    }

    public void StartShaking()
    {
        shaking = true;
        sprite.color = shakingColor;
        crackMask.SetActive(true);
        StartCoroutine(Shake());
    }

    public void StopShaking()
    {
        crackMask.SetActive(false);
        shaking = false;
        sprite.color = defaultColor;
    }

    private IEnumerator Shake()
    {
        Vector3 originalPosition = transform.position;

        while (shaking)
        {
            float xOffset = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            float yOffset = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

            yield return null;
            yield return null;
        }

        transform.position = originalPosition;
    }




}
