using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : MonoBehaviour
{
    void Start() => LeanTween.rotateAround(gameObject, transform.up, 360, 1.25f).setLoopClamp();
}
