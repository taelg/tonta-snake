using System.Collections;
using UnityEngine;

public class MusicEffectsBehavior : MonoBehaviour
{
    [SerializeField] private AudioSourcePoolBehavior musicSourcePool;
    [SerializeField] private float pitchOnBoost = 1.2f;

    public void SetMusicPitch(float pitch)
    {
        foreach (AudioSource source in musicSourcePool.GetAllObjects())
            source.pitch = pitch;
    }

    public void AnimateBoostEffect(float boostDuration)
    {
        SetMusicPitch(pitchOnBoost);
        StartCoroutine(EndBoostFXAfterDelay(boostDuration));
    }

    private IEnumerator EndBoostFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetMusicPitch(1f);
    }

}
