using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    //Set Up the zone:
    [Header("Family:")]
    [SerializeField] GameObject[] family;
    [SerializeField] List<Vector3> spawnPos = new List<Vector3>();

    [Header("Team:")]
    [SerializeField] Team team;

    [Header("Catapult:")]
    [SerializeField] Catapult cata; //Cata xD


    private void Start()
    {
        SetZone();
    }

    public void SetZone()
    {
        //Shuffle
        for (int i = 0; i < family.Length; i++)
        {
            GameObject temp = family[i];
            int randomIndex = Random.Range(i, family.Length);
            family[i] = family[randomIndex];
            family[randomIndex] = temp;
        }
        int playerId = Random.Range(0, family.Length);
        for (int i = 0; i < spawnPos.Count; i++)
        {
            var f = Instantiate(family[i], spawnPos[i], transform.rotation);
            f.transform.parent = transform;
            f.GetComponent<Npc>().team = team;
            f.GetComponent<Npc>().npcCatapult = cata;
            GameController.instance.npcList.Add(f.transform);
            if (i == playerId && team == Team.ally)
                f.GetComponent<Npc>().SetAsPlayer();
        }
    }
}
