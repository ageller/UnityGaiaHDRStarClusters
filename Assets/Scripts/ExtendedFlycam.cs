//https://wiki.unity3d.com/index.php/FlyCam_Extended

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ExtendedFlycam : MonoBehaviour
{
 
	/*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
				  Q:    Climb
				  E:    Drop
					  Shift:    Move faster
					Control:    Move slower
						End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/
 
	public float cameraSensitivity = 90;
	public float climbSpeed = 1f;
	public float normalMoveSpeed = 1f;
	public float slowMoveFactor = 0.1f;
	public float fastMoveFactor = 10;
 
	private float rotationX = 0.0f;
	private float rotationY = 0.0f;

	void Start ()
	{
		rotationX = transform.rotation.x;
		rotationY = transform.rotation.y;
		//Screen.lockCursor = true;
	}
 
	void Update ()
	{
		if (Input.GetMouseButton(0)){
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
		}
		rotationY = Mathf.Clamp (rotationY, -90, 90);
 
		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
 
 		float speedFac = 1.0f;
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
		{
			speedFac = fastMoveFactor;
		}
		else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
		{
			speedFac = slowMoveFactor;

		}

		transform.position += transform.forward * normalMoveSpeed * speedFac * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * normalMoveSpeed * speedFac * Input.GetAxis("Horizontal") * Time.deltaTime;
		
		if (Input.GetKey (KeyCode.Q)) {transform.position += transform.up * climbSpeed * speedFac * Time.deltaTime;}
		if (Input.GetKey (KeyCode.E)) {transform.position -= transform.up * climbSpeed * speedFac * Time.deltaTime;}
 
		if (Input.GetKeyDown (KeyCode.End))
		{
			Cursor.visible = (Cursor.visible == false) ? true : false;
		}
	}
}