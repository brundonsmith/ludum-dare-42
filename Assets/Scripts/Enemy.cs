using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour {

	public readonly static int ManaPerAttack = 3;
	public readonly static int StaminaPerAttack = 1;
	public readonly static float turnLength = 2; // seconds

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

	// battle state
	private float lastAction;
	private bool enemyTurn = true;
	
	// Update is called once per frame
	void Update () {
		if(this.battling && Time.time - lastAction > turnLength) {
			lastAction = Time.time;

			bool spellCast = backpackController.ConsumeMana(ManaPerAttack);
			if(spellCast) {
				// TODO: play animation
				Debug.Log("Enemy died from spell!");
				scrollManager.Resume();
				GameObject.Destroy(this.gameObject);
			} else if(enemyTurn) {
				// TODO: play animation
				Debug.Log("Player took " + this.damage + " damage!");
				bool stillAlive = backpackController.ConsumeHealth(this.damage);
				if(!stillAlive) {
					this.battling = false;
					GameObject.Destroy(GameObject.Find("hero"));
					Debug.Log("Game Over");
				}
			} else {
				// TODO: play animation
				bool strongHit = backpackController.ConsumeStamina(StaminaPerAttack);
				if(strongHit) {
					Debug.Log("Player spent " + StaminaPerAttack + " stamina to do 2 damage!");
					this.health -= 2;
				} else {
					Debug.Log("Player did 1 damage!");
					this.health -= 1;
				}

				if(this.health <= 0) {
					// TODO: play animation
					Debug.Log("Enemy died from attack!");
					scrollManager.Resume();
					GameObject.Destroy(this.gameObject);
				}
			}			
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.name == "hero") {
			Debug.Log("Battle started!");
			scrollManager.Pause();
			this.battling = true;
			this.lastAction = Time.time;
		}
	}
}
