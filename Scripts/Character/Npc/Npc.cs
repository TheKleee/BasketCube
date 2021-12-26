using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public enum Team
{
    ally,
    enemy
}
public class Npc : CharacterBehaviour
{
    [Header("Grab Point:")]
    public Transform grabPoint;   //Where you'll carry stuff :/
    //public bool isCarrying { get; set; }
    public Team team { get; set; }
    int startHp;
    //Target Priority:
    //Priority list: Ball (based on distance || catapult done?), sabotage (bump enemies), catapult
    [Header("Npc mat:")]
    public Material[] npcMats;  //Red and Green! :)

    #region Targets:
    //Player:
    Transform targetLookAt; //Hope this works >xD
    //Npc:
    public List<Transform> npcTargetList = new List<Transform>();
    public List<Transform> cataPartsList = new List<Transform>();   //Use later just to pick up the cata parts xD
    float npcTargetDistance = 100;    //Get the closest target from the list
    [Header("NPC TARGET:")]
    [SerializeField] Transform npcTarget;
    public Catapult npcCatapult { get; set; }
    #endregion targets />

    Camera cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        targetLookAt = transform.GetChild(0);
        startHp = hp;
        Timing.RunCoroutine(_SetNpcTarget().CancelWith(gameObject));
        if (iType == InputType.npc)
        {
            GetComponent<Renderer>().material = team == Team.enemy ? npcMats[0] : npcMats[1];
            bounceForce = team == Team.enemy ? bounceForce / 2f : bounceForce;
        }
            rb = rb == null ? GetComponent<Rigidbody>() : rb;
        //Npc[] testNpcList = FindObjectsOfType<Npc>();
        //for (int i = 0; i < testNpcList.Length; i++)
        //    targetList.Add(testNpcList[i].transform);
    }
    private void Update()
    {
        if(!GameController.instance.gamePaused)
            if (!GameController.instance.gameEnded)
            {
                if (!GameController.instance.gameDur && iType == InputType.player)
                {
                    //Select the green character:
    #if UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray player = cam.ScreenPointToRay(Input.mousePosition);
    #elif UNITY_ANDROID
                if (Input.touchCount > 0)
                { 
                    Ray player = cam.ScreenPointToRay(Input.touches[0].position);
    #endif
                        if (Physics.Raycast(player, out RaycastHit hit))
                        {
                            if (hit.transform.CompareTag("Character") && hit.transform.GetComponent<InputManager>() != null)
                            {
                                if (hit.transform.GetComponent<Npc>().team == Team.ally)
                                    hit.transform.GetComponent<InputManager>().RemoveAsPlayer(this);
                            }
                        }
                    }
                    if (Mathf.Abs(joystick.Horizontal) > 0.2f || Mathf.Abs(joystick.Vertical) > 0.2f)
                        GameController.instance.StartGame();
                    return;
                }

                else if (GameController.instance.gameDur && !stunned)
                {
                    if (!disabled)
                    {
                        //Rotate... npc = towards target, player = based on joystick (like shortcut run)
                        if (iType == InputType.player)
                        {
                            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
                            {
                                targetLookAt.transform.localPosition =
                                    new Vector3(joystick.Horizontal * 2, 0, joystick.Vertical * 2);
                                var rotLookAt = Quaternion.LookRotation(targetLookAt.localPosition);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotLookAt, rotSpeed * 2 * Time.deltaTime);
                            }
                        }
                        else
                        {
                            if (npcTarget != null && iType == InputType.npc)
                            {
                                var rotDir = npcTarget.transform.position - transform.position;
                                rotDir.y = 0;
                                var rotLookAt = Quaternion.LookRotation(rotDir);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotLookAt, rotSpeed * Time.deltaTime);
                            }
                        }

                        //Move forward
                        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
                    }
                }

                #region Max Distance Respawn:
                if (Vector3.Distance(transform.position, Vector3.zero) > 500)
                    if (!GameController.instance.setList.Contains(gameObject))
                    {
                        GameController.instance.setList.Add(gameObject);
                    }
                #endregion}
            }
    }

