﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    // initialized, then left alone
    public AudioClip sound; // the sound it makes when it's (re)placed in an open slot in the backpack, or when it's crafted
    public AudioClip soundOfLoss; // the sound it makes when it gets forcibly ejected from the backpack due to overfilling
    public AudioClip soundOfConsumption; // the sound it makes when it's consumed by the hero
    private GameObject backpack;
    private BackpackController backpackController;

    // variables
    public bool isBeingDragged = false;
    private Vector3 originalItemPosition;
    public int healthBoost = 0;
    public int manaBoost = 0;
    public int staminaBoost = 0;

    // Use this for initialization that depends on other objects
    void Start () {
        //sprite = gameObject.GetComponent<SpriteRenderer>();
        backpack = GameObject.Find("Backpack");
        backpackController = backpack.GetComponent<BackpackController>();
    }
	
	// Update is called once per frame
	void Update () {
        if(isBeingDragged) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
            //Debug.Log(Input.mousePosition + " " + worldPos + " " + worldPos2D);
            transform.position = worldPos2D;
        }
    }

    void OnMouseDown()
    {
        isBeingDragged = true;
        originalItemPosition = this.transform.position;
        backpackController.ReserveGridSlot(originalItemPosition);
    }

    void OnMouseUp()
    {
        isBeingDragged = false;

        BackpackController theBackpackController = backpackController; // @hack to robustify against DestroyImmediate destroying me in the middle of my OnMouseUp event
        if ( ! backpackController.ReceiveItemFromMouse(gameObject))
        {
            if (this != null) { // @hack to robustify against DestroyImmediate destroying me in the middle of my OnMouseUp event
                transform.position = originalItemPosition;
            }
        }
        theBackpackController.ReleaseGridSlot();
    }

}

