using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    // initialized, then left alone
    private SpriteRenderer sprite;
    private GameObject backpack;
    private BackpackController backpackController;
    
    // variables
    private bool isBeingDragged = false;
    private Vector3 originalItemPosition;
    public int healthBoost = 0;
    public int manaBoost = 0;
    public int staminaBoost = 0;

    // Use this for initialization
    void Start () {
        sprite = gameObject.GetComponent<SpriteRenderer>();
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
        //originalItemGridSlot = backpackController.gridSlot(this.transform.position);
        //Debug.Log(gameObject.name + " is being dragged");
    }

    void OnMouseUp()
    {
        isBeingDragged = false;

        if ( ! backpackController.ReceiveItemFromMouse(gameObject))
        {
            transform.position = originalItemPosition;
        }
        /*
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
        
        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
        if (hit.collider != null)
        {
            GameObject collidedObj = hit.collider.gameObject;
            Debug.Log(gameObject.name + " dropped over a " + collidedObj.name);
          

        }
        */
    }

}

