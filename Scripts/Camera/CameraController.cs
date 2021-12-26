using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton:
    public static CameraController instance;
    private void Awake() => instance = this;
    #endregion Singleton />

    [Header("Target:")]
    [SerializeField] Transform center;
    public Transform curTarget { private get; set; }
    public Transform ball { private get; set; }
    public InputManager player { get; set; }
    private void Start()
    {
        canSetTarget = true;
        curTarget = center;
    }
    [HideInInspector] public bool canSetTarget;
    public void SetPlayerTarget()
    {
        canSetTarget = false;
        curTarget = player.transform;
    }

    private void FixedUpdate()
    {
        if (canSetTarget) SetPlayerTarget();

        transform.position = Vector3.Lerp(transform.position,
            GameController.instance.gameDur ? curTarget.transform.position : center.position,
            10 * Time.deltaTime);
    }


}
