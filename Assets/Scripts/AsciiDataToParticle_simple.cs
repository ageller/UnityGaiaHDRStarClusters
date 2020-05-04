using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class AsciiDataToParticle_simple : MonoBehaviour {


	// path to data file
	public TextAsset dataFile;     //drop your data file here in inspector

	public string particleName = "particleSystem";
	public float minSize = 0.002f;
	public float maxSize = 1.0f;

	private float gmin = 999.0f;
	private float gmax = -999.0f;

	private List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
	private ParticleSystemRenderer psr;

	// Read the data and creates the particle system
	public void createParticlesFromFile()
	{
  
		//for this case, I need to read in the data twice ,first to get the scaling factors, and next to define the particles
		string[] allLines = Regex.Split(dataFile.text, "\n|\r|\r\n");
		for (int i=0; i<allLines.Length; i++){
			if (allLines[i].Length > 0){
				string[] data = allLines[i].Split();
				if (float.Parse(data[7]) > gmax) gmax = float.Parse(data[7]);
				if (float.Parse(data[7]) < gmin) gmin = float.Parse(data[7]);
			}
		}
		Debug.Log(gmax+" "+gmin);

		float size = 1.0f;
		for (int i=0; i<allLines.Length; i++){
			if (allLines[i].Length > 0){
				string[] data = allLines[i].Split();

				//use the magnitude to scale the particle size
				size = Mathf.Pow(10.0f, 1.0f - (float.Parse(data[7]) - gmin)/(gmax - gmin))/10.0f;

				ParticleSystem.Particle p = new ParticleSystem.Particle
				{
					position = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2])),
					startSize = size,
					startColor = new Color(1,1,1,1)
				};

				particles.Add(p);
			}
		}


	}


	void Start()
	{
		particles.Clear(); 

		//read from the file, and create the particles
		createParticlesFromFile();

		// add particles into the scene
		GetComponent<ParticleSystem>().SetParticles(particles.ToArray(), particles.Count);
		psr = GetComponent<ParticleSystemRenderer>();
		psr.minParticleSize = minSize;
		psr.maxParticleSize = maxSize;



	}

	//update the particles on each draw pass?
	void Update()
	{

	}
	
}
