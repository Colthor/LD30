using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	public GameScript globalScript;

	public float OxygenMax = 20.0f;
	public float FuelMax = 5.0f;
	public float LateralThrustMod = 0.5f;
	
	public float m_thrustForce = 20.0f;

	public AudioClip ThrustSound;
	public AudioClip CollisionSound;

	int m_insideCount = 0;
	float m_Oxygen;
	float m_Fuel;
	
	bool m_thrustUp = false;
	bool m_thrustLeft = false;
	bool m_thrustRight = false;

	bool m_isDead = false;

	public bool IsInside()
	{
		return m_insideCount > 0;
	}

	public float FuelFractionRemain()
	{
		return m_Fuel/FuelMax;
	}

	public float OxygenFractionRemain()
	{
		return m_Oxygen/OxygenMax;
	}

	public bool IsDead()
	{
		return m_isDead;
	}



	// Use this for initialization
	void Start ()
	{
	
		m_Oxygen = OxygenMax;
		m_Fuel = FuelMax;
		m_isDead = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_isDead)
		{
			//Reset controls?
			m_thrustUp = false;
			m_thrustRight = false;
			m_thrustLeft = false;
			particleSystem.enableEmission = false;
			return;
		}
		
		if(globalScript.IsControllingPlayer())
		{
			m_thrustUp = Input.GetKey(globalScript.UpKey1) || Input.GetKey(globalScript.UpKey2);
			m_thrustLeft = Input.GetKey(globalScript.LeftKey1) || Input.GetKey(globalScript.LeftKey2);
			m_thrustRight = Input.GetKey(globalScript.RightKey1) || Input.GetKey(globalScript.RightKey2);
		}
		
		
		if(m_thrustUp)
		{
			m_Fuel -= Time.deltaTime;
		}
		if(m_thrustLeft || m_thrustRight)
		{
			m_Fuel -= LateralThrustMod * Time.deltaTime;
		}
		if(m_Fuel < 0f)
		{
			//Debug.Log("Fuel empty");
			m_Fuel = 0f;
		}
		if((m_thrustUp || m_thrustLeft || m_thrustRight) && m_Fuel > 0f)
		{
			particleSystem.enableEmission = true;
			audio.loop = true;
			if(!audio.isPlaying || audio.time > 0.3f) audio.Play ();
		}
		else
		{
			audio.loop = false;
			particleSystem.enableEmission = false;
		}

		if(!IsInside())
		{
			m_Oxygen -= Time.deltaTime;
		}
		else
		{
			m_Oxygen = OxygenMax;
		}

		if(m_Oxygen <= 0f)
		{
			Debug.Log("You died!");
			m_isDead = true;
			//Destroy(this);
		}
	}

	public void SetInside(bool IsInside)
	{
		if(IsInside)
		{
			m_insideCount++;
		}
		else
		{
			m_insideCount--;
		}
	}

	// FixedUpdate is called a fixed number of times per second, so should be used
	// for physics stuff - eg. the controls.
	void FixedUpdate()
	{
		if(globalScript.IsControllingPlayer() && m_Fuel > 0f)
		{
			if(m_thrustUp)
			{
				rigidbody2D.AddForce(new Vector2(0f, m_thrustForce));
			}
			if(m_thrustLeft)
			{
				rigidbody2D.AddForce(new Vector2(-m_thrustForce, 0f));
			}
			if(m_thrustRight)
			{
				rigidbody2D.AddForce(new Vector2(m_thrustForce, 0f));
			}
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "ShipChunk")
		{
			//audio.pitch = Random.Range(0.8f, 1.2f);
			audio.PlayOneShot (CollisionSound);
		}
	}

	//If we have our feet on a chunk, recharge fuel
	void OnCollisionStay2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "ShipChunk" && coll.contacts[0].point.y < transform.position.y)
		{
			//Debug.Log("Fuel charging");
			m_Fuel = Mathf.Min(m_Fuel + Time.deltaTime, FuelMax);
		}
	}
}
