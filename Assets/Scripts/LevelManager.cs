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
    private List<List<char>> tileCharArray = null;

    private LevelLoader levelLoader;

	// Use this for initialization
	void Start () {
        levelLoader = GetComponent<LevelLoader>();
	}

    public bool GetIsTileWalkable(int x, int y)
    {
        return isWalkableArray[y][x];
    }

    public List<List<bool>> GetIsWalkableArray()
    {
        return isWalkableArray;
    }

    void SetTilemap(bool useTileArray = false)
    {
        levelTilemap.ClearAllTiles();
        // for now just load a bunch of bools if they're tiles or not because that's pretty basic
        for (int y = 0; y < isWalkableArray.Count; y++)
        {
            for (int x = 0; x < isWalkableArray[0].Count; x++)
            {
                if (useTileArray)
                {
                    if (isWalkableArray[y][x])
                    {
                        levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                    }
                    else
                    {
                        levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                    }
                } else
                {
                    // then use the char mapping to the index to the tile
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

    private void SetMap(List<List<bool>> walkable, List<List<char>> charArray = null)
    {
        isWalkableArray = walkable;
        tileCharArray = charArray;
    }

    private void LoadFromLevelLoader()
    {
        SetMap(levelLoader.pacMov, levelLoader.tilecharArray);
        SetTilemap(true);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
        {
            SetMap(CreateRandomIsWallList());
            SetTilemap();
        } else if (Input.GetKeyDown(KeyCode.I))
        {
            CreateSquareMap();
            SetTilemap();
        } else if (Input.GetKeyDown(KeyCode.L))
        {
            // load it from the level loader's contents
            LoadFromLevelLoader();
        }
	}
}
