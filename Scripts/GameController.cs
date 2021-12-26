using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

public class GameController : MonoBehaviour
{
    //Spawn Characters on character spawn pos,
    //Give player one of the ally characters to control => material set to blue (player mat)
    //Start the game once it's ready.

    #region Singleton:
    //This should be removed for multiplayer >:O
    public static GameController instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion Singleton />

    [Header("Vfx:")]
    public GameObject vfx;  //The water cube splash >:D
    [SerializeField] GameObject poof;   //Poof C:<

    [Header("Ball:")]
    public GameObject ball;

    public bool gameDur { get; private set; }
    public List<Transform> npcList = new List<Transform>();
    public void StartGame()
    {
        gameDur = true;
        var newball = Instantiate(ball);
        ball = newball;
        Timing.RunCoroutine(_SpawnTick().CancelWith(gameObject));
    }
    public bool gameEnded { get; private set; }
    public bool gamePaused { get; set; } //When ball is shooting!!! >:O
    [Header("CubeFetti:")]
    [SerializeField] GameObject endCubefetti;
    #region EndGame:
    public void EndGame()
    {
        gameDur = false;
        gameEnded = true;
        Timing.RunCoroutine(_EndGame().CancelWith(gameObject));
    }
    IEnumerator<float> _EndGame()
    {
        yield return Timing.WaitForSeconds(.75f);
        SceneManager.LoadSceneAsync(0);
    }
    #endregion end />
    //The Spawn Tick: 
    #region Spawn:
    public HashSet<GameObject> setList = new HashSet<GameObject>();
    Vector3 randPos = Vector3.zero;
    IEnumerator<float> _SpawnTick()
    {
        while (gameDur)
        {
            if (setList.Count > 0)
            {
                if (setList.First() != ball)
                    randPos = new Vector3(Random.Range(-2.25f, 2.25f), 3.5f, Random.Range(-2.75f, 2.75f));
                else randPos = new Vector3(0, 2.5f, 0);
                if (setList.First().GetComponent<Npc>() != null)
                    setList.First().GetComponent<Npc>().Drop();
                yield return Timing.WaitForSeconds(.25f);
                setList.First().transform.position = randPos;
                Instantiate(poof, randPos, poof.transform.rotation);    //POOF!!! :O
                setList.First().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                setList.First().GetComponent<Rigidbody>().velocity = Vector3.zero;
                setList.First().GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                if (setList.First().GetComponent<Npc>() != null)
                {
                    setList.First().GetComponent<Npc>().inAir =
                    setList.First().GetComponent<Npc>().disabled = true;
                    setList.First().GetComponent<Npc>().stunned = false;
                } else {
                    if (setList.First().GetComponent<Item>() != null)
                    {
                        setList.First().GetComponent<Item>().StopIgnoringCollision();
                        setList.First().transform.parent = null;
                    }
                }
                setList.Remove(setList.First());
                continue;
            }
            yield return Timing.WaitForSeconds(.5f);
        }
    }
    #endregion


    private void OnTriggerEnter(Collider character)
    {
        if (character.CompareTag("Character"))
        {
            Instantiate(vfx, character.transform.position, vfx.transform.rotation);
            if (character.GetComponent<Npc>() != null)
            {
                character.GetComponent<Npc>().stunned = true;
            }
            if (!setList.Contains(character.gameObject))
            {
                setList.Add(character.gameObject);
            }
            character.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;  //In water!!!
        }
    }
}
