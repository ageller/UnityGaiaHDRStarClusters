using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateStarClusterSphere : MonoBehaviour
{

	public string fileName;     //filename within the data folder
	public Material material;
	public Color color;

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
			sizes[i] = float.Parse(data[4]);
			//positions[i] = new Vector3(1f,0f,0f);
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

			//I might need a second to show the back faces...

			var renderer = sphere.GetComponent<Renderer>();
			renderer.sharedMaterial = material;
			renderer.sharedMaterial.color = color;
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
