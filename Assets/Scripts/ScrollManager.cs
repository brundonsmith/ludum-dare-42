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


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		GameObject dirt_container = GameObject.Find("dirt_container");

		Transform[] dirt_strips = dirt_container.GetComponentsInChildren<Transform>();

		foreach (Transform dirt_strip in dirt_strips) {
			float speedChange = Time.deltaTime * scrollSpeed;
			dirt_strip.position = dirt_strip.position - new Vector3(speedChange, 0, 0);
		}

	}
}
