using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUP : MonoBehaviour
{
    [SerializeField] Npc npc;

    private void OnTriggerEnter(Collider ground)
    {
        if (ground.CompareTag("Ground"))
        {
            npc.GetUp();
        }
    }
}
