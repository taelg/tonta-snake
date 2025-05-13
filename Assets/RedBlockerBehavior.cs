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
