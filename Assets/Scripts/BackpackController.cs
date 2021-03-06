﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackController : MonoBehaviour {

    // initialized once, then left alone
    public GameObject meters;
    public GameObject redMushroom;
    public GameObject blueMushroom;
    public GameObject greenMushroom;
    public GameObject healthPotion;
    public GameObject manaPotion;
    public GameObject staminaPotion;
    public GameObject uselessPotion;
    public GameObject gold;
    public GameObject splat;
    public GameObject junk1;
    public GameObject junk2;
    private GameObject[,] recipes;
    private readonly int potionBoostAmount = 3; // used for Consume logic
    private readonly Vector2Int gridSize = new Vector2Int(3, 3);
    private SpriteRenderer sprite;
    private MetersController metersController;
    private AudioSource audioSource;
    private Vector2 spriteSize;
    private Vector2 bottomLeftCorner;
    private Vector2 slotSize;
    private System.Random rng = new System.Random();
    private Vector2Int reservedGridSlot = new Vector2Int(-1, -1);

    // Use this for initialization that only depends on this object and no other objects
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization that depends on other objects
    void Start() {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        spriteSize = new Vector2(sprite.size.x, sprite.size.y);
        metersController = meters.GetComponent<MetersController>();
        // assumes the backpack does not move or get resized. If it does, we'll need to update these.
        bottomLeftCorner = DropZ(transform.position) - spriteSize / 2;
        slotSize = sprite.size / gridSize;
        recipes = new GameObject[,]
        {
            // ingredient1 + ingredient2 = result
            
            // useful recipes
            
            // mushroom + mushroom
            {redMushroom,   redMushroom,   healthPotion},
            {blueMushroom,  blueMushroom,  manaPotion},
            {greenMushroom, greenMushroom, staminaPotion},

            // neutral recipes

            // potion + mushroom
            {healthPotion,  redMushroom,   healthPotion },
            {manaPotion,    blueMushroom,  manaPotion },
            {staminaPotion, greenMushroom, staminaPotion },

            // potion + potion
            /*
            {healthPotion,  healthPotion,  healthPotion },
            {manaPotion,    manaPotion,    manaPotion },
            {staminaPotion, staminaPotion, staminaPotion },
            */

            // failure recipes

            // mushroom + mushroom
            {redMushroom,   blueMushroom,  splat},
            {redMushroom,   greenMushroom, splat},
            {blueMushroom,  greenMushroom, splat},

            // potion + mushroom
            {healthPotion,  blueMushroom,  uselessPotion },
            {healthPotion,  greenMushroom, uselessPotion },
            {manaPotion,    redMushroom,   uselessPotion },
            {manaPotion,    greenMushroom, uselessPotion },
            {staminaPotion, redMushroom,   uselessPotion },
            {staminaPotion, blueMushroom,  uselessPotion },

            // potion + potion
            /*
            {healthPotion,  manaPotion,    uselessPotion },
            {manaPotion,    staminaPotion, uselessPotion },
            {staminaPotion, healthPotion,  uselessPotion },
            */

            // uselessPotion + anything
            /*
            {uselessPotion, redMushroom,   uselessPotion },
            {uselessPotion, blueMushroom,  uselessPotion },
            {uselessPotion, greenMushroom, uselessPotion },
            {uselessPotion, healthPotion,  uselessPotion },
            {uselessPotion, manaPotion,    uselessPotion },
            {uselessPotion, staminaPotion, uselessPotion },
            {uselessPotion, uselessPotion, uselessPotion },
            {uselessPotion, junk1,         uselessPotion },
            {uselessPotion, junk2,         uselessPotion },
            {uselessPotion, splat,         uselessPotion },
            {uselessPotion, gold,          uselessPotion },
            */
        };
        NoteContentsChanged(); // to initialize
    }

    // Update is called once per frame
    void Update() {
      
    }

    public bool ReceiveItemFromHero(GameObject item)
    // @return Whether item was successfully placed into the backpack
    // @param item Assumes that item has already been Instantiated and is ready to be placed in the backpack. We can change that if we want.
    {
        Debug.Log("Received " + item.name + " from hero...");
        // mark each slot as empty (false) or full (true);
        bool[,] slotFull = new bool[gridSize.x, gridSize.y];
        foreach (ItemController itemController in FindObjectsOfType<ItemController>())
        {
            Vector2Int slot = GridSlot(itemController.transform.position);
                if (slot.x >= 0 && slot.y >= 0) { 
                    slotFull[slot.x, slot.y] = true;
            }
        }

        // compute the number of available slots
        int availableSlotCount = 0;
        for (int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                if (! (x == reservedGridSlot.x && y == reservedGridSlot.y)) { 
                    if (!slotFull[x, y])
                        availableSlotCount++;
                }
            }
        }
        if(availableSlotCount == 0)
        {
            Debug.Log("...but I'm out of space!");
            PlayAudioClipOfLoss(item.GetComponent<ItemController>());
            item.transform.position = new Vector3(-99999, -99999, -99999); // @hack if I can't destroy it immediately, just send it off the edge of the world
            GameObject.Destroy(item);
            return false;
        } else
        {
            // pick an available slot at random
            int slotIndex = rng.Next(0, availableSlotCount-1);
            Vector2Int slot = new Vector2Int(0, 0);
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (!(x == reservedGridSlot.x && y == reservedGridSlot.y))
                    {
                        if (slotIndex == 0) slot = new Vector2Int(x, y);
                        if (!slotFull[x, y]) slotIndex--;
                    }
                }
            }

            Debug.Log("...it fell into " + slot);
            SnapToGridSlot(item, slot);
            NoteContentsChanged();
            PlayAudioClip(item.GetComponent<ItemController>());
            return true;
        }
    }

    public bool ConsumeHealth(int amt)
    // @param amt how much health the hero needs
    // @return Whether the full amount was successfully consumed. If this returns false, you die.
    {
        Debug.Log("Hero needs " + amt + " HP!");
        // no special case for TotalHealth; the hero will consume everything red in a last-ditch attempt to not die
        return Consume(amt, 0, 0);
    }

    public bool ConsumeMana(int amt)
    // @param amt how much mana the hero needs
    // @return Whether the full amount was successfully consumed. If this returns false, your spell failed.
    {
        Debug.Log("Hero needs " + amt + " mana!");
        if (TotalMana() >= amt) { 
            return Consume(0, amt, 0);
        } else
        {
            return false; // I don't have enough for you, sorry!
        }
    }

    public bool ConsumeStamina(int amt)
    // @param amt how much stamina the hero needs
    // @return Whether the full amount was successfully consumed. If this returns false, you slow down and/or your melee damage is reduced.
    {
        Debug.Log("Hero needs " + amt + " stamina!");
        if (TotalStamina() >= amt) { 
            return Consume(0, 0, amt);
        } else
        {
            return false; // I don't have enough for you, sorry!
        }
    }

    public bool Consume(int healthToConsume, int manaToConsume, int staminaToConsume)
        // @return Whether all the full amounts were successfully consumed. Destroys (consumes) one or more items by side effect.
    {
        // first, drink potions until all needs are less than three <3
        foreach (ItemController itemController in FindObjectsOfType<ItemController>())
        {
            if (!itemController.isBeingDragged)
            {
                bool itemConsumed = false;
                if (healthToConsume >= potionBoostAmount && itemController.healthBoost >= potionBoostAmount)
                {
                    itemConsumed = true;
                    healthToConsume -= itemController.healthBoost;
                }
                if (manaToConsume >= potionBoostAmount && itemController.manaBoost >= potionBoostAmount)
                {
                    itemConsumed = true;
                    manaToConsume -= itemController.manaBoost;
                }
                if (staminaToConsume >= potionBoostAmount && itemController.staminaBoost >= potionBoostAmount)
                {
                    itemConsumed = true;
                    staminaToConsume -= itemController.staminaBoost;
                }
                if (itemConsumed)
                {
                    ConsumeItemNow(itemController);
                }
            }
        }

        // next, eat mushrooms
        foreach (ItemController itemController in FindObjectsOfType<ItemController>())
        {
            bool itemConsumed = false;
            if (! itemController.isBeingDragged) { 
                if (healthToConsume > 0 && itemController.healthBoost > 0 && itemController.healthBoost < potionBoostAmount)
                {
                    itemConsumed = true;
                    healthToConsume -= itemController.healthBoost;
                }
                if (manaToConsume > 0 && itemController.manaBoost > 0 && itemController.manaBoost < potionBoostAmount)
                {
                    itemConsumed = true;
                    manaToConsume -= itemController.manaBoost;
                }
                if (staminaToConsume > 0 && itemController.staminaBoost > 0 && itemController.staminaBoost < potionBoostAmount)
                {
                    itemConsumed = true;
                    staminaToConsume -= itemController.staminaBoost;
                }
                if (itemConsumed)
                {
                    ConsumeItemNow(itemController);
                }
            }
        }

        // if that's still not enough, consume whatever helps (some of which will overheal/overboost)
        foreach (ItemController itemController in FindObjectsOfType<ItemController>())
        {
            if (!itemController.isBeingDragged)
            {
                bool itemConsumed = false;
                if (healthToConsume > 0 && itemController.healthBoost > 0)
                {
                    itemConsumed = true;
                    healthToConsume -= itemController.healthBoost;
                }
                if (manaToConsume > 0 && itemController.manaBoost > 0)
                {
                    itemConsumed = true;
                    manaToConsume -= itemController.manaBoost;
                }
                if (staminaToConsume > 0 && itemController.staminaBoost > 0)
                {
                    itemConsumed = true;
                    staminaToConsume -= itemController.staminaBoost;
                }
                if (itemConsumed)
                {
                    ConsumeItemNow(itemController);
                }
            }
        }

        NoteContentsChanged();
        return (healthToConsume <= 0 && manaToConsume <= 0 && staminaToConsume <= 0);
    }

    private void ConsumeItemNow(ItemController itemController)
    {
        Debug.Log("Hero consumed " + itemController.gameObject);
        PlayAudioClipOfConsumption(itemController);
        GameObject.DestroyImmediate(itemController.gameObject);
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
            PlayAudioClipOfLoss(item.GetComponent<ItemController>());
            GameObject.DestroyImmediate(item); // must destroy immediately so the destroyed item will not count toward the totals
            NoteContentsChanged();
            return false;
        } else { 
            Debug.Log(item.name + " received, placing into slot " + slot);
            SnapToGridSlot(item, slot);
            // check for overlap
            foreach (ItemController otherItemController in FindObjectsOfType<ItemController>())
            {
                if( item.GetComponent<ItemController>() != otherItemController) // it's ok to overlap yourself
                {
                    Vector2Int otherSlot = GridSlot(otherItemController.transform.position);
                    if (slot == otherSlot)
                    {
                        GameObject craftingResult = Craft(item, otherItemController.gameObject); // crafts by side effect
                        // if you can craft, make the crafted object sound. that counts as a successful drop so return true
                        if( craftingResult != null)
                        {
                            NoteContentsChanged();
                            PlayAudioClip(craftingResult.GetComponent<ItemController>());
                            return true;
                        } else
                        { // if you can't craft, make the dropped object sound. return false to snap the dropped object back to its original position
                            PlayAudioClip(item.GetComponent<ItemController>());
                            return false;
                        }
                    }
                }
            }
            PlayAudioClip(item.GetComponent<ItemController>());
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
        string name1 = TypeName(ingredient1.name);
        string name2 = TypeName(ingredient2.name);
        for (recipeNum = 0; recipeNum < recipes.GetLength(0); recipeNum++)
        {
            GameObject recipeIngredient1 = recipes[recipeNum, 0];
            GameObject recipeIngredient2 = recipes[recipeNum, 1];
            GameObject recipeResult = recipes[recipeNum, 2];
            // we don't have to call TypeName on the recipe names because they never change
            if (recipeIngredient1.name.Equals(name1) && recipeIngredient2.name.Equals(name2)) {
                GameObject result = Instantiate<GameObject>(recipeResult, destination.position, destination.rotation, gameObject.transform);
                Debug.Log("Crafted " + ingredient1.name + " + " + ingredient2.name + " into " + result.name);
                GameObject.DestroyImmediate(ingredient1); // must destroy immediately so the ingredients will not count toward the totals
                GameObject.DestroyImmediate(ingredient2);
                return result;
            }
        }
        // no recipes apply - fallthrough case
        return null;
    }

    // My contents have changed
    private void NoteContentsChanged()
    {
        metersController.UpdateMeters();
    }

    public void ReserveGridSlot(Vector3 worldPos3d)
    {
        reservedGridSlot = GridSlot(worldPos3d);
        //Debug.Log("Reserved grid slot " + reservedGridSlot);
    }

    public void ReleaseGridSlot()
    {
        reservedGridSlot = new Vector2Int(-1, -1);
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

    private void SnapToGridSlot(GameObject item, Vector2Int slot)
    {
        Vector2 offsetToOrigin = -1 * spriteSize / 2;
        Vector2 itemSizeOffset = slotSize / 2;
        Vector2 slotOffset = slot * slotSize;
        Vector2 offset = offsetToOrigin + slotOffset + itemSizeOffset;
        //Debug.Log("SnapToGridSlot: " + offsetToOrigin + " " + slotOffset + " " + itemSizeOffset);
        item.transform.localPosition = ZeroZ(offset);
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
            if (GridSlot(itemController.transform.position).x >= 0)
            { // item is in backpack
                Vector3Int boostVector = new Vector3Int(itemController.healthBoost, itemController.manaBoost, itemController.staminaBoost);
                total += boostVector;
                //Debug.Log(itemController.gameObject.name + " in slot " + GridSlot(itemController.transform.position) + " contributes " + boostVector + " for a running total of " + total);
            }
        }
        return total;
    }

    public int TotalGold()
        // @return The total number of gold items in the backpack
    {
        int total = 0;
        foreach (ItemController itemController in FindObjectsOfType<ItemController>())
        {
            if (GridSlot(itemController.transform.position).x >= 0 || itemController.isBeingDragged)
            { // item is in backpack (or being dragged)
                string name = TypeName(itemController.name);
                if(name.Equals("Gold"))
                {
                    total++;
                }
            }
        }
        return total;
    }

    public void PlayAudioClip(ItemController itemController)
    {
        //audioSource.pitch = Random.Range(lowPitchRange, highPitchRange);
        audioSource.PlayOneShot(itemController.sound);
    }

    public void PlayAudioClipOfLoss(ItemController itemController)
    {
        audioSource.PlayOneShot(itemController.soundOfLoss);
    }

    public void PlayAudioClipOfConsumption(ItemController itemController)
    {
        audioSource.PlayOneShot(itemController.soundOfConsumption);
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

    public static string TypeName(string str)
    {
        char[] stopChars = { '(' };
        int index = str.IndexOfAny(stopChars);
        if (index > 0)
        {
            return str.Substring(0, index);
        } else
        {
            return str;
        }
    }
}
