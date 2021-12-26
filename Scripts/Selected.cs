using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : MonoBehaviour
{
    private void Awake() => LeanTween.scale(gameObject, transform.localScale * 1.2f, .75f).setLoopPingPong();
}
