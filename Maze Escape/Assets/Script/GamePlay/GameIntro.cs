using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntro : MonoBehaviour
{
    public GameObject effect;
    public GameObject player;

    void Awake()
    {
        effect.SetActive(true);
        player.SetActive(false);

    }
    void Start()
    {
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        effect.SetActive(false);
        yield return null;
        player.SetActive(true);
    }
}
