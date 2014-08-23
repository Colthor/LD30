using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	public GameScript globalScript;
	
	float m_thrustForce = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// FixedUpdate is called a fixed number of times per second, so should be used
	// for physics stuff - eg. the controls.
	void FixedUpdate()
	{
		if(globalScript.IsControllingPlayer())
		{
			if(Input.GetKey(globalScript.UpKey))
			{
				rigidbody2D.AddForce(new Vector2(0f, m_thrustForce));
			}
			if(Input.GetKey(globalScript.LeftKey))
			{
				rigidbody2D.AddForce(new Vector2(-m_thrustForce, 0f));
			}
			if(Input.GetKey(globalScript.RightKey))
			{
				rigidbody2D.AddForce(new Vector2(m_thrustForce, 0f));
			}
		}
	}
}
