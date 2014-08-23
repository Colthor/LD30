using UnityEngine;
using System.Collections;

public class ShipChunkScript : MonoBehaviour {
	public GameScript globalScript;

	public enum ChunkType
	{
		BaseChunk,
		NormalChunk,
		PowerChunk,
		TeleporterChunk
	}
	
	string[] ChunkLayouts =
	{
		  "XXX0X"
		+ "X000X"
		+ "X000X"
		+ "X000X"
		+ "XXXXX",

		  "XXXXX"
		+ "00000"
		+ "XXXXX"
		+ "XXXXX"
		+ "XXXXX",
		
		  "XXXXX"
		+ "00000"
		+ "XXXXX"
		+ "00000"
		+ "XXXXX",
		
		  "XXXXX"
		+ "0000X"
		+ "XXX0X"
		+ "XXX0X"
		+ "XXX0X",
		
		  "X0XXX"
		+ "00XXX"
		+ "XXXXX"
		+ "XXXXX"
		+ "XXXXX",
		
		  "XXX0X"
		+ "0000X"
		+ "XXXXX"
		+ "XXXXX"
		+ "XXXXX",
		
		  "X0X0X"
		+ "00X0X"
		+ "XXX0X"
		+ "0000X"
		+ "XXXXX",
		
		  "X0XXX"
		+ "00XXX"
		+ "XXXXX"
		+ "XXX00"
		+ "XXX0X"
	};

	float m_thrustForce = 20000.0f;
	ChunkType m_myType = ChunkType.NormalChunk;

	public bool m_connectedWithBase=false;
	int m_collideWithConnectedCount = 0;

	// Use this for initialization
	void Start ()
	{
	
	}

	public void InitialiseChunk(ChunkType chType)
	{
		int layoutNum = 0;
		m_myType = chType;

		switch(m_myType)
		{
			case ChunkType.BaseChunk:
			m_connectedWithBase = true;
			break;

			case ChunkType.PowerChunk:
			break;

			case ChunkType.TeleporterChunk:
			break;

			case ChunkType.NormalChunk:
				layoutNum = Random.Range(0, ChunkLayouts.Length);
			break;
		}
		
		TileMapScript tms = GetComponent<TileMapScript>();
		tms.LevelData = ChunkLayouts[layoutNum];
		tms.GenerateMap();

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	/*public bool ConnectedWithBase()
	{
		return m_connectedWithBase;
	}*/

	public bool TouchingBase()
	{
		return m_collideWithConnectedCount > 0;
	}

	// FixedUpdate is called a fixed number of times per second, so should be used
	// for physics stuff - eg. the controls.
	void FixedUpdate()
	{
		if(ChunkType.BaseChunk == m_myType && !globalScript.IsControllingPlayer())
		{
			if(Input.GetKey(globalScript.UpKey))
			{
				rigidbody2D.AddForce(new Vector2(0f, m_thrustForce));
			}
			if(Input.GetKey(globalScript.DownKey))
			{
				rigidbody2D.AddForce(new Vector2(0f, -m_thrustForce));
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

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(coll.gameObject.tag == "ShipChunk" && m_connectedWithBase)
		{
			ShipChunkScript scs = coll.gameObject.GetComponent<ShipChunkScript>();
			if(!scs.m_connectedWithBase)
			{
				/*Vector2 anchorPoint1 = new Vector2(0f, 2f);
				Vector2 anchorPoint2 = new Vector2(2f, -3f);
				Vector2 anchorPoint3 = new Vector2(-2f, -3f);*/
				m_collideWithConnectedCount++;
				scs.m_connectedWithBase = true;

				coll.gameObject.transform.parent = transform;
				Destroy(coll.gameObject.rigidbody2D);

				/*DistanceJoint2D joint = gameObject.AddComponent<DistanceJoint2D>();
				joint.collideConnected = true;
				joint.connectedBody = coll.gameObject.rigidbody2D;
				joint.distance = Vector3.Distance(coll.gameObject.transform.position, transform.position);
				joint.anchor = anchorPoint1;
				joint.connectedAnchor = anchorPoint1;
				scs.m_connectedWithBase = true;

				joint = gameObject.AddComponent<DistanceJoint2D>();
				joint.collideConnected = true;
				joint.connectedBody = coll.gameObject.rigidbody2D;
				joint.distance = Vector3.Distance(coll.gameObject.transform.position, transform.position);
				joint.anchor = -anchorPoint2;
				joint.connectedAnchor = -anchorPoint2;
				
				joint = gameObject.AddComponent<DistanceJoint2D>();
				joint.collideConnected = true;
				joint.connectedBody = coll.gameObject.rigidbody2D;
				joint.distance = Vector3.Distance(coll.gameObject.transform.position, transform.position);
				joint.anchor = -anchorPoint3;
				joint.connectedAnchor = -anchorPoint3;*/

			}
		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if(coll.gameObject.tag == "ShipChunk")
		{
			ShipChunkScript scs = coll.gameObject.GetComponent<ShipChunkScript>();
			if(scs.m_connectedWithBase ^ m_connectedWithBase)
			{
				m_collideWithConnectedCount--;
			}
		}
	}
}
