using UnityEngine;
using System.Collections;

public class ShipChunkScript : MonoBehaviour {

	
	public string[] ChunkLayouts =
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

	// Use this for initialization
	void Start () {

		TileMapScript tms = GetComponent<TileMapScript>();
		tms.LevelData = ChunkLayouts[0];
		tms.GenerateMap();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
