using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DropObject : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PolygonCollider2D polyCollider;

    public GameManager gameManager;
    public SO_DropObjData data; //provided when instantiated
    public int growthSize = 0;

    bool inValidArea = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>();
    }
    private void Start()
    {
        StartCoroutine(CheckInvalidAfterSpawn());
    }
    IEnumerator CheckInvalidAfterSpawn()
    {
        yield return new WaitForSeconds(1);
        if (inValidArea == false)
        {
            gameManager.invalidDropObjs.Add(this);
        }
    }

    #region Grow on collision
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DropObject>() == null) return;
        if (this.growthSize == data.dropItems.Count - 1) return;    //reached max growth

        DropObject collidedBall = collision.gameObject.GetComponent<DropObject>();
        if (collidedBall.growthSize == this.growthSize)
        {
            //Debug.Log("Collide");
            collision.gameObject.GetComponent<DropObject>().growthSize = -1;    //attempt to reduce triggering to once per collision
            Grow(collision);
        }
    }
    void Grow(Collision2D collision)
    {
        Vector2 mergePos = (gameObject.transform.position + collision.gameObject.transform.position) / 2;
        Destroy(collision.gameObject);
        gameObject.transform.position = mergePos;

        Score();
        UpdateObject(this.growthSize + 1);
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Enter area designinated as valid play area
        if (collision.gameObject.GetComponent<AreaCountdown>() != null)
        {
            gameManager.invalidDropObjs.Remove(this);
            inValidArea = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        //Touch trigger below playing field, instant gameover
        if (collision.gameObject.GetComponent<AreaOutOfMap>() != null)
        {
            Destroy(gameObject);
            gameManager.GameOverByOutOfMap();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Exit valid play area
        if (collision.gameObject.GetComponent<AreaCountdown>() != null)
        {
            gameManager.invalidDropObjs.Add(this);
            inValidArea = false;
            gameObject.layer = LayerMask.NameToLayer("InvalidDropObj");
        }
    }

    void Score()
    {
        gameManager.AddScore(growthSize);
    }

    public void UpdateObject(int growthSize)
    {
        this.growthSize = growthSize;
        if (data.dropItems[growthSize].sprite != null)
        {
            spriteRenderer.sprite = data.dropItems[growthSize].sprite;
            polyCollider.points = data.dropItems[growthSize].colliderPoints;
        }
        spriteRenderer.color = data.dropItems[growthSize].color;

        float tempScale = data.dropItems[growthSize].sizeMultiplier;
        gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
    }

    private void OnDestroy()
    {
        gameManager.invalidDropObjs.Remove(this);
    }
}
