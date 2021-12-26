using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public enum Emotion
{
    blink,
    surprised
}
public class Expression : MonoBehaviour
{
    
    Vector3 localSize;
    float[] sizeY = new float[2];
    [Header("Emotion Type:")]
    public Emotion emotion;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            sizeY[i] = transform.GetChild(i).localScale.y;
        }
        if (emotion == Emotion.blink) Timing.RunCoroutine(_Blink().CancelWith(gameObject));
        else
        {
            localSize = transform.localScale;
            localSize.z = 0;
        }
    }

    IEnumerator<float> _Blink()
    {
        float blinkTime = Random.Range(1.0f, 3.25f);
        yield return Timing.WaitForSeconds(blinkTime);

        foreach (Transform c in transform)
            LeanTween.scaleY(c.gameObject, 0, 0.5f).setEaseInBack();
        yield return Timing.WaitForSeconds(0.5f);

        for (int i = 0; i < transform.childCount; i++)
            LeanTween.scaleY(transform.GetChild(i).gameObject, sizeY[i], .5f).setEaseOutBack();

        Timing.RunCoroutine(_Blink().CancelWith(gameObject));
    }

    #region Surprise:
    public void Wow()
    {
        Timing.RunCoroutine(_Wow().CancelWith(gameObject));
    }
    IEnumerator<float> _Wow()
    {
        LeanTween.scale(gameObject, localSize * 5f, .5f);
        yield return Timing.WaitForSeconds(.5f);
        LeanTween.scale(gameObject, localSize, .5f);
    }
    #endregion
}
