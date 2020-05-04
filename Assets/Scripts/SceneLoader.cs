//https://www.youtube.com/watch?v=YMj2qPq9CP8

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

	public GameObject loadingScreen;
	public Slider slider;
	public Text progressText;

	public void LoadIt(int sceneIndex){
		StartCoroutine(LoadAsynchronously(sceneIndex));
	}

	IEnumerator LoadAsynchronously(int sceneIndex){

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		loadingScreen.SetActive(true);

		while (!operation.isDone){
			float progress = Mathf.Clamp01(operation.progress/0.9f); //because there are two steps in loading, 0 - 0.9 (which has steps) and then 0.9 - 1 (which does not have steps)

			Debug.Log(operation.progress+" "+progress);
			slider.value = progress;
			progressText.text = progress*100+"%";
			yield return null;
		}
	}
}
