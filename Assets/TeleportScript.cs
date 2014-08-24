using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {

	public GameScript globalScript;

	void Start()
	{
		//Debug.Log("Teleporter created");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//Debug.Log("Teleporter OnTriggerEnter");
		if(other.gameObject.tag == "Spaceonaut")
		{
			if(null != globalScript)
			{
				globalScript.AttemptTeleport();
			}
		}
	}
}
