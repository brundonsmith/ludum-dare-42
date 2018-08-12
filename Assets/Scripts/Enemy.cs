using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour {

	public readonly static int ManaPerAttack = 3;
	public readonly static int StaminaPerAttack = 1;

	public int health;
	public int damage;

	private ScrollManager scrollManager;
	private BackpackController backpackController;
	private bool battling = false;

	// Use this for initialization
	void Start () {
		this.scrollManager = FindObjectOfType<ScrollManager>();
		this.backpackController = FindObjectOfType<BackpackController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.name == "hero") {
			scrollManager.Pause();
			this.battling = true;

			/* if(has enough mana) */
			// trigger spell animation
			
		}
	}
}
