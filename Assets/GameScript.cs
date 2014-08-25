using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public KeyCode SwitchControllingKey = KeyCode.Tab;
	public KeyCode UpKey1 = KeyCode.UpArrow;
	public KeyCode DownKey1 = KeyCode.DownArrow;
	public KeyCode LeftKey1 = KeyCode.LeftArrow;
	public KeyCode RightKey1 = KeyCode.RightArrow;
	public KeyCode RotateLeftKey1 = KeyCode.Delete;
	public KeyCode RotateRightKey1 = KeyCode.PageDown;
	public KeyCode UpKey2 = KeyCode.W;
	public KeyCode DownKey2 = KeyCode.S;
	public KeyCode LeftKey2 = KeyCode.A;
	public KeyCode RightKey2 = KeyCode.D;
	public KeyCode RotateLeftKey2 = KeyCode.Q;
	public KeyCode RotateRightKey2 = KeyCode.E;
	public KeyCode ActionKey = KeyCode.Space;
	public KeyCode RestartKey = KeyCode.R;

	public GameObject ShipChunkPrefab;
	public GameObject SpaceonautPrefab;
	public GUISkin UISkin;
	public Texture2D OxygenMeterTexture;
	public Texture2D FuelMeterTexture;

	public int FieldWidth = 6;
	public int FieldHeight = 7;
	public int OuterRows = 1;
	public float GapWidth = 3.0f;
	public float Variance = 1.0f;
	public int PowerChunks = 3;
	public int PowerChunksRequired = 2;


	GameObject player;
	GameObject baseChunk;
	bool controllingPlayer = false;

	int m_powerConnected = 0;
	bool m_teleporterConnected = false;
	bool m_hasWon = false;
	bool m_Restart = false;
	bool m_doTitle = true;

	// Use this for initialization
	void Start ()
	{
		m_doTitle = true;
		//LoadLevel();	
	}

	void Restart()
	{
		m_Restart = true;
		m_doTitle = false;
		if(player)
		{
			Destroy(player);
			player = null;
		}
		baseChunk = null;
		GameObject[] gameObs;
		gameObs = GameObject.FindGameObjectsWithTag("ShipChunk");
		
		foreach (GameObject go in gameObs)
		{
			Destroy(go);
		}
	}

	void WinGame()
	{
		if(player != null)
		{
			PlayerScript ps = player.GetComponent<PlayerScript>();
			if(!ps.IsDead())
			{
				m_hasWon = true;
				Debug.Log("You win! Hurrah!");
				Destroy(player);
				player = null;
			}
		}
	}

	public void AddPower()
	{
		m_powerConnected ++;
	}

	public void ConnectTeleporter()
	{
		m_teleporterConnected = true;
	}

	public bool AttemptTeleport()
	{
		if (m_teleporterConnected && m_powerConnected >= PowerChunksRequired)
		{
			WinGame();
			return true;
		}
		else
		{
			Debug.Log("We dinnae ha the pooer!");
			return false;
		}
	}

	public bool IsControllingPlayer()
	{
		return controllingPlayer;
	}

	public void InitEasy()
	{
		FieldWidth = 5;
		FieldHeight = 6;
		OuterRows = 1;
		GapWidth = 4.0f;
		PowerChunks = 3;
		PowerChunksRequired = 2;
	}
	
	public void InitNormal()
	{
		FieldWidth = 6;
		FieldHeight = 7;
		OuterRows = 1;
		GapWidth = 3.0f;
		PowerChunks = 3;
		PowerChunksRequired = 2;
	}

	public void InitHard()
	{
		FieldWidth = 8;
		FieldHeight = 9;
		OuterRows = 2;
		GapWidth = 2.5f;
		PowerChunks = 4;
		PowerChunksRequired = 2;
	}

	public void LoadLevel()
	{
		m_powerConnected = 0;
		m_teleporterConnected = false;
		m_hasWon = false;
		m_Restart = false;
		m_doTitle = false;

		particleSystem.Stop();
		particleSystem.Clear();
		particleSystem.Play();


		//This should either get the Squares_* from TileMapScript, or set it, rather than being hardcoded...
		float xSpread = ShipChunkScript.ChunkSquares * ShipChunkScript.ChunkSquareSize + GapWidth; // (tms.Squares_X * tms.Square_Size + GapWidth);
		float ySpread = ShipChunkScript.ChunkSquares * ShipChunkScript.ChunkSquareSize + GapWidth; // (tms.Squares_Y * tms.Square_Size + GapWidth);

		int powerChunksRemaining = PowerChunks;
		int chunksRemaining = FieldWidth * FieldHeight;

		for(int x = 0; x < FieldWidth; x++)
		{
			for(int y = 0; y < FieldHeight; y++)
			{
				Vector3 pos =  new Vector3((float)x * xSpread, (float)y * ySpread, 10f);
				Vector2 offset = Random.insideUnitCircle * Variance;
				pos.x += offset.x;
				pos.y += offset.y;
				GameObject chunk = (GameObject) Instantiate(ShipChunkPrefab, pos, Quaternion.identity);
				ShipChunkScript.ChunkType typeToAdd = ShipChunkScript.ChunkType.NormalChunk;
				
				if(x==OuterRows && y == OuterRows)
				{
					//base chunk

					baseChunk = chunk;
					typeToAdd = ShipChunkScript.ChunkType.BaseChunk;

					Vector3 playerPos = pos;
					playerPos.z = 0f;
					player = (GameObject)Instantiate(SpaceonautPrefab, playerPos, Quaternion.identity);
					PlayerScript ps = player.GetComponent<PlayerScript>();
					ps.globalScript = this;
				}
				else
				{
					if(x == FieldWidth-(OuterRows+1) && y == FieldHeight-(OuterRows+1))
					{
						typeToAdd = ShipChunkScript.ChunkType.TeleporterChunk;
					}
					else if(powerChunksRemaining > 0 && ( Random.value < 0.2f || chunksRemaining < powerChunksRemaining + 3))
					{
						typeToAdd = ShipChunkScript.ChunkType.PowerChunk;
						powerChunksRemaining--;
					}

				}
				ShipChunkScript chunkScript = chunk.GetComponent<ShipChunkScript>();
				chunkScript.globalScript = this;
				chunkScript.InitialiseChunk(typeToAdd);
				chunksRemaining--;
			}
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_Restart)
		{
			m_Restart = false;
			LoadLevel();
		}
		else if (m_doTitle)
		{
			//Anything here?
		}
		else
		{
			//if(Input.GetKey())
			if(Input.GetKeyDown(SwitchControllingKey))
			{
				controllingPlayer = !controllingPlayer;
			}
			if(m_hasWon || (null != player && player.GetComponent<PlayerScript>().IsDead()))
			{
				controllingPlayer = true;
			}
			CameraSizer camSize = Camera.main.GetComponent<CameraSizer>();
			
			Vector3 camPos = Camera.main.transform.position;

			if(controllingPlayer)
			{
				//This enables smooth zooming, but makes me feel ill.
				/*if(camSize.PixelsPerUnit < 64f)
				{
					camSize.PixelsPerUnit *= 1.1f;
				}

				if(camSize.PixelsPerUnit > 64f)
				{*/
					camSize.PixelsPerUnit = 64f;
				//}
				if(null != player)
				{
					camPos.x =  player.transform.position.x;
					camPos.y =  player.transform.position.y;
				}
			}
			else
			{
				
				/*if(camSize.PixelsPerUnit > 16f)
				{
					camSize.PixelsPerUnit *= 0.9f;
				}

				if(camSize.PixelsPerUnit < 16f)
				{*/
					camSize.PixelsPerUnit = 16f;
				//}

				if(null != baseChunk)
				{
					camPos.x =  baseChunk.transform.position.x;
					camPos.y =  baseChunk.transform.position.y;
				}
			}
			Camera.main.transform.position = camPos;
			camPos.z = 25;
			transform.position = camPos;

			if(Input.GetKeyDown(RestartKey))
			{
				Restart();
			}
			
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Restart();
				m_Restart = false;
				m_doTitle = true;
			}
		}
	}

	void DoTitleGUI()
	{
		int fontsize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = 64;
		GUIContent title = new GUIContent( "Disconnect");

		Vector2 titleSize = GUI.skin.label.CalcSize(title);

		GUI.Label(new Rect((Screen.width - titleSize.x)/2, 32, titleSize.x, titleSize.y), title);

		GUI.skin.label.fontSize = 32;

		
		GUI.Label(new Rect(64, 64 + titleSize.y, 512, 512), "Tab\n\nArrows / WSAD\nSpace\nQ & E / Del & PgDn\n\nR\nEsc");
		GUI.Label(new Rect(384, 64 + titleSize.y, 512, 512), "Switch between ship and Spaceonaut control\nMovement\nAttach touching chunks to ship\nRotate ship\n\nRestart game\nMenu");


		GUI.skin.label.fontSize = fontsize;
		
		if(GUI.Button(new Rect(64, Screen.height - 196, 256, 128), "Easy"))
		{
			InitEasy();
			LoadLevel();
		}
		if(GUI.Button(new Rect(Screen.width/2 - 128, Screen.height - 196, 256, 128), "Normal"))
		{
			InitNormal();
			LoadLevel();
		}
		if(GUI.Button(new Rect(Screen.width - (256+64), Screen.height - 196, 256, 128), "Hard"))
		{
			InitHard();
			LoadLevel();
		}
	}

	void DoGameGUI()
	{
		float OxygenFraction = 0f;
		float FuelFraction = 0f;
		bool IsDead = true;
		if(null != player)
		{
			PlayerScript ps = player.GetComponent<PlayerScript>();
			OxygenFraction = ps.OxygenFractionRemain();
			FuelFraction = ps.FuelFractionRemain();
			IsDead = ps.IsDead();
		}
		
		GUI.Label(new Rect(10, 10, 200, 25), "Oxygen");
		if(OxygenFraction > 0f)
		{
			GUI.skin.box.normal.background =  OxygenMeterTexture;
			GUI.Box(new Rect(200, 10, 150 * OxygenFraction, 25), GUIContent.none);
		}
		GUI.Label(new Rect(10, 35, 150, 25), "Fuel");
		if(FuelFraction > 0f) 
		{
			GUI.skin.box.normal.background =  FuelMeterTexture;
			GUI.Box(new Rect(200, 35, 150*FuelFraction, 25), GUIContent.none );
		}
		
		
		GUI.Label(new Rect(10, 60, 200, 25), "Power Connected");
		GUI.Label(new Rect(200, 60, 200, 25), "" + m_powerConnected + "/" + PowerChunksRequired);
		if (m_teleporterConnected)
		{
			GUI.Label(new Rect(10, 85, 300, 25), "Teleporter connected!");
		}
		
		if(IsDead && !m_hasWon)
		{
			Color textcol = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = Color.red;
			int fontsize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontsize * 3;
			GUIContent LoseMsg = new GUIContent("Dead!");
			GUI.Label(new Rect((Screen.width - GUI.skin.label.CalcSize(LoseMsg).x) / 2, Screen.height - 128, 5000, 250), LoseMsg);
			GUI.skin.label.normal.textColor = textcol;
			GUI.skin.label.fontSize = fontsize;
		}
		
		if(m_hasWon)
		{
			Color textcol = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = Color.green;
			int fontsize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontsize * 3;
			GUIContent WinMsg = new GUIContent("You made it home!");
			GUI.Label(new Rect((Screen.width - GUI.skin.label.CalcSize(WinMsg).x) / 2, Screen.height - 128, 5000, 250), WinMsg);
			GUI.skin.label.normal.textColor = textcol;
			GUI.skin.label.fontSize = fontsize;
		}
	}

	void OnGUI()
	{
		GUI.skin = UISkin;

		if(m_Restart) return;

		if(m_doTitle)
		{
			DoTitleGUI();
		}
		else
		{
			DoGameGUI();
		}

	}
}
