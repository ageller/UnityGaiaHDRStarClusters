//https://www.reddit.com/r/Unity3D/comments/dlyihn/controlling_the_positions_of_individual_particles/f4vg1yz/using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

using UnityEngine.VFX;
using UnityEditor.VFX;
using UnityEditor.Experimental.Rendering.HDPipeline;

public class AsciiDataToTexture : MonoBehaviour {

	// path to data file
	public string fileName;     //filename within the data folder
	public int ParticleCount = 10000; //number of particles to read in
	public float gmax = 14.0f; //max gmag in file (for normalization of sizes)
	public float gmin = 2.0f; //min gmag in file (for normalization of sizes)
	public float rmax = 30000.0f; //maximum size of data set in pc (from Bailer-Jones matches to this file)
	//public float minSize = 0.005f;   //this is the tangent(theta) = radius/distance //now set directly in VFX
	// public bool readTextFile = true;
	// public bool saveTextures = true;

	public Texture2D colormap; 

	public GameObject loadingScreen;
	public Slider slider;
	public Text progressText;

	private int maxTexSize = 10000; //Textures can only be 16384 in width (I think)
	private GameObject mainCamera;

	private UnityEngine.Experimental.VFX.VisualEffect vfx;

	private Texture2D PosTex;
	private Texture2D SizeTex;
	private Texture2D ColorTex;

	private int progressInterval;
	private float progress = 0.0f;

	private string dataFileName;
	private Color[] positions;
	private Color[] sizes;
	private Color[] colors;

	private int[] scaledTeff;
	private int colormapWidth;

	private Thread thread;

	// void SaveTextureToFile(Texture2D texture, string filename){ 
	// 	//I'm not sure if this is working
	// 	// Encode texture into the EXR
	// 	//  https://docs.unity3d.com/ScriptReference/ImageConversion.EncodeToEXR.html
	// 	byte[] bytes = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
	// 	System.IO.File.WriteAllBytes(Application.dataPath+"/Resources/"+filename+".exr", bytes);


	// 	//System.IO.File.WriteAllBytes(Application.dataPath+"/Textures/"+filename, texture.EncodeToPNG());
	// }

	// Texture2D LoadTextureFromFile(string filename){ 
	// 	//this doesn't work!

	// 	ParticleCount = 10000;
	// 	int width = (int) Mathf.Min(ParticleCount, maxTexSize);
	// 	int height = (int) Mathf.Round(Mathf.Ceil(ParticleCount/maxTexSize));

	// 	byte[] bytes = System.IO.File.ReadAllBytes(Application.dataPath+"/Resources/"+filename+".exr");
	// 	Debug.Log(bytes.Length);
	// 	Texture2D texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
	// 	texture.LoadRawTextureData(bytes);
	// 	texture.Apply();


	// 	// byte[] bytes = System.IO.File.ReadAllBytes(Application.dataPath+"/Resources/"+filename+".exr");
	// 	// Texture2D texture = new Texture2D(1,1, TextureFormat.RGBAFloat, false);
	// 	// texture.LoadImage(bytes);

	// 	return texture;
	// }
	// void LoadTextures(){
	// 	//not using this because LoadTextureFromFile doesn't work
	// 	PosTex = LoadTextureFromFile("VFXGaiaPosTexture");
	// 	SizeTex = LoadTextureFromFile("VFXGaiaSizeTexture");
	// 	ColorTex = LoadTextureFromFile("VFXGaiaColorTexture");

	// 	//get particle count
	// 	//this may not be exactly correct, but it's hopefully close enough
	// 	ParticleCount = PosTex.width*PosTex.height;

	// 	//how can I get rmax?
	// 	rmax = 10000f;

	// 	Debug.Log(ParticleCount+" "+rmax);

	// }

	// void SetVFXTextures(){
	// 	vfx.SetTexture("VFXGaiaPosTexture", PosTex);
	// 	vfx.SetTexture("VFXGaiaSizeTexture", SizeTex);
	// 	vfx.SetTexture("VFXGaiaColorTexture", ColorTex);

	// }

