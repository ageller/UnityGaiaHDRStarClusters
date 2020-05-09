using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	private InputField input;

	void Start(){
		input = GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate{ 
			InputItemSelection(input);
		});

	}


	//It would be nice if this was the same script as I use for the RayCastCluster
	void InputItemSelection(InputField input){
		
		string name = input.text;

		var item = GameObject.Find(name);
		if (item != null){
			Transform tr = 	item.transform;

			//if would be nice if this could lerp to that position
			GameObject.Find("CameraTarget").transform.position = tr.position;
			GameObject.Find("MainCamera").GetComponent<MouseOrbitImproved>().distance = 5f;
		}
	}
}
