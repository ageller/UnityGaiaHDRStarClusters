﻿//https://answers.unity.com/questions/547513/how-do-i-detect-when-mouse-passes-over-an-object.html
using System.Collections;
using UnityEngine;

public class RayCastCluster : MonoBehaviour
{

	Ray ray;
	RaycastHit hit;


    // Update is called once per frame
    void Update(){
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)){
			//print(hit.transform.name);
			//if (Input.GetMouseButtonDown(0)){ //could I get a double click?
			if (Input.GetMouseButtonDown(1)){
				GameObject.Find("CameraTarget").transform.position = hit.transform.position;
				GameObject.Find("MainCamera").GetComponent<MouseOrbitImproved>().distance = 5f;
			}
		}
    }
}
