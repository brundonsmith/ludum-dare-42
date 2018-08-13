using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour {

	public readonly static int ManaPerAttack = 3;
	public readonly static int StaminaPerAttack = 1;
	public readonly static float turnLength = 2; // seconds

	public int health;
	public int damage;
	public GameObject spellBlastPrefab;

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
				Instantiate(spellBlastPrefab, GameObject.Find("hero").transform.position + new Vector3(0, 0.5f, -3), Quaternion.identity);
				Debug.Log("Enemy died from spell!");
				GameObject.Destroy(this.gameObject);
				scrollManager.Resume();
			} else if(enemyTurn) {
				this.GetComponentInChildren<Animation>().Play("Attack");
				Debug.Log("Hero took " + this.damage + " damage!");
				bool stillAlive = backpackController.ConsumeHealth(this.damage);
				if(!stillAlive) {
					this.battling = false;
					GameObject.Find("hero").transform.rotation = Quaternion.Euler(0, 0, 180);
					GameObject.Find("hero").transform.position += new Vector3(0, 0.6f, 0);
					scrollManager.GameOver();
					Debug.Log("Game Over");
				}
				enemyTurn = false;
			} else {
				GameObject.Find("hero").GetComponentInChildren<Animator>().SetTrigger("Attack");
				bool strongHit = backpackController.ConsumeStamina(StaminaPerAttack);
				if(strongHit) {
					Debug.Log("Hero spent " + StaminaPerAttack + " stamina to do 2 damage!");
					this.health -= 2;
				} else {
					Debug.Log("Hero did 1 damage!");
					this.health -= 1;
				}

				if(this.health <= 0) {
					Debug.Log("Enemy died from attack!");
					GameObject.Destroy(this.gameObject);
					scrollManager.Resume();
				}
				enemyTurn = true;
			}			
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.name == "hero") {
			Debug.Log("Battle started!");
			scrollManager.Pause();
			this.battling = true;
			this.lastAction = Time.time - turnLength;
		}
	}
}
