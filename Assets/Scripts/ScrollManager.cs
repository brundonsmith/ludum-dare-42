using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Transform:{
// 	position,
// 	rotation,
// 	scale
// }
// vector3 is (position, rotation, scale)
// dirt_strip.position =

public class ScrollManager : MonoBehaviour {

	public float scrollSpeed;
	public float enemyChance;
	public float chestChance;

	public GameObject[] dirtstripPrefabs;
	public GameObject[] enemyPrefabs;
	public GameObject chestPrefab;

	private GameObject hero;
	private GameObject dirt_container;
	private bool paused = false;

	// Use this for initialization
	void Start () {
		hero = GameObject.Find("hero");
		dirt_container = GameObject.Find("dirt_container");
	}

	// Update is called once per frame
	void Update () {
		if(!paused) {
			
			// move everything along and possible create/delete strips
			Transform[] dirt_strips = dirt_container.GetComponentsInChildren<Transform>();
			foreach (Transform dirt_strip in dirt_strips) {
				if(dirt_strip.transform.parent == dirt_container.transform) {
					if (dirt_strip.position.x < -10) {

						//get starting placement of dirt_strip
						float startingPosition = dirt_strip.position.x;
						float positionChange = startingPosition + 21 - getMovement();

						// destroy last dirt strip
						GameObject.Destroy(dirt_strip.gameObject);

						// randomly pick a dirt_strip from the array and create it at start of screen
						createDirtStrip(positionChange);

					} else {
						dirt_strip.position = dirt_strip.position - new Vector3(getMovement(), 0, 0);
					}
				}
			}

			// possibly spawn enemy
			if(Random.value < this.enemyChance) {
				GameObject newEnemy = Instantiate(randomElement(enemyPrefabs));
				newEnemy.transform.parent = dirt_container.transform;
				newEnemy.transform.localPosition = new Vector3(22, 0, 0);
			}

			// possibly spawn chest
			if(Random.value < this.chestChance) {
				GameObject newChest = Instantiate(chestPrefab);
				newChest.transform.parent = dirt_container.transform;
				newChest.transform.localPosition = new Vector3(22, 0, 0);
			}
		}
	}

	void createDirtStrip(float positionChange){
		GameObject new_dirt_strip = GameObject.Instantiate(randomElement(dirtstripPrefabs));
		new_dirt_strip.transform.parent = dirt_container.transform;
		new_dirt_strip.transform.localPosition = new Vector3(positionChange, 0, 0);
	}

	public void Pause() {
		this.paused = true;
		hero.GetComponent<Animator>().speed = 0;
	}

	public void Resume() {
		this.paused = false;
		hero.GetComponent<Animator>().speed = 1;
	}

	private float getMovement() {
		return Time.deltaTime * scrollSpeed;
	}

	private GameObject randomElement(GameObject[] arr) {
		return arr[Mathf.FloorToInt(Random.Range(0, arr.Length))];
	}
}