#region AI xD
    IEnumerator<float> _SetNpcTarget()
    {
        //Quick fix :|
        while (!GameController.instance.gameDur)
        {
            if (GameController.instance.gameDur) break;
            yield return Timing.WaitForSeconds(.25f);
        }

        npcTargetList.AddRange(GameController.instance.npcList);
        //Check the target priority!!! >:|
        while (!GameController.instance.gameEnded)
        {
            if (GameController.instance.gameEnded) break;
#region Return to catapult:
            if (grabPoint.childCount > 0)
            {
                npcTarget = npcCatapult.transform;
                yield return Timing.WaitForSeconds(.25f);
            } else npcTarget = null;
#endregion

#region Random Pause Delay:
            float nullTargetChance = Random.Range(0, 1);
            if (nullTargetChance > .85f)
            {
                npcTarget = null;
                yield return Timing.WaitForSeconds(.5f);
                continue;
            }
#endregion might remove later idk xD />

#region Get a target:
            //Set npc Target:
            //Priority list: Ball (based on distance || catapult done?), sabotage (bump enemies), catapult
            if (npcTarget == null && npcTarget != npcCatapult.transform)
            {
                for (int i = 0; i < npcTargetList.Count; i++)
                {
                    //If not self xD
                    if (npcTargetList[i] != transform)
                    {
                        //Item Priority! >:]
                        if(npcTargetList[i].GetComponent<Item>() != null)
                        {
                            if (!npcTargetList[i].GetComponent<Item>().carried)
                            {
                                var dist = Vector3.Distance(transform.position, npcTargetList[i].transform.position);
                                if (dist < npcTargetDistance)
                                {
                                    npcTargetDistance = dist;
                                    npcTarget = npcTargetList[i].transform;
                                }
                                continue;
                            }
                        }
                        //Not the same team -.-
                        if (npcTargetList[i].GetComponent<Npc>() != null)
                        {
                            if (npcTargetList[i].GetComponent<Npc>().team != team)
                            {
                                var dist = Vector3.Distance(transform.position, npcTargetList[i].transform.position);
                                if (dist < npcTargetDistance)
                                {
                                    npcTargetDistance = dist;
                                    npcTarget = npcTargetList[i].transform;
                                }
                            }
                        }
                    }
                }
            yield return Timing.WaitForSeconds(.25f);   //Detect if there is another target priority... -.-
            }
#endregion

            //TO DO :\
#region Defend ally:
            //If your ally is close by and has an item => search for enemies nearby and bump them
#endregion

            //TO DO :/
#region Attack enemy:
            //If an enemy has an item (and you don't) bump them
#endregion

            npcTargetDistance = 100;    //Has to be reset -.-
        }
    }
#endregion ai />

    public void Grab(Item i)
    {
        Drop();
        i.transform.SetParent(grabPoint);
        i.transform.localEulerAngles = i.transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        //isCarrying = false;
        if (grabPoint.childCount > 0)
        {
            var drop = grabPoint.GetChild(0);
            drop.GetComponent<Item>().StopIgnoringCollision();
            Vector3 dir = (drop.transform.position - transform.position).normalized;
            drop.GetComponent<Rigidbody>().AddForce((dir + Vector3.up) * 5f, ForceMode.Impulse);
        }
    }

#region Stun:
    [Header("Stun Fx:")]
    [SerializeField] GameObject stunFx;
    IEnumerator<float> _Stun()
    {
        stunned = true;
        stunFx.SetActive(true);
        yield return Timing.WaitForSeconds(stunDur);
        GetUp();
        stunned = false;
        stunFx.SetActive(false);
        hp = startHp;
    }

    private void OnCollisionEnter(Collision character)
    {
        if (character.transform.CompareTag("Character"))
        {
            //Push them away!!!
            if (character.transform.GetComponent<Npc>() != null)
            {
                if (character.transform.GetComponent<Npc>().team != team)
                {
                    character.transform.GetComponent<Npc>().Bounce();
                    var dir = (character.transform.position - transform.position).normalized;
                    character.transform.GetComponent<Npc>().rb.AddForce(dir * bounceForce, ForceMode.Impulse);
                } else {
                    Physics.IgnoreCollision(character.transform.GetComponent<Collider>(), GetComponent<Collider>(), true);
                }
            }
        }
    }

    public void Bounce()
    {
        Drop();
        hp -= 1;
        Timing.RunCoroutine(_Disabled().CancelWith(gameObject));
    }

    IEnumerator<float> _Disabled()
    {
        disabled = true;
        yield return Timing.WaitForSeconds(.25f);
        if (hp == 0) Timing.RunCoroutine(_Stun().CancelWith(gameObject));  //instantiate stun vfx (?)
        WakeUp();
    }

    public void WakeUp()
    {
        disabled = stunned = false;
    }
#endregion Stun />

#region Get Up:
    public void GetUp()
    {
        if (inAir)
            Timing.RunCoroutine(_GetUp().CancelWith(gameObject));
    }
    IEnumerator<float> _GetUp()
    {
        LeanTween.rotate(gameObject, new Vector3(0, transform.parent.localEulerAngles.y, 0), .25f);
        yield return Timing.WaitForSeconds(.25f);
        WakeUp();
    }
    #endregion get up />


    #region Look At Ball:
    public void LookAtBall(Transform ball)
    {
        Timing.RunCoroutine(_LookAtBall(ball).CancelWith(gameObject));
        //Be surprised!
        var emotions = GetComponentsInChildren<Expression>();
        for (int i = 0; i < emotions.Length; i++)
        {
            if (emotions[i].emotion == Emotion.surprised)
            {
                emotions[i].Wow();
                break;
            }
        }
    }
    IEnumerator<float> _LookAtBall(Transform ball)
    {
        while (GameController.instance.gamePaused)
        {
            //Rotate towards the ball:
            var lookDir = ball.position - transform.position;
            var lookRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 20 * Time.deltaTime);
            yield return Timing.WaitForSeconds(Timing.DeltaTime);
        }
    }
    #endregion look at ball />

}
