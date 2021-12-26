using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MEC;

public class CataTarget : MonoBehaviour
{
    [Header("ScoreBoard:")]
    [SerializeField] ScoreBoard score;

    [Header("CubeFetti:")]
    [SerializeField] GameObject cubeFetti;

    [Header("BRAVO!")]
    [SerializeField] GameObject bravo;
    [SerializeField] TextMeshProUGUI bravoTxt;
    readonly string[] bravos = new string[] {"AWESOME!", "GOOD JOB!", "GREAT!", "NICE!", "WELL DONE!", "WOW!", "AMAZING!" };
    [Space]
    [SerializeField] Team team;

    private void OnTriggerEnter(Collider ball)
    {
        if(ball.GetComponent<Item>() != null)
        {
            score.Point();
            Instantiate(cubeFetti, transform.position, cubeFetti.transform.rotation);
            if(team == Team.ally) Bravo();
        }
    }

    void Bravo()
    {
        var randBravo = Random.Range(0, bravos.Length);
        bravo.SetActive(true);
        bravoTxt.text = bravos[randBravo];
        Timing.RunCoroutine(_Bravo().CancelWith(gameObject));
    }

    IEnumerator<float> _Bravo()
    {
        LeanTween.scale(bravo, Vector3.one * 2f, .75f).setEaseOutBounce();
        yield return Timing.WaitForSeconds(1f);
        bravo.transform.localScale = Vector3.zero;
        bravo.SetActive(false);
    }
}
