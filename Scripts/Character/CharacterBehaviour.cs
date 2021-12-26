using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : InputManager
{
    //Contains character data!!!
    /*
     * Data:
    Hp, Stun Time, Push Force, Move Speed, Rotation Speed (?)
     */
    [Header("Main Data:")]
    public int hp;
    public float stunDur;
    public float bounceForce;
    public float moveSpeed;
    [Space]
    public float rotSpeed;
}
