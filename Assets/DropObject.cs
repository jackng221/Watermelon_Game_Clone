using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PolygonCollider2D polyCollider;

    public SO_DropObjData data; //provided when instantiated
    public int growthSize = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    #region Grow on collision
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DropObject>() == null) return;
        if (this.growthSize == data.dropItems.Count - 1) return;    //reached max growth

        DropObject collidedBall = collision.gameObject.GetComponent<DropObject>();
        if (collidedBall.growthSize == this.growthSize)
        {
            Debug.Log("Collide");
            collision.gameObject.GetComponent<DropObject>().growthSize = -1;    //attempt to reduce triggering to once per collision
            Grow(collision);
        }
    }
    void Grow(Collision2D collision)
    {
        Vector2 mergePos = (gameObject.transform.position + collision.gameObject.transform.position) / 2;
        Destroy(collision.gameObject);
        gameObject.transform.position = mergePos;

        UpdateObject(this.growthSize + 1);
    }
    #endregion

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
}
