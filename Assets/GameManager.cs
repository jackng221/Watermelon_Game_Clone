using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    event Action GameStart;
    event Action GameOver;
    event Action ScoreUpdated;

    PlayerControl playerControl;
    [SerializeField] GameObject dropObjPrefab;
    [SerializeField] SO_DropObjData dropObjData;

    int score;
    int nextGrowthOrder;
    [SerializeField] int largestDropObjGrowth = 0;

    private void Awake()
    {
        playerControl = gameObject.GetComponent<PlayerControl>();
        GameStart += ()=> playerControl.enabled = true;
        GameOver += ()=> playerControl.enabled = false;
    }
    private void Start()
    {
        GameStart.Invoke();
    }

    void RollGrowth()
    {
        nextGrowthOrder = UnityEngine.Random.Range(0, largestDropObjGrowth);
    }
    public void DropObject(Vector2 worldPos)
    {
        Instantiate(dropObjPrefab, worldPos, Quaternion.identity);
    }

    //Scores
    public void AddScore(int value)
    {
        score += value;
        ScoreUpdated.Invoke();
    }
    void ResetScore()
    {
        score = 0;
        ScoreUpdated.Invoke();
    }
}
