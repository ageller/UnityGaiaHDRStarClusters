using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class DropdownController : MonoBehaviour
{


	private Dropdown dropdown;

	void Start(){
		dropdown = GetComponent<Dropdown>();
		dropdown.onValueChanged.AddListener(delegate{ 
			DropdownItemSelection(dropdown);
		});

	}

	//It would be nice if this was the same script as I use for the RayCastCluster
	void DropdownItemSelection(Dropdown dropdown){
		int index = dropdown.value;
		string name = dropdown.options[index].text;

		Transform tr = 	GameObject.Find(name).transform;

		//if would be nice if this could lerp to that position
		GameObject.Find("CameraTarget").transform.position = tr.position;
		GameObject.Find("MainCamera").GetComponent<MouseOrbitImproved>().distance = 5f;
	}
}
