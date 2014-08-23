using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraSizer : MonoBehaviour {

	private int PrevWidth;
	private int PrevHeight;
	private float PrevPPU;
	public float PixelsPerUnit = 100f;

	// Use this for initialization
	void Start () {
		ResizeCamera();
	}

	void ResizeCamera()
	{
		PrevWidth = Screen.width;
		PrevHeight = Screen.height;
		PrevPPU = PixelsPerUnit;
		camera.orthographicSize = PrevHeight / (2.0f * PixelsPerUnit);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(!(PrevWidth == Screen.width && PrevHeight == Screen.height && PixelsPerUnit == PrevPPU))
		{
			ResizeCamera();
		}
	}
}
