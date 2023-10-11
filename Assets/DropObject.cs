using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public SO_DropObjData data; //provided when instantiated

    int growthOrder = 0;
    public int GrowthOrder { get { return growthOrder; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DropObject>() == null) return;
        if (this.growthOrder == data.dropItems.Count - 1) return;    //reached max growth

        DropObject collidedBall = collision.gameObject.GetComponent<DropObject>();
        if (collidedBall.GrowthOrder == this.growthOrder)
        {
            Debug.Log("Collide");
            Grow(collision);
        }
    }

    void Grow(Collision2D collision)
    {
        Vector2 mergePos = (gameObject.transform.position + collision.gameObject.transform.position) / 2;
        Destroy(collision.gameObject);
        gameObject.transform.position = mergePos;

        growthOrder++;
        spriteRenderer.color = data.dropItems[growthOrder].color;
        gameObject.transform.localScale *= data.dropItems[growthOrder].sizeMultiplier;
    }
    void SetGrowth(int growthOrder)
    {
        spriteRenderer.color = data.dropItems[growthOrder].color;
        gameObject.transform.localScale *= data.dropItems[growthOrder].sizeMultiplier;
    }
}
