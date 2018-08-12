using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour {

	public Sprite openedSprite;
	public GameObject[] backpackItemPrefabs;
	private int containedItemIndex;

	public GameObject ContainedItemPrefab {
		get {
			return this.backpackItemPrefabs[this.containedItemIndex];
		}
	}

	// Use this for initialization
	void Start () {
		this.containedItemIndex = Mathf.FloorToInt(Random.Range(0, backpackItemPrefabs.Length));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("Triggered chest");
		if(other.name == "hero") {
			BackpackController backpack = FindObjectOfType<BackpackController>();
			backpack.ReceiveItemFromHero(Instantiate(this.ContainedItemPrefab, backpack.transform));
			this.GetComponent<SpriteRenderer>().sprite = this.openedSprite;
		}
	}
}
