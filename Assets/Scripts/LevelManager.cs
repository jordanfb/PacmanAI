using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private Tilemap levelTilemap;
    [SerializeField]
    private List<Tile> tiles; // indexed by number for now. We'll also have to have special tile numbers that represent special things like spawning, dots, and ghost spawn stuff.

    private List<List<bool>> isWalkableArray;

	// Use this for initialization
	void Start () {
		
	}

    public bool GetIsTileWalkable(int x, int y)
    {
        return isWalkableArray[y][x];
    }

    public List<List<bool>> GetIsWalkableArray()
    {
        return isWalkableArray;
    }

    void SetTilemap(List<List<bool>> isWallList)
    {
        levelTilemap.ClearAllTiles();
        // for now just load a bunch of bools if they're tiles or not because that's pretty basic
        for (int y = 0; y < isWallList.Count; y++)
        {
            for (int x = 0; x < isWallList[0].Count; x++)
            {
                if (isWallList[y][x])
                {
                    levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                } else
                {
                    levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                }
            }
        }
    }

    List<List<bool>> CreateRandomIsWallList(int width = 20, int height = 20)
    {
        List<List<bool>> output = new List<List<bool>>();
        for (int y = 0; y < height; y++)
        {
            List<bool> currentZ = new List<bool>();
            for (int x = 0; x < width; x++)
            {
                currentZ.Add(Random.value < .5);
            }
            output.Add(currentZ);
        }
        return output;
    }

    private void CreateSquareMap(int width = 20, int height = 20)
    {
        List<List<bool>> output = new List<List<bool>>();
        for (int y = 0; y < height; y++)
        {
            List<bool> currentZ = new List<bool>();
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height-1) {
                    currentZ.Add(false);
                } else
                {
                    if (x == 0 || x == width -1)
                    {
                        currentZ.Add(false);
                    } else
                    {
                        currentZ.Add(true);
                    }
                }
            }
            output.Add(currentZ);
        }
        SetMap(output);
    }

    private void SetMap(List<List<bool>> walkable)
    {
        isWalkableArray = walkable;
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
        {
            SetMap(CreateRandomIsWallList());
            SetTilemap(isWalkableArray);
        } else if (Input.GetKeyDown(KeyCode.I))
        {
            CreateSquareMap();
            SetTilemap(isWalkableArray);
        }
	}
}
