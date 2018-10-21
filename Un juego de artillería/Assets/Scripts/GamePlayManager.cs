using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{

    [SerializeField]
    EnemyAI enemy;
    [SerializeField]
    PlayerControl hero;
    [SerializeField]
    GameObject startButton;
    [SerializeField]
    GameObject attackButton;
    public CameraFollow cameraFollow;
    [SerializeField]
    GameObject UIRoot;

    Text[] mainText;
    int timeRemaining = 10;
    GameObject[] UI;
    Gun heroGun;

    public static bool coroutineExplosionOn = false;

    private void Awake()
    {
        UIRoot.SetActive(false);
        PlayerPrefs.SetInt("Exploded", 0);
    }

    // Use this for initialization
    void Start()
    {
        heroGun = hero.GetComponentInChildren<Gun>();
        enemy.GetComponentInChildren<Gun>().gunFired += SwapTurn;
        heroGun.gunFired += SwapTurn;
        mainText = startButton.GetComponentsInChildren<Text>();
    }

    public void StartGame()
    {
        UIRoot.SetActive(true);
        startButton.GetComponent<Button>().enabled = false;
        enemy.hasTurn = true;
        InvokeRepeating("DecreaseTime", 0, 1);
        SwapTurn();
    }

    void SwapTurn()
    {
        StartCoroutine(SwapTurnCoroutine());
    }

    IEnumerator SwapTurnCoroutine()
    {
        timeRemaining = 10;
        //If hero has already launched the rocket/s, but at the time we get here rockets had not exploded yet
        //StopCoroutine -> when the rocket/s impact, swapTurn will be called(otherwise double swapTurn problems will occur)
        if (hero.hasTurn && heroGun.fired)
        {
            StopCoroutine("SwapTurnCoroutine");
        }
        //if its the end of our turn and we are still charging the rocket -> fire automatically before swap turn
        else if (hero.hasTurn && heroGun.state == Gun.States.Down)
        {
            heroGun.state = Gun.States.Fire;
        }
        else
        {
            hero.hasTurn = !hero.hasTurn;
            UIRoot.SetActive(hero.hasTurn);
            cameraFollow.SetPlayerToFollow(hero.hasTurn ? hero.transform : enemy.transform);
            if (!hero.hasTurn)
            {
                enemy.GetComponent<PlayerControl>().SetPlayerWeapon(0);
            }
            else
            {
                hero.GetComponent<PlayerControl>().SetPlayerWeapon(0);
            }
            if (!enemy.hasTurn)
            {
                yield return new WaitForSeconds(2f);
            }
            enemy.hasTurn = !enemy.hasTurn;
        }
    }

    void DecreaseTime()
    {
        timeRemaining--;
        if (timeRemaining < 0)
        {
            SwapTurn();
        }
        foreach (Text t in mainText)
        {
            t.text = " " + timeRemaining;
        }
    }
}
