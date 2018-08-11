using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackController : MonoBehaviour {

    // initialized once, then left alone
    public GameObject redMushroom;
    public GameObject blueMushroom;
    public GameObject healthPotion;
    public GameObject manaPotion;
    public GameObject staminaPotion;
    private GameObject[,] recipes;
    private readonly Vector2Int gridSize = new Vector2Int(3, 3);
    private SpriteRenderer sprite;
    private Vector2 spriteSize;
    private Vector2 bottomLeftCorner;
    private Vector2 slotSize;

    // Variables
    //private GameObject[,] itemGrid;

    // Use this for initialization
    void Start () {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        spriteSize = new Vector2(sprite.size.x, sprite.size.y);
        // assumes the backpack does not move or get resized. If it does, we'll need to update these.
        bottomLeftCorner = DropZ(transform.position) - spriteSize / 2;
        slotSize = sprite.size / gridSize;
        recipes = new GameObject[,]
        {
            // ingredient1 + ingredient2 = result
            {redMushroom, redMushroom, healthPotion},
            {blueMushroom, blueMushroom, manaPotion},
            // @todo add more here
        };
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
        Vector2Int slot = GridSlot(itemOrigin);
        if(slot.x < 0 || slot.y < 0)
        {
            GameObject.Destroy(item);
            return false;
        } else { 
            Debug.Log(item.name + " received, placing into slot " + slot);
            Vector2 offsetToOrigin = -1 * spriteSize / 2;
            Vector2 itemSizeOffset = slotSize / 2;
            Vector2 slotOffset = slot * slotSize;
            Vector2 offset = offsetToOrigin + slotOffset + itemSizeOffset;
            //Debug.Log(offsetToOrigin + " " + slotOffset + " " + itemSizeOffset);
            item.transform.localPosition = ZeroZ(offset);
            // check for overlap
            foreach (ItemController otherItemController in FindObjectsOfType<ItemController>())
            {
                if( item.GetComponent<ItemController>() != otherItemController) // it's ok to overlap yourself
                {
                    Vector2Int otherSlot = GridSlot(otherItemController.transform.position);
                    if (slot == otherSlot)
                    {
                        GameObject craftingResult = Craft(item, otherItemController.gameObject); // crafts by side effect
                        // if you can craft, that counts as a successful drop so return true
                        // if you can't craft, return false to snap the dropped object back to its original position
                        return (craftingResult != null); 
                    }
                }
            }
            return true;
        }
    }

    private GameObject Craft(GameObject ingredient, GameObject otherIngredient)
        // @return null if these ingredients are not craftable
    {
        GameObject result = CraftOrdered(ingredient, otherIngredient, otherIngredient.transform);
        if (result == null)
        {
            result = CraftOrdered(otherIngredient, ingredient, otherIngredient.transform);
        }
        return result;
    }

    private GameObject CraftOrdered(GameObject ingredient1, GameObject ingredient2, Transform destination)
    // @param destination where to put the crafted item
    {
        int recipeNum;
        for (recipeNum = 0; recipeNum < recipes.GetLength(0); recipeNum++)
        {
            GameObject recipeIngredient1 = recipes[recipeNum, 0];
            GameObject recipeIngredient2 = recipes[recipeNum, 1];
            GameObject recipeResult = recipes[recipeNum, 2];
            // @todo robustify this against names with "(Clone)"
            if (recipeIngredient1.name.Equals(ingredient1.name) && recipeIngredient2.name.Equals(ingredient2.name)) {
                Debug.Log("About to craft " + ingredient1.name + " + " + ingredient2.name + " into " + recipeResult.name + " @ " + destination.position);
                GameObject result = Instantiate<GameObject>(recipeResult, destination);
                result.transform.position = destination.position;
                Debug.Log("Created " + result + " @ " + result.transform.position);
                GameObject.Destroy(ingredient1);
                GameObject.Destroy(ingredient2);
                return result;
            }
        }
        // no recipes apply - fallthrough case
        return null;
    }

    public Vector2Int GridSlot(Vector3 worldPos3d)
        // returns which (x,y) backpack slot, with origin at bottom left, is the best fit for worldPos, or (-1, -1) if worldPos is outside the backpack
    {
        return GridSlot(DropZ(worldPos3d));
    }
    public Vector2Int GridSlot(Vector2 worldPos) {
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

    public int TotalHealth()
    {
        return TotalHealthManaStamina().x;
    }

    public int TotalMana()
    {
        return TotalHealthManaStamina().y;
    }

    public int TotalStamina()
    {
        return TotalHealthManaStamina().z;
    }

    private Vector3Int TotalHealthManaStamina()
    // @return (healthBoost, manaBoost, staminaBoost) summed over all items in the backpack
    {
        Vector3Int total = new Vector3Int(0, 0, 0);
        foreach (ItemController itemController in FindObjectsOfType<ItemController>())
        {
            if (GridSlot(itemController.transform.position).x > 0)
            { // item is in backpack
                total.x += itemController.healthBoost;
                total.y += itemController.manaBoost;
                total.z += itemController.staminaBoost;
            }
        }
        return total;
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
