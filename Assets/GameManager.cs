using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] int score;
    [SerializeField] int currentDropGrowth = 0;
    [SerializeField] int nextDropGrowth = 0;

    private void Awake()
    {
        playerControl = gameObject.GetComponent<PlayerControl>();

        GameStart += () => playerControl.enabled = true;
        GameStart += () => RollGrowth(true);
        GameOver += () => playerControl.enabled = false;

        NewDropRoll += () => RollGrowth(false);
    }
    private void Start()
    {
        GameStart.Invoke();
    }

    //Game Mechanics
    public void DropObject(Vector2 worldPos)
    {
        GameObject obj = Instantiate(dropObjPrefab, worldPos, Quaternion.identity);
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
    public void AddScore(int value)
    {
        score += value;
        ScoreUpdate.Invoke();
    }
    void ResetScore()
    {
        score = 0;
        ScoreUpdate.Invoke();
    }
}
