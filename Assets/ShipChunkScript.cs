using UnityEngine;
using System.Collections;

public class ShipChunkScript : MonoBehaviour {
	public GameScript globalScript;
	public GameObject TeleportPrefab;
	public Material ConnectedMaterial;
	public static int ChunkSquares = 5;
	public static float ChunkSquareSize = 1f;


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
		+ "X444X"
		+ "X444X"
		+ "X444X"
		+ "XXXXX",

		  "XXX0X"
		+ "X555X"
		+ "X565X"
		+ "X555X"
		+ "XXXXX",

		  "XXXXX"
		+ "XEEEX"
		+ "XEEEX"
		+ "XEEEX"
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

	public float m_thrustForce = 2000000.0f;
	public float m_torque = 2000000.0f;


	ChunkType m_myType = ChunkType.NormalChunk;

	public ChunkType GetChunkType()
	{
		return m_myType;
	}

	public bool m_connectedWithBase=false;
	int m_collideWithConnectedCount = 0;

	int m_connectedCount = 1;

	// Use this for initialization
	void Start ()
	{
	
	}

	string CombineLayouts(string BaseLayout, string ToMerge)
	{
		bool rotate = Random.value < 0.5f;
		int xMin = 0, xStep = 1;
		int yMin = 0, yStep = 1;
		string output = "";

		if(Random.value < 0.5f)
		{
			xMin = ChunkSquares-1;
			xStep = -1;
		}
		if(Random.value < 0.5f)
		{
			yMin = ChunkSquares-1;
			yStep = -1;
		}
		int i;
		int basei = 0;
		for(int x = xMin; x < ChunkSquares && x >= 0; x+=xStep)
		{
			for(int y = yMin; y < ChunkSquares && y >= 0; y+=yStep)
			{
				if(rotate)
				{
					i = x * ChunkSquares + y;
				}
				else
				{
					i = y * ChunkSquares + x;
				}

				if('X' != ToMerge[i])
				{
					output += ToMerge[i];
				}
				else
				{
					output += BaseLayout[basei];
				}
				basei++;
			}
		}

		return output;
	}

	public void ConnectChunk()
	{
		GetComponent<MeshRenderer>().material = ConnectedMaterial;
	}

	public void InitialiseChunk(ChunkType chType)
	{
		int layoutNum = 0;
		m_myType = chType;
		GameObject teleporter = null;
		bool MergeLayout = false;

		switch(m_myType)
		{
			case ChunkType.BaseChunk:
				m_connectedWithBase = true;
				layoutNum = 0;
				GetComponent<MeshRenderer>().material = ConnectedMaterial;
			this.name = "BaseChunk";
				break;

			case ChunkType.PowerChunk:
				layoutNum = 2;
				this.name = "PowerChunk";
			break;

			case ChunkType.TeleporterChunk:
				layoutNum = 1;
				this.name = "TeleporterChunk";

				teleporter = (GameObject) Instantiate(TeleportPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
				teleporter.GetComponent<TeleportScript>().globalScript = globalScript;
				teleporter.transform.position = transform.position;
			break;

			case ChunkType.NormalChunk:
				layoutNum = Random.Range(3, ChunkLayouts.Length);
				this.name = "NormalChunk " + layoutNum;
				Vector3 scale = new Vector3(1f, 1f, 1f);
				if(Random.value < 0.5f) scale.x = -1f;
				if(Random.value < 0.5f) scale.y = -1f;
				if(Random.value < 0.75f) MergeLayout = true;
				transform.localScale = scale;
				transform.rotation = Quaternion.AngleAxis( (float) Random.Range(0,4) * 90f, Vector3.forward);
				break;
		}


		TileMapScript tms = GetComponent<TileMapScript>();
		if(MergeLayout)
		{
			tms.LevelData = CombineLayouts(ChunkLayouts[layoutNum], ChunkLayouts[Random.Range(3, ChunkLayouts.Length)]);
		}
		else
		{
			tms.LevelData = ChunkLayouts[layoutNum];
		}
		tms.Squares_X = ChunkSquares;
		tms.Squares_Y = ChunkSquares;
		tms.Square_Size = ChunkSquareSize;
		tms.GenerateMap();

		//This is only here rather than above to work around some bug or obscurity in Unity
		//If it's up there ^ the teleporter isn't created at all. It's odd.
		if(null != teleporter)
		{
			teleporter.transform.parent = transform; //This breaks it?!
		}

	}

	void ConnectJoiningObjects()
	{
		GameObject[] gameObs;
		gameObs = GameObject.FindGameObjectsWithTag("ShipChunk");

		foreach (GameObject go in gameObs)
		{
			ShipChunkScript scs = go.GetComponent<ShipChunkScript>();
			if(!scs.m_connectedWithBase && scs.TouchingBase())
			{
				scs.m_connectedWithBase = true;
				if(scs.GetChunkType() == ChunkType.PowerChunk)
				{
					globalScript.AddPower();
				}
				else if(scs.GetChunkType() == ChunkType.TeleporterChunk)
				{
					globalScript.ConnectTeleporter();
				}
				
				go.transform.parent = transform;
				Destroy(go.rigidbody2D);

				m_connectedCount++;
				scs.ConnectChunk();
				
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(ChunkType.BaseChunk == m_myType && !globalScript.IsControllingPlayer())
		{
			if(Input.GetKeyDown(globalScript.ActionKey))
			{
				ConnectJoiningObjects();
			}
		}
	
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
			if(Input.GetKey(globalScript.RotateLeftKey))
			{
				rigidbody2D.AddTorque(m_connectedCount * m_torque);
			}
			if(Input.GetKey(globalScript.RotateRightKey))
			{
				rigidbody2D.AddTorque(m_connectedCount * -m_torque);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if(coll.gameObject.tag == "ShipChunk" && !m_connectedWithBase)
		{
			ShipChunkScript scs = coll.gameObject.GetComponent<ShipChunkScript>();
			if(scs.m_connectedWithBase)
			{
				m_collideWithConnectedCount++;
				/*scs.m_connectedWithBase = true;

				coll.gameObject.transform.parent = transform;
				Destroy(coll.gameObject.rigidbody2D);*/

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

	//Spaceonaut is inside - tell them
	void OnTriggerEnter2D(Collider2D coll)
	{
		if(coll.gameObject.tag == "Spaceonaut")
		{
			PlayerScript ps = coll.gameObject.GetComponent<PlayerScript>();
			ps.SetInside(true);
		}
	}
	//Spaceonaut is outside - tell them
	void OnTriggerExit2D(Collider2D coll)
	{
		if(coll.gameObject.tag == "Spaceonaut")
		{
			PlayerScript ps = coll.gameObject.GetComponent<PlayerScript>();
			ps.SetInside(false);
		}
	}
}
