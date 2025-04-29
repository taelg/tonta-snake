using System.Collections;
using UnityEngine;

public class PointVFXBehavior : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    [Space]
    [Header("Internal")]
    [SerializeField] private Vector2 targetPos;

    public void AnimatePoint(Vector2 targetPos)
    {
        this.targetPos = targetPos;
        StartCoroutine(AnimatePointCoroutine());
    }

    void OnEnable()
    {
        StartCoroutine(AnimatePointCoroutine());
    }

    private IEnumerator AnimatePointCoroutine()
    {
        Vector2 startPos = transform.position;
        Vector2 curveDirection = Random.value < 0.5f ? Vector2.right : Vector2.left;
        float distanceFromTarget = Vector2.Distance(startPos, targetPos);
        float curveHeight = distanceFromTarget / 3f;

        Vector2 controlPoint = (startPos + targetPos) / 2f + (curveDirection * curveHeight);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = movementCurve.Evaluate(Mathf.Clamp01(time / duration));

            Vector3 curvedPos = Mathf.Pow(1 - t, 2) * startPos +
                                2 * (1 - t) * t * controlPoint +
                                Mathf.Pow(t, 2) * targetPos;

            transform.position = curvedPos;

            yield return null;
        }

        transform.position = targetPos;
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }
}
