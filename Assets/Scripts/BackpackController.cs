using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackController : MonoBehaviour {

    // initialized once, then left alone
    private readonly Vector2Int gridSize = new Vector2Int(3, 3);
    private SpriteRenderer sprite;
    private Vector2 spriteSize;
    private Vector2 bottomLeftCorner;
    private Vector2 slotSize;

    // Variables
    private GameObject[,] itemGrid;

    // Use this for initialization
    void Start () {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        spriteSize = new Vector2(sprite.size.x, sprite.size.y);
        // assumes the backpack does not move or get resized. If it does, we'll need to update these.
        bottomLeftCorner = DropZ(transform.position) - spriteSize / 2;
        slotSize = sprite.size / gridSize; 
    }

    // Update is called once per frame
    void Update () {
		
	}

    public bool ReceiveItemFromHero(GameObject item)
    // @return Whether item was successfully placed into the backpack
    {
        return false; // @todo unstub   
    }

    public bool ReceiveItemFromMouse(GameObject item)
        // @return Did the item successfully drop?
        // This is called when the player is done dragging, and drops and item into an inventory slot. It will also get called on a single click: picking up an item and putting it right back.
    {
        Vector2 itemCenter = DropZ(item.transform.position);
        SpriteRenderer itemSprite = item.GetComponent<SpriteRenderer>();
        Vector2 itemSpriteSize = new Vector2(itemSprite.size.x, itemSprite.size.y);
        Vector2 itemOrigin = itemCenter - itemSpriteSize / 2;
        Vector2Int slot = gridSlot(itemOrigin);
        if(slot.x < 0 || slot.y < 0)
        {
            GameObject.Destroy(item);
            return false;
        } else { 
            Debug.Log(gameObject.name + " received, placing into slot " + slot);
            Vector2 offsetToOrigin = -1 * spriteSize / 2;
            Vector2 itemSizeOffset = slotSize / 2;
            Vector2 slotOffset = slot * slotSize;
            Vector2 offset = offsetToOrigin + slotOffset + itemSizeOffset;
            Debug.Log(offsetToOrigin + " " + slotOffset + " " + itemSizeOffset);
            item.transform.localPosition = ZeroZ(offset);
            // check for overlap
            ItemController[] itemControllers = FindObjectsOfType<ItemController>();
            foreach (ItemController itemController in itemControllers)
            {
                if( item.GetComponent<ItemController>() != itemController) // it's ok to overlap yourself
                {
                    Vector2Int otherSlot = gridSlot(itemController.transform.position);
                    if (slot == otherSlot)
                    {
                        // Here is where to implement crafting.
                        return false; // overlap
                    }
                }
            }
            return true;
        }
    }

    public Vector2Int gridSlot(Vector3 worldPos3d)
        // returns which (x,y) backpack slot, with origin at bottom left, is the best fit for worldPos, or (-1, -1) if worldPos is outside the backpack
    {
        return gridSlot(DropZ(worldPos3d));
    }
    public Vector2Int gridSlot(Vector2 worldPos) {
        // returns which (x,y) backpack slot, with origin at bottom left,  is the best fit for worldPos, or (-1, -1) if worldPos is outside the backpack
        Vector2 offset = worldPos - bottomLeftCorner;
        //Debug.Log("offset = " + offset + " slotsize = " + slotSize + " spriteSize = " + spriteSize);
        if (offset.x < 0 || offset.x > spriteSize.x
            || offset.y < 0 || offset.y > spriteSize.y)
        {
            return new Vector2Int(-1, -1); // outside the backpack
        } else
        { // inside the backpack
            Vector2 floatyResult = offset / slotSize;
            int resultX = (int)Math.Floor(floatyResult.x);
            int resultY = (int)Math.Floor(floatyResult.y); // this is where we would flip Y if we wanted to
            return new Vector2Int(resultX, resultY);
        }
    }

    // @todo promote to utilities
    public static Vector2 DropZ(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }

    public static Vector3 ZeroZ(Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0);
    }
}
