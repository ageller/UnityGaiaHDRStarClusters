using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GenerateStarClusterSphere : MonoBehaviour
{

	public string fileName;     //filename within the data folder
	//public Material material;
	public Color color;
	public Font font;
	public int fontsize = 6;

	private string dataFileName;
	private int ParticleCount = 0; 
	private int maxRows = 3500; //maximum number of rows to consider (there are ~3500 OCs in the file)
	private Vector3[] positions;
	private float[] sizes;
	private string[] names;

	void ReadAsciiFile(){
		//read in the file

		StreamReader reader = new StreamReader(dataFileName);
		string line;

		// for this case, I need to read in the data twice ,first to get the number of particles, and next to define the particles
		for (int i = 0; (line = reader.ReadLine()) != null && i < maxRows; i++) ParticleCount ++;
		reader.Close();

		//ParticleCount = 1;
		Debug.Log("Number of star clusters "+ParticleCount);

		// Set all of your particle positions in the texture
		positions = new Vector3[ParticleCount];
		sizes = new float[ParticleCount];
		names = new string[ParticleCount];

		reader = new StreamReader(dataFileName);
		for (int i = 0; (line = reader.ReadLine()) != null && i < ParticleCount; i++) {
			string[] data = line.Split();

			names[i] = data[0];
			positions[i] = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
			sizes[i] = float.Parse(data[4])*2f; //want a diameter I think
			//positions[i] = new Vector3(10f,10f,10f);
			//sizes[i] = 100f;
		}
		reader.Close();
	}

	void createSpheres(){
		for (int i = 0; i < ParticleCount; i++) {
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.name = names[i];
			sphere.transform.position = positions[i];
			sphere.transform.localScale = new Vector3(sizes[i], sizes[i], sizes[i]);
			sphere.transform.SetParent(transform);

			WireframeRenderer wr = sphere.AddComponent<WireframeRenderer>();
			wr.LineColor = color;
			wr.Scale = sizes[i];
			wr.Center = positions[i];
			wr.ShowBackFaces = true;
			// wr.enabled = false;


			// var renderer = sphere.GetComponent<Renderer>();
			// renderer.sharedMaterial = material;
			// renderer.sharedMaterial.color = color;

			//also add a canvas with a label containing the name
			// Canvas
			//https://docs.unity3d.com/ScriptReference/Canvas.html
			GameObject myGO = new GameObject();
			myGO.name = "Canvas";
			myGO.transform.SetParent(sphere.transform);
			//myGO.transform.position = new Vector3(positions[i].x, positions[i].y + 0.1f, positions[i].z);
			
			Canvas myCanvas = myGO.AddComponent<Canvas>();
			//myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			myCanvas.renderMode = RenderMode.WorldSpace;
			myCanvas.transform.localPosition = new Vector3(0f, 1f, 0f); //not sure what the units are here

			// myGO.AddComponent<CanvasScaler>();
			// myGO.AddComponent<GraphicRaycaster>();

			// Text
			GameObject myText = new GameObject();
			myText.name = "Text";
			myText.transform.SetParent(myGO.transform);
			myText.transform.localPosition = new Vector3(0f,0f,0f);



			Text text = myText.AddComponent<Text>();
			text.text = names[i];
			text.color = color;
			text.font = font;
			text.fontSize = fontsize;
			text.material = font.material;
			text.alignment = TextAnchor.MiddleCenter;

			var rectTransform = text.transform as RectTransform;
			rectTransform.sizeDelta = new Vector2(500f, rectTransform.sizeDelta.y);
			//I need to set the width to 500 and center the text
			//Also I want to make the text always face the camera and also remain the same size on the screen
		}

	}
	// Start is called before the first frame update
	void Start(){
		dataFileName = Application.dataPath+"/Data/"+fileName;
		ReadAsciiFile();
		createSpheres();

	}

	// Update is called once per frame
	void Update(){
		
	}
}
