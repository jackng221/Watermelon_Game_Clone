using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    event Action GameStart;
    event Action GameOver;
    event Action ScoreUpdate;
    event Action NewDropRoll;

    PlayerControl playerControl;
    [SerializeField] GameObject dropObjPrefab;
    [SerializeField] SO_DropObjData dropObjData;
    [SerializeField] SpriteRenderer currentDropSprite;
    [SerializeField] Image nextDropUiImage;
    [SerializeField] TextMeshProUGUI scoreUI;

    [SerializeField] int score;
    [SerializeField] AnimationCurve scoreMultiplier;
    [SerializeField] int currentDropGrowth = 0;
    [SerializeField] int nextDropGrowth = 0;

    public List<DropObject> invalidDropObjs;
    bool inCountDown = false;
    Coroutine countDownCoroutine;

    private void Awake()
    {
        playerControl = gameObject.GetComponent<PlayerControl>();

        GameStart += () => playerControl.enabled = true;
        GameStart += () => RollGrowth(true);
        GameOver += () => playerControl.enabled = false;

        NewDropRoll += () => RollGrowth(false);
        ScoreUpdate += UpdateScoreUI;
    }
    private void Start()
    {
        GameStart.Invoke();
    }
    private void Update()
    {
        if (invalidDropObjs.Count != 0 && inCountDown == false)
        {
            countDownCoroutine = StartCoroutine(CountDown());
        }
        else if (invalidDropObjs.Count == 0 && inCountDown == true)
        {
            StopCoroutine(countDownCoroutine);
            inCountDown = false;
        }
    }


    //Game Mechanics
    public void DropObject(Vector2 worldPos)
    {
        GameObject obj = Instantiate(dropObjPrefab, worldPos, Quaternion.identity);
        obj.GetComponent<DropObject>().gameManager = this;
        obj.GetComponent<DropObject>().data = dropObjData;
        obj.GetComponent<DropObject>().UpdateObject(currentDropGrowth);

        NewDropRoll.Invoke();

    }
    void RollGrowth(bool isFirstRoll)
    {
        if (isFirstRoll)
        {
            currentDropGrowth = UnityEngine.Random.Range(0, dropObjData.largestDropObjGrowth);
            nextDropGrowth = UnityEngine.Random.Range(0, dropObjData.largestDropObjGrowth);
        }
        else
        {
            currentDropGrowth = nextDropGrowth;
            nextDropGrowth = UnityEngine.Random.Range(0, dropObjData.largestDropObjGrowth);
        }
        UpdateDropSprites(currentDropGrowth, nextDropGrowth);
    }
    IEnumerator CountDown()
    {
        inCountDown = true;

        float timer = 10f;
        do
        {
            Debug.Log(timer);
            yield return new WaitForSeconds(1);
            timer -= 1;
        } while ( timer > 0 );
        Debug.Log("Time out");
    }


    //UI
    void UpdateDropSprites(int currentgrowthsize, int nextgrowthsize)   //sprite, color, scale only
    {
        //current drop
        if (dropObjData.dropItems[currentgrowthsize].sprite != null)
        {
            currentDropSprite.sprite = dropObjData.dropItems[currentgrowthsize].sprite;
        }
        currentDropSprite.color = dropObjData.dropItems[currentgrowthsize].color;

        float tempscale = dropObjData.dropItems[currentgrowthsize].sizeMultiplier;
        currentDropSprite.gameObject.transform.localScale = new Vector3(tempscale, tempscale, tempscale);

        //next drop
        if (dropObjData.dropItems[nextgrowthsize].sprite != null)
        {
            nextDropUiImage.sprite = dropObjData.dropItems[nextgrowthsize].sprite;
        }
        nextDropUiImage.color = dropObjData.dropItems[nextgrowthsize].color;

        float tempnextscale = dropObjData.dropItems[nextgrowthsize].sizeMultiplier;
        nextDropUiImage.gameObject.transform.localScale = new Vector3(tempnextscale, tempnextscale, tempnextscale);
    }
    public void UpdateCurrentDropSpritePos(Vector2 dropPos)
    {
        currentDropSprite.transform.position = dropPos;
    }
    

    //Scores
    public void AddScore(int growth)
    {
        float tempAddScore = scoreMultiplier.Evaluate(growth);
        //Debug.Log(tempAddScore);
        tempAddScore = Mathf.Round(tempAddScore);
        score += (int) tempAddScore;
        ScoreUpdate.Invoke();
    }
    void ResetScore()
    {
        score = 0;
        ScoreUpdate.Invoke();
    }
    void UpdateScoreUI()
    {
        scoreUI.text = "Score: \n" + score;
    }
}