	void ReadAsciiFile(){
		//read in the file

		// float gmin = 999.0f;
		// float gmax = -999.0f;

		StreamReader reader = new StreamReader(dataFileName);
		string line;

		// // can I use StreamReader to just get the first and last lines in the file, for gmin, gmax, and also to count the file length?
		// // for this case, I need to read in the data twice ,first to get the scaling factors, and next to define the particles
		// for (int i = 0; (line = reader.ReadLine()) != null && i < ParticleCount; i++) {
		// 	string[] data = line.Split();
		// 	if (float.Parse(data[7]) > gmax) gmax = float.Parse(data[7]);
		// 	if (float.Parse(data[7]) < gmin) gmin = float.Parse(data[7]);

		// 	if (Mathf.Abs(float.Parse(data[0])) > rmax) rmax = float.Parse(data[0]);
		// 	if (Mathf.Abs(float.Parse(data[1])) > rmax) rmax = float.Parse(data[1]);
		// 	if (Mathf.Abs(float.Parse(data[2])) > rmax) rmax = float.Parse(data[2]);
		// }
		// reader.Close();



		Debug.Log(gmax+" "+gmin+" "+rmax+" "+ParticleCount);

		// Set all of your particle positions in the texture
		positions = new Color[ParticleCount];
		sizes = new Color[ParticleCount];
		colors = new Color[ParticleCount];
		scaledTeff = new int[ParticleCount];

		float size = 1.0f;
		reader = new StreamReader(dataFileName);
		for (int i = 0; (line = reader.ReadLine()) != null && i < ParticleCount; i++) {
			string[] data = line.Split();
			//Debug.Log(i+" "+ParticleCount);

			positions[i] = new Color(float.Parse(data[0])/rmax, float.Parse(data[1])/rmax, float.Parse(data[2])/rmax, 1.0f);

			//use the magnitude to scale the particle size
			size = Mathf.Pow(10.0f, 1.0f - (float.Parse(data[7]) - gmin)/(gmax - gmin))/10.0f;
			sizes[i] = new Color(size, 0.0f, 0.0f, 0.0f);

			scaledTeff[i] = Mathf.FloorToInt(Mathf.Clamp( ((float.Parse(data[6]) - 1000.0f)/19000.0f), 0.0f, 1.0f)*colormapWidth);
			//colors[i] = colormap.GetPixel(scaledTeff, 0);
			//colors[i] = new Color(scaledTeff, 0.0f, 0.0f, 0.0f);

			if (i % progressInterval == 0) 	{
				progress = Mathf.Clamp01((float)i/(float)ParticleCount);
				//Debug.Log(progress);
			}
		}
		reader.Close();
		progress = 1.0f;




	}

	void setTextures(){

		//I can't do this in the thread, but this is very fast
		for (int i = 0; i < ParticleCount; i++) colors[i] = colormap.GetPixel(scaledTeff[i], 0);

		int width = (int) Mathf.Min(ParticleCount, maxTexSize);
		int height = (int) Mathf.Round(Mathf.Ceil(ParticleCount/maxTexSize));

		// Create texture
		PosTex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
		PosTex.SetPixels(positions);
		PosTex.Apply();

		SizeTex = new Texture2D(width, height, TextureFormat.RFloat, false);
		//SizeTex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
		SizeTex.SetPixels(sizes);
		SizeTex.Apply();

		ColorTex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
		ColorTex.SetPixels(colors);
		ColorTex.Apply();

		// if (saveTextures){
		// 	SaveTextureToFile(PosTex, "VFXGaiaPosTexture");
		// 	SaveTextureToFile(SizeTex, "VFXGaiaSizeTexture");
		// 	SaveTextureToFile(ColorTex, "VFXGaiaColorTexture");
		// }

		vfx.SetInt("VFXGaiaParticleCount", ParticleCount);
		vfx.SetFloat("VFXGaiaRScale", rmax);
		vfx.SetTexture("VFXGaiaPosTexture", PosTex);
		vfx.SetTexture("VFXGaiaSizeTexture", SizeTex);
		vfx.SetTexture("VFXGaiaColorTexture", ColorTex);
	}


	void Awake() {
		thread = new Thread(ReadAsciiFile);
		progress = 0;
		progressInterval = (int) Mathf.Round(ParticleCount/1000);
	}

	void Start(){
		mainCamera = GameObject.Find("MainCamera");
		vfx = this.GetComponent<UnityEngine.Experimental.VFX.VisualEffect>();
		dataFileName = Application.dataPath+"/Data/"+fileName;
		colormapWidth = colormap.width;

		StartCoroutine(LoadingBar());
		StartCoroutine(RunOffMainThread());


	}

	void Update(){
		vfx.SetVector3("VFXCameraPosition", mainCamera.transform.position);
	}


	IEnumerator LoadingBar(){
		while (progress < 1){
			//Debug.Log(progress);
			slider.value = progress;
			progressText.text = (progress*100.0f).ToString("0.00")+"%";
			yield return null;
		}

		loadingScreen.SetActive(false);

	}

	IEnumerator RunOffMainThread(){
		thread.Start();

		while(thread.IsAlive) yield return null;

		Debug.Log("setting textures");

		setTextures();
	}


}
