using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Catapult : MonoBehaviour
{
    [Header("Team:")]
    [SerializeField] Team team;

    [Header("Craft Parts:")]
    public GameObject[] parts;
    bool allParts = true;

    [Space]
    [SerializeField] float desiredRotation;
    [Header("High Point:")]
    public GameObject highPoint;    //The max Y distance :C
    public Transform target; //Shoot Target... -.-   

    [Header("Items:")]
    [SerializeField] GameObject[] items;

    [Header("Ball Pos:")]
    public Transform ballPos;
    public Rigidbody ball;

    private void OnTriggerEnter(Collider item)
    {
        if (item.GetComponent<Item>() != null)
        {
            if (!allParts && item.GetComponent<Item>().itemType == ItemType.item &&
                item.GetComponent<Item>().team == team)
            {
                CraftCata(item.GetComponent<Item>().itemId);
                for (int i = 0; i < GameController.instance.npcList.Count; i++)
                    GameController.instance.npcList[i]
                        .GetComponent<Npc>()
                        .npcTargetList
                        .Remove(item.transform);
                Destroy(item.gameObject);
            }

            if (allParts && item.GetComponent<Item>().itemType == ItemType.ball &&
                item.GetComponent<Item>().carried &&
                item.GetComponent<Item>().team == team)
            {
                GetComponent<Collider>().enabled = false;
                ball = item.GetComponent<Rigidbody>();
                ball.transform.position = ballPos.position;
                CameraController.instance.curTarget = item.transform;
                LaunchCatapult();
            }
        }
    }

    #region Crafting:
    void CraftCata(int id)
    {
        parts[id].SetActive(true);
        CheckCraftDone();
    }

    void CheckCraftDone()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (!parts[i].activeSelf)
            {
                allParts = false;
                break;
            }
            allParts = true;
        }
    }
    #endregion crafting />

    public void LaunchCatapult()
    {
        //launch the ball + destroy the catapult
        //=> set all parts inactive + instantitate items and shoot them forward)
        Timing.RunCoroutine(_DisableActiveParts().CancelWith(gameObject));
    }

    IEnumerator<float> _DisableActiveParts()
    {
        GameController.instance.gamePaused = true;
        for (int i = 0; i < GameController.instance.npcList.Count; i++)
            GameController.instance.npcList[i]
                        .GetComponent<Npc>()
                        .LookAtBall(ball.transform);

        ball.GetComponent<Item>().StopIgnoringCollision();
        ball.useGravity = false;
        ball.transform.parent = ballPos;
        Npc[] chars = FindObjectsOfType<Npc>();
        foreach (var c in chars)
        {
            c.disabled = true;
        }
        LeanTween.rotateX(parts[0], desiredRotation, .25f);
        yield return Timing.WaitForSeconds(.15f);
        ball.transform.parent = null;
        //Launch the ball! :O
        Launch();
        yield return Timing.WaitForSeconds(.25f);
        //Disable active parts:
        for (int i = 0; i < parts.Length; i++)
            parts[i].SetActive(false);
        //Instantiate items:
        for (int i = 0; i < items.Length; i++)
        {
            var item = Instantiate(items[i], transform.position + new Vector3(0, 1.25f, 0), items[i].transform.rotation);
            float randForce = Random.Range(3.5f, 8.5f);
            item.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * randForce, ForceMode.Impulse);
            yield return Timing.WaitForSeconds(.25f);
        }
        parts[0].transform.localEulerAngles = Vector3.zero;
        //Max y = high point.y
        yield return Timing.WaitForSeconds(3f);
        allParts = false;

        foreach (var c in chars)
        {
            c.disabled = false;
        }
        CameraController.instance.canSetTarget = 
        GetComponent<Collider>().enabled = true;
        ball.GetComponent<Item>().StopIgnoringCollision();
        GameController.instance.gamePaused = false;
    }


    #region Kinematic Equations:
    public float gravity = -18;
    public float h = 25;

    void Launch()
    {
        Physics.gravity = Vector3.up * gravity;
        ball.useGravity = true;
        ball.velocity = CalculateLaunchData().initialVelocity;
    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.position.y - ball.position.y;
        Vector3 displacementXZ = new Vector3(target.position.x - ball.position.x, 0, target.position.z - ball.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }
    #endregion
}
