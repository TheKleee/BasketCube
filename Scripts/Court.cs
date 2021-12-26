using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Court : MonoBehaviour
{
    private void OnCollisionEnter(Collision character)
    {
        if (character.transform.CompareTag("Character"))
        {
            if (character.transform.GetComponent<Npc>() != null)
            {
                var c = character.transform.GetComponent<Npc>();
                if (c.inAir) c.GetUp();
            }
        }
    }

    private void OnCollisionExit(Collision character)
    {
        if (character.transform.CompareTag("Character"))
        {
            if (character.transform.GetComponent<Npc>() != null)
            {
                var c = character.transform.GetComponent<Npc>();
                c.inAir = c.disabled = true;
            }
        }
    }
}
