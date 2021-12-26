using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public enum ItemType
{
    ball,
    item
}
public class Item : MonoBehaviour
{
    [Header("Item Type:")]
    public ItemType itemType;
    [Header("Team:")]
    public Team team;

    [Header("Item Id:")]
    public int itemId;  //Use it for crafting the catapult!!!! : D

    Npc[] npcList;

    #region Test :S
    public bool carried { get; set; }
    List<Collider> ignored = new List<Collider>();
    #endregion Test />
    private void Awake()
    {
        StopIgnoringCollision();
        npcList = FindObjectsOfType<Npc>();
        for (int i = 0; i < npcList.Length; i++)
        {
            if (itemType == ItemType.ball)
                npcList[i].npcTargetList.Add(transform);
            else
            {
                if (npcList[i].team == team)
                {
                    npcList[i].npcTargetList.Add(transform);
                    continue;
                }
            }
        }
    }

    private void Update()
    {
        #region Max Distance Respawn:
        if (Vector3.Distance(transform.position, Vector3.zero) > 500) 
            if (!GameController.instance.setList.Contains(gameObject))
            {
                GameController.instance.setList.Add(gameObject);
            }
        #endregion
    }

    #region Carry:
    public void StopIgnoringCollision()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        transform.parent = null;
        carried = false;
        Timing.RunCoroutine(_StopIgnoringCollision().CancelWith(gameObject));
    }
    IEnumerator<float> _StopIgnoringCollision()
    {
        yield return Timing.WaitForSeconds(.45f);
        if (ignored.Count > 0)
            for (int i = 0; i < ignored.Count; i++)
                Physics.IgnoreCollision(ignored[i].transform.GetComponent<Collider>(),
                    GetComponent<Collider>(),
                    false);
        ignored.Clear();
    }
    #endregion Carry />

    private void OnCollisionEnter(Collision character)
    {
        if (character.transform.CompareTag("Character"))
        {
            if (carried || character.transform.GetComponent<Item>() != null)
            {
                Physics.IgnoreCollision(character.transform.GetComponent<Collider>(),
                    GetComponent<Collider>(),
                    true);
                if(character.transform.GetComponent<Npc>() != null)
                    ignored.Add(character.transform.GetComponent<Collider>());
                return;
            }
            if (character.transform.GetComponent<Npc>() != null && !carried)
            {
                #region Not my team >:C
                if (character.transform.GetComponent<Npc>().team != team && itemType == ItemType.item)
                {
                    Physics.IgnoreCollision(character.transform.GetComponent<Collider>(),
                        GetComponent<Collider>(),
                        true);
                    return;
                }
                #endregion not my team />

                var c = character.transform.GetComponent<Npc>();
                if(c.team == team && itemType == ItemType.item)
                {
                    if (c.grabPoint.childCount > 0)
                        c.Drop();

                    carried = true;
                    c.Grab(this);
                    GetComponent<Rigidbody>().isKinematic = true;
                    return;
                }
                else if (itemType == ItemType.ball)
                {
                    carried = true;
                    c.Grab(this);
                    GetComponent<Rigidbody>().isKinematic = true;
                    team = c.team;
                }
            }
        }
    }
}
