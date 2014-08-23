using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public KeyCode SwitchControllingKey = KeyCode.Tab;
	public KeyCode UpKey = KeyCode.UpArrow;
	public KeyCode DownKey = KeyCode.DownArrow;
	public KeyCode LeftKey = KeyCode.LeftArrow;
	public KeyCode RightKey = KeyCode.RightArrow;
	public KeyCode ActionKey = KeyCode.Space;

	public GameObject ShipChunkPrefab;
	public GameObject SpaceonautPrefab;

	public int FieldWidth = 5;
	public int FieldHeight = 6;
	public float GapWidth = 0.0f;


	GameObject player;
	GameObject baseChunk;
	public bool controllingPlayer = false;

	// Use this for initialization
	void Start ()
	{
		LoadLevel();	
	}

	public bool IsControllingPlayer()
	{
		return controllingPlayer;
	}

	public void LoadLevel()
	{
		baseChunk = (GameObject) Instantiate(ShipChunkPrefab, new Vector3(0f, 0f, 10f), Quaternion.identity);
		ShipChunkScript scs = baseChunk.GetComponent<ShipChunkScript>();
		scs.globalScript = this;
		scs.InitialiseChunk(ShipChunkScript.ChunkType.BaseChunk);

		TileMapScript tms = baseChunk.GetComponent<TileMapScript>();
		player = (GameObject)Instantiate(SpaceonautPrefab, new Vector3(tms.Squares_X * tms.Square_Size * 0.5f, tms.Squares_Y * tms.Square_Size * 0.5f, 0f), Quaternion.identity);
		PlayerScript ps = player.GetComponent<PlayerScript>();
		ps.globalScript = this;

		float xSpread = (tms.Squares_X * tms.Square_Size + GapWidth);
		float ySpread = (tms.Squares_Y * tms.Square_Size + GapWidth);

		for(int x = 0; x < FieldWidth; x++)
		{
			for(int y = 0; y < FieldHeight; y++)
			{
				if(x==0 && y == 0)
				{
					//base chunk, already created
				}
				else
				{
					GameObject chunk = (GameObject) Instantiate(ShipChunkPrefab, new Vector3((float)x * xSpread, (float)y * ySpread, 10f), Quaternion.identity);
					
					ShipChunkScript chunkScript = chunk.GetComponent<ShipChunkScript>();
					chunkScript.globalScript = this;
					if(x == FieldWidth-1 && y == FieldHeight-1)
					{
						chunkScript.InitialiseChunk(ShipChunkScript.ChunkType.TeleporterChunk);
					}
					else
					{
						chunkScript.InitialiseChunk(ShipChunkScript.ChunkType.NormalChunk);
					}

				}
			}
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
		//if(Input.GetKey())
		if(Input.GetKeyDown(SwitchControllingKey))
		{
			controllingPlayer = !controllingPlayer;
		}
		CameraSizer camSize = Camera.main.GetComponent<CameraSizer>();
		
		Vector3 camPos = Camera.main.transform.position;

		if(controllingPlayer)
		{
			camSize.PixelsPerUnit = 64f;
			camPos.x =  player.transform.position.x;
			camPos.y =  player.transform.position.y;
		}
		else
		{
			camSize.PixelsPerUnit = 16f;
			camPos.x =  baseChunk.transform.position.x;
			camPos.y =  baseChunk.transform.position.y;
		}
		Camera.main.transform.position = camPos;
	}
}
