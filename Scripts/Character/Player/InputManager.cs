using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public enum InputType
{
    npc,
    player
}
[RequireComponent(typeof(Rigidbody))]
public class InputManager : MonoBehaviour
{
    //Select the character you want to play as!!!
    [Header("Character Type:")]
    public InputType iType;


    [Header("Main Mat:")]
    public GameObject playerCrown;
    public GameObject selected;

    #region Rigidbody:
    [Header("Rigidbody:")]
    public Rigidbody rb;
    #endregion Rigidbody />

    #region Data:
    public Joystick joystick { get; set; }
    public bool stunned { get; set; }
    public bool disabled { get; set; }
    public bool inAir { get; set; }
    #endregion data />

    public void SetAsPlayer()
    {
        CameraController.instance.player = this;
        CameraController.instance.SetPlayerTarget();
        joystick = FindObjectOfType<Joystick>();
        iType = InputType.player;
        playerCrown.SetActive(true);
        //selected.SetActive(true);
    }

    public void RemoveAsPlayer(Npc oldPlayer)
    {
        //Remove this one as player
        oldPlayer.iType = InputType.npc;
        oldPlayer.playerCrown.SetActive(false);
        //oldPlayer.selected.SetActive(false);

        SetAsPlayer();
    }
}
