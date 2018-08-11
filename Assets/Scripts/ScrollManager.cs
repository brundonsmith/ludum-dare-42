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

	public GameObject[] dirtstripPrefabs;
	public float scrollSpeed;

	private GameObject dirt_container;

	// Use this for initialization
	void Start () {
		dirt_container = GameObject.Find("dirt_container");
	}

	// Update is called once per frame
	void Update () {
		float speedChange = Time.deltaTime * scrollSpeed;

		Transform[] dirt_strips = dirt_container.GetComponentsInChildren<Transform>();

		foreach (Transform dirt_strip in dirt_strips) {
			if(dirt_strip.transform.parent == dirt_container.transform) {
				if (dirt_strip.position.x < -10) {

					//get starting placement of dirt_strip
					float startingPosition = dirt_strip.position.x;

					// destroy last dirt strip
					GameObject.Destroy(dirt_strip.gameObject);

					// randomly pick a dirt_strip from the array and create it at start of screen
					float randomNum = Random.Range(0, dirtstripPrefabs.Length);
					int index = Mathf.FloorToInt(randomNum);
					GameObject new_dirt_strip = GameObject.Instantiate(dirtstripPrefabs[index], new Vector3(startingPosition + 19 - speedChange, 0, 0), Quaternion.identity);
					new_dirt_strip.transform.parent = dirt_container.transform;

				} else {
					dirt_strip.position = dirt_strip.position - new Vector3(speedChange, 0, 0);
				}
			}
		}

	}
}
