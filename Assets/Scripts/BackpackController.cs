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
    private Vector2 topRightCorner;
    private Vector2 slotSize;

    // Variables
    private GameObject[,] itemGrid;

    // Use this for initialization
    void Start () {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        spriteSize = new Vector2(sprite.size.x, sprite.size.y);
        // assumes the backpack does not move or get resized. If it does, we'll need to update these.
        bottomLeftCorner = DropZ(transform.position) - spriteSize / 2;
        // topRightCorner = DropZ(transform.position) + spriteSize / 2;
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

    public void ReceiveItemFromMouse(GameObject item)
        // This is called when the player is done dragging, and drops and item into an inventory slot. It will also get called on a single click: picking up an item and putting it right back.
    {
        Vector3 itemPos = item.transform.position;
        Debug.Log(gameObject.name + " received a " + item.name + " @ " + itemPos);
        Vector2Int slot = gridSlot(itemPos);
        Debug.Log("Placing item into slot " + slot);
    }

    private Vector2Int gridSlot(Vector3 worldPos3d)
        // returns which (x,y) backpack slot, with origin at top left, is the best fit for worldPos, or null if worldPos is outside the backpack
    {
        return gridSlot(DropZ(worldPos3d));
    }
    private Vector2Int gridSlot(Vector2 worldPos) {
        // returns which (x,y) backpack slot, with origin at top left,  is the best fit for worldPos, or (-1, -1) if worldPos is outside the backpack
        Vector2 offset = worldPos - bottomLeftCorner;
        // @todo adjust for item sprite size
        //Debug.Log("corners = " + bottomLeftCorner + " " + topRightCorner);
        Debug.Log("offset = " + offset + " slotsize = " + slotSize + " spriteSize = " + spriteSize);
        if (offset.x < 0 || offset.x > spriteSize.x
            || offset.y < 0 || offset.y > spriteSize.y)
        {
            return new Vector2Int(-1, -1); // outside the backpack
        } else
        { // inside the backpack
            Vector2 floatyResult = offset / slotSize;
            int resultX = Convert.ToInt32(floatyResult.x);
            int resultY = gridSize.y - 1 - Convert.ToInt32(floatyResult.y); // flip Y so the origin is at top left instead of bottom left
            return new Vector2Int(resultX, resultY);
        }
    }

    // @todo promote to utilities
    public static Vector2 DropZ(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }
}
