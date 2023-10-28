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

    GameControl playerControl;
    [SerializeField] GameObject dropObjPrefab;
    [SerializeField] SO_DropObjData dropObjData;
    [SerializeField] SpriteRenderer currentDropSprite;
    [SerializeField] Image nextDropUiImage;
    [SerializeField] TextMeshProUGUI scoreUI;
    [SerializeField] GameObject gameOverUI;

    [SerializeField] int score;
    [SerializeField] AnimationCurve scoreMultiplier;
    [SerializeField] int currentDropGrowth = 0;
    [SerializeField] int nextDropGrowth = 0;

    [SerializeField] float failTimerDefault = 10f;
    float failTimer;
    [SerializeField] GameObject failTimerPrefab;
    List<GameObject> failTimerObjs;
    public List<DropObject> invalidDropObjs;
    bool inCountDown = false;
    Coroutine countDownCoroutine;


    private void Awake()
    {
        playerControl = gameObject.GetComponent<GameControl>();

        GameStart += () => playerControl.enabled = true;
        GameStart += () => RollGrowth(true);
        gameOverUI.SetActive(false);
        GameOver += () => playerControl.enabled = false;
        GameOver += EndGame;

        NewDropRoll += () => RollGrowth(false);
        ScoreUpdate += UpdateScoreUI;

        failTimerObjs = new List<GameObject>();
        invalidDropObjs = new List<DropObject>();
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

        while (failTimerObjs.Count < invalidDropObjs.Count)
        {
            failTimerObjs.Add( Instantiate(failTimerPrefab) );
        }
        while (failTimerObjs.Count > invalidDropObjs.Count)
        {
            Destroy(failTimerObjs[failTimerObjs.Count - 1].gameObject);
            failTimerObjs.RemoveAt(failTimerObjs.Count -1);
        }
        for (int i = 0; i < failTimerObjs.Count; i++)
        {
            failTimerObjs[i].transform.position = invalidDropObjs[i].gameObject.transform.position;
            failTimerObjs[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = failTimer.ToString() + "s";
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

        failTimer = failTimerDefault;
        do
        {
            Debug.Log(failTimer);
            yield return new WaitForSeconds(1);
            failTimer -= 1;
        } while ( failTimer > 0 );

        Debug.Log("Time out");
        GameOver.Invoke();
    }
    void EndGame()
    {
        gameOverUI.SetActive(true);
        SessionManager.Instance.LoadScene("Title");
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

    [ContextMenu("GameOver")]
    public void GameOverTest()
    {
        GameOver.Invoke();
    }
}
