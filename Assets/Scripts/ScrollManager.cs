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

	public readonly static int GoldWorth = 100; // how many points is each pile of gold coins worth?

    public float scrollSpeed;
	public float enemyChance;
    public float chestChance;
    public float metersUntilDifficultyIncrease = 50; // the difficulty increases by one notch every ___ meters
    public float enemyIncreaseAmount = 1.3F; // each time the difficulty increases, multiply enemyChance and scrollSpeed by this amount
    public float chestIncreaseAmount = 1.27F; // each time the difficulty increases, multiply chestChance by this amount
    public float pitchIncreaseAmount = 1.1F; // each time the difficulty increases, increase the pitch of the music by this amount

	public GameObject[] dirtstripPrefabs;
	public GameObject[] enemyPrefabs;
	public GameObject chestPrefab;

	private GameObject hero;
	private GameObject dirt_container;
    private GameObject music;
    private BackpackController backpackController;
	private bool paused = false;
	private bool gameOver = false;

	private float metersMoved = 0;
    private float lastCheckpoint = 0; // used for notching up the difficulty

	// Use this for initialization
	void Start () {
		hero = GameObject.Find("hero");
		dirt_container = GameObject.Find("dirt_container");
        backpackController = GameObject.Find("Backpack").GetComponent<BackpackController>();
        music = GameObject.Find("Music");
    }

	// Update is called once per frame
	void Update () {
		if(!paused && !gameOver) {
			
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

			metersMoved += getMovement();

            PossiblyIncreaseDifficulty();
        }
	}

    private void PossiblyIncreaseDifficulty()
    {
        if (metersMoved > lastCheckpoint + metersUntilDifficultyIncrease)
        {
            lastCheckpoint = lastCheckpoint + metersUntilDifficultyIncrease;
            enemyChance *= enemyIncreaseAmount;
            scrollSpeed *= enemyIncreaseAmount;
            chestChance *= chestIncreaseAmount;
            AudioSource musicSource = music.GetComponent<AudioSource>();
            musicSource.pitch *= pitchIncreaseAmount;
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

	public void GameOver() {
		this.gameOver = true;
	}

	private float getMovement() {
		return Time.deltaTime * scrollSpeed;
	}

	private GameObject randomElement(GameObject[] arr) {
		return arr[Mathf.FloorToInt(Random.Range(0, arr.Length))];
	}

	void OnGUI() {
		if(!gameOver) {
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.black;
			style.fontSize = 24;
			style.fontStyle = FontStyle.Bold;
			GUI.Label(new Rect(10, 10, 100, 20), "Distance: " + Mathf.Round(metersMoved), style);
			GUI.Label(new Rect(10, 30, 100, 20), "Gold (100 each): " + GoldWorth * backpackController.TotalGold(), style);
		} else {
			GUIStyle gameOverStyle = new GUIStyle();
			gameOverStyle.normal.textColor = Color.red;
			gameOverStyle.fontSize = 72;
			gameOverStyle.fontStyle = FontStyle.Bold;
			gameOverStyle.alignment = TextAnchor.LowerCenter;
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 20), "You Are Strapped To A Corpse", gameOverStyle);
			
			GUIStyle scoreStyle = new GUIStyle();
			scoreStyle.normal.textColor = Color.white;
			scoreStyle.fontSize = 32;
			scoreStyle.fontStyle = FontStyle.Bold;
			scoreStyle.alignment = TextAnchor.LowerCenter;
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 30, 100, 20), "Score: " + Score(), scoreStyle);
		}
	}

    private int Score()
    {
        return (int)Mathf.Round(metersMoved) + backpackController.TotalGold() * GoldWorth;
    }
}
