using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public KeyCode SwitchControllingKey = KeyCode.Tab;
	public KeyCode UpKey = KeyCode.UpArrow;
	public KeyCode DownKey = KeyCode.DownArrow;
	public KeyCode LeftKey = KeyCode.LeftArrow;
	public KeyCode RightKey = KeyCode.RightArrow;
	public KeyCode RotateLeftKey = KeyCode.Delete;
	public KeyCode RotateRightKey = KeyCode.PageDown;
	public KeyCode ActionKey = KeyCode.Space;
	public KeyCode RestartKey = KeyCode.R;

	public GameObject ShipChunkPrefab;
	public GameObject SpaceonautPrefab;
	public GUISkin UISkin;
	public Texture2D OxygenMeterTexture;
	public Texture2D FuelMeterTexture;

	public int FieldWidth = 6;
	public int FieldHeight = 7;
	public float GapWidth = 3.0f;
	public int PowerChunks = 3;
	public int PowerChunksRequired = 2;


	GameObject player;
	GameObject baseChunk;
	bool controllingPlayer = false;

	int m_powerConnected = 0;
	bool m_teleporterConnected = false;
	bool m_hasWon = false;
	bool m_Restart = false;

	// Use this for initialization
	void Start ()
	{
		LoadLevel();	
	}

	void Restart()
	{
		m_Restart = true;
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

	public void LoadLevel()
	{
		m_powerConnected = 0;
		m_teleporterConnected = false;
		m_hasWon = false;
		m_Restart = false;


		//This should either get the Squares_* from TileMapScript, or set it, rather than being hardcoded...
		float xSpread =5 * 1 + GapWidth;// (tms.Squares_X * tms.Square_Size + GapWidth);
		float ySpread =5 * 1 + GapWidth;// (tms.Squares_Y * tms.Square_Size + GapWidth);

		int powerChunksRemaining = PowerChunks;
		int chunksRemaining = FieldWidth * FieldHeight;

		for(int x = 0; x < FieldWidth; x++)
		{
			for(int y = 0; y < FieldHeight; y++)
			{
				Vector3 pos =  new Vector3((float)x * xSpread, (float)y * ySpread, 10f);
				GameObject chunk = (GameObject) Instantiate(ShipChunkPrefab, pos, Quaternion.identity);
				ShipChunkScript.ChunkType typeToAdd = ShipChunkScript.ChunkType.NormalChunk;
				
				if(x==1 && y == 1)
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
					if(x == FieldWidth-2 && y == FieldHeight-2)
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
		}
	}

	void OnGUI()
	{
		if(m_Restart) return;

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
		GUI.skin = UISkin;

		GUI.Label(new Rect(10, 10, 150, 25), "Oxygen");
		if(OxygenFraction > 0f)
		{
			GUI.skin.box.normal.background =  OxygenMeterTexture;
			GUI.Box(new Rect(150, 10, 150 * OxygenFraction, 25), GUIContent.none);
		}
		GUI.Label(new Rect(10, 35, 150, 25), "Fuel");
		if(FuelFraction > 0f) 
		{
			GUI.skin.box.normal.background =  FuelMeterTexture;
			GUI.Box(new Rect(150, 35, 150*FuelFraction, 25), GUIContent.none );
		}

		
		GUI.Label(new Rect(10, 60, 150, 25), "Power connected");
		GUI.Label(new Rect(150, 60, 150, 25), "" + m_powerConnected + "/" + PowerChunksRequired);
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
			GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 25, 300, 250), "Dead!");
			GUI.skin.label.normal.textColor = textcol;
			GUI.skin.label.fontSize = fontsize;
		}

		if(m_hasWon)
		{
			Color textcol = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = Color.green;
			int fontsize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontsize * 3;
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 + 25, 500, 250), "You've made it home!");
			GUI.skin.label.normal.textColor = textcol;
			GUI.skin.label.fontSize = fontsize;
		}
	}
}
