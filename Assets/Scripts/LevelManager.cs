using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    // Special characters in level loading:
    // . = dot
    // * = big dot
    // & = cherry
    // S/s = spawn for pacman
    // I/i = inky spawn
    // B/b = blinky spawn
    // P/p = pinky spawn
    // C/c = clyde spawn
    // uppercase is spawning facing upwards
    // lowercase is spawning facing downwards
    // we may do other directions if we decide we want them...

    [SerializeField]
    private string[] defaultLevelFilenames;
    [SerializeField]
    private int cherryPointsSpawnLevel = 50;

    [SerializeField]
    private TextAsset[] levelTextAssets;

    public bool ghostsCollideWithEachother = true;
    public bool bigDotsSpecial = false;
    public bool playLevel = false; // this is used to pause everything during the countdown
    [SerializeField]
    private Tilemap levelTilemap;
    [SerializeField]
    private List<char> tileKeys; // each of these are the keys for the tiles in the tiles list. Make sure to keep them in line or bad things will happen :(
    [SerializeField]
    private List<Tile> tiles; // indexed by number for now. We'll also have to have special tile numbers that represent special things like spawning, dots, and ghost spawn stuff.

    [Space]
    [SerializeField]
    private GameObject scoreDisplayParent;
    [SerializeField]
    private Text scoreDisplayText;
    [SerializeField]
    private GameObject countdownParent;
    [SerializeField]
    private Text countdownText;
    [SerializeField]
    private GameObject winTextParent;

    [Space]
    [SerializeField]
    private GameObject pacmanPrefab;
    [SerializeField]
    private GameObject inkyPrefab;
    [SerializeField]
    private GameObject blinkyPrefab;
    [SerializeField]
    private GameObject pinkyPrefab;
    [SerializeField]
    private GameObject clydePrefab;
    [SerializeField]
    private GameObject dotPrefab;
    [SerializeField]
    private GameObject bigDotPrefab;
    [SerializeField]
    private GameObject cherryPrefab;

    [Space]
    [SerializeField]
    private Camera levelCamera; // this is used to focus it on the level. We also have to account for the percent of the screen that the UI takes up.
    [SerializeField]
    private Vector2 UIScreenCoveragePercent; // this is what percentage of screen is covered by UI for the x and the y dimensions so the camera can scale the level appropriately
    [SerializeField]
    private float UIYOffset = 0; // the percent of the screen that it's offset by vertically (so that we can have more UI on th bottom than the top). Positive moves the level up, negative moves it down

    [Space]
    [SerializeField]
    private Text scoreDisplay;
    [SerializeField]
    private Text highscoreDisplay;
    private float countDownTime = 4;

    private List<List<bool>> isWalkableArray;
    private List<List<char>> tileCharArray = null;

    private Vector2 pacmanSpawnLocation;
    private SpawnOrientation pacmanSpawnOrientation;
    private Vector2[] ghostSpawnLocations;
    private SpawnOrientation[] ghostSpawnOrientations;
    private List<Vector2> dotLocations;
    private List<Vector2> bigDotLocations;
    private Vector2 cherryLocation;
    private bool levelHasCherry = false;
    private int levelWidth;
    private int levelHeight;

    private int levelLoaded = 0;
    private int points;
    private int ghostKills;
    private int currentGhostKills;
    private float levelTime;
    private int highScore;
    private int numLives;
    private List<GameObject> levelGameObjects = new List<GameObject>(); // these get destroyed and remade every time the level is reloaded.
    private List<GameObject> resetableGameObjects = new List<GameObject>(); // these are the ghosts and pacman which get reset when pacman dies
    private GameObject cherryGameObject;
    private bool hasSpawnedCherry = false; // so that you don't spawn it multiple times
    private int numDotsEaten = 0;

    private LevelLoader levelLoader;

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            HighScore = PlayerPrefs.GetInt("HighScoreLevel" + levelLoaded);

        }
        else
        {
            HighScore = 0;
        }
        levelLoader = GetComponent<LevelLoader>();
        levelLoader.ReadAsset(levelTextAssets[levelLoaded]);
        LoadFromLevelLoader();
	}

    public int Points
    {
        get
        {
            return points;
        }

        set
        {
            points = value;
            scoreDisplay.text = value.ToString("00000");
        }
    }

    public int HighScore
    {
        get
        {
            return highScore;
        }

        set
        {
            highScore = value;
            highscoreDisplay.text = value.ToString("00000");
        }
    }

    public bool GetIsTileWalkable(int x, int y)
    {
        if (x < 0 || x >= levelWidth || y < 0 || y >= levelHeight)
            return false; // can't walk if it's out of bounds
        return isWalkableArray[y][x];
    }

    public List<List<bool>> GetIsWalkableArray()
    {
        return isWalkableArray;
    }

    public SpawnOrientation[] GetGhostSpawnOrientations()
    {
        return ghostSpawnOrientations; // returns a 4 long array with the orientation of Inky Blinky Pinky and Clyde in that order
    }

    public Vector2[] GetGhostSpawnLocations()
    {
        return ghostSpawnLocations; // returns a 4 long array with the position of Inky Blinky Pinky and Clyde in that order. If the coordinates are at (-1, -1) it doesn't have a spawn location in this level
    }

    public Vector2 GetPacmanSpawnLocation()
    {
        return pacmanSpawnLocation;
    }

    public SpawnOrientation GetPacmanSpawnOrientation()
    {
        return pacmanSpawnOrientation;
    }
    
    public List<Vector2> GetDotLocations()
    {
        return dotLocations;
    }

    public List<Vector2> GetBigDotLocations()
    {
        return bigDotLocations;
    }

    public Vector2 GetCherryLocations()
    {
        return cherryLocation;
    }

    public int GetLevelWidth()
    {
        return levelWidth;
    }

    public int GetLevelHeight()
    {
        return levelHeight;
    }
    
    public Vector2Int GetLevelDimensions()
    {
        return new Vector2Int(levelWidth, levelHeight);
    }

    void SetTilemap(bool useTileArray = false)
    {
        int numPacmanSpawnLocations = 0; // these are for averaging the positions
        int[] numGhostSpawnLocations = new int[4];
        dotLocations = new List<Vector2>();
        bigDotLocations = new List<Vector2>();
        int numCherrySpawnLocations = 0; // for averaging the position
        cherryLocation = new Vector2();
        levelHasCherry = false;
        levelHeight = isWalkableArray.Count;
        levelWidth = isWalkableArray[0].Count;
        bool[] isGhostInLevel = new bool[4];
        if (useTileArray)
        {
            pacmanSpawnLocation = Vector2Int.zero;
            ghostSpawnLocations = new Vector2[4];
            ghostSpawnOrientations = new SpawnOrientation[] { SpawnOrientation.North, SpawnOrientation.North, SpawnOrientation.North, SpawnOrientation.North };
        }
        levelTilemap.ClearAllTiles();
        // for now just load a bunch of bools if they're tiles or not because that's pretty basic
        for (int y = 0; y < isWalkableArray.Count; y++)
        {
            for (int x = 0; x < isWalkableArray[0].Count; x++)
            {
                if (!useTileArray)
                {
                    if (isWalkableArray[y][x])
                    {
                        // this is the floor
                        levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                    }
                    else
                    {
                        // this is the generic solid square wall
                        levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                    }
                }
                else
                {
                    char currentTileChar = tileCharArray[y][x];
                    if (currentTileChar == 'S' || currentTileChar == 's')
                    {
                        currentTileChar = ' '; // make it a floor
                        pacmanSpawnLocation += new Vector2(x, y);
                        numPacmanSpawnLocations++; // so we can average the position later
                        if (currentTileChar == 'S')
                        {
                            pacmanSpawnOrientation = SpawnOrientation.North;
                        }
                        else
                        {
                            pacmanSpawnOrientation = SpawnOrientation.South;
                        }
                    }
                    else if (currentTileChar == 'I' || currentTileChar == 'i')
                    {
                        currentTileChar = ' '; // make it a floor
                        ghostSpawnLocations[0] += new Vector2(x, y);
                        numGhostSpawnLocations[0]++; // so we can average the position later
                        isGhostInLevel[0] = true;
                        if (currentTileChar == 'I')
                        {
                            ghostSpawnOrientations[0] = SpawnOrientation.North;
                        }
                        else
                        {
                            ghostSpawnOrientations[0] = SpawnOrientation.South;
                        }
                    }
                    else if (currentTileChar == 'B' || currentTileChar == 'b')
                    {
                        currentTileChar = ' '; // make it a floor
                        ghostSpawnLocations[1] += new Vector2(x, y);
                        numGhostSpawnLocations[1]++; // so we can average the position later
                        isGhostInLevel[1] = true;
                        if (currentTileChar == 'B')
                        {
                            ghostSpawnOrientations[1] = SpawnOrientation.North;
                        }
                        else
                        {
                            ghostSpawnOrientations[1] = SpawnOrientation.South;
                        }
                    }
                    else if (currentTileChar == 'P' || currentTileChar == 'p')
                    {
                        currentTileChar = ' '; // make it a floor
                        ghostSpawnLocations[2] += new Vector2(x, y);
                        isGhostInLevel[2] = true;
                        numGhostSpawnLocations[2]++; // so we can average the position later
                        if (currentTileChar == 'P')
                        {
                            ghostSpawnOrientations[2] = SpawnOrientation.North;
                        }
                        else
                        {
                            ghostSpawnOrientations[2] = SpawnOrientation.South;
                        }
                    }
                    else if (currentTileChar == 'C' || currentTileChar == 'c')
                    {
                        currentTileChar = ' '; // make it a floor
                        ghostSpawnLocations[3] += new Vector2(x, y);
                        isGhostInLevel[3] = true;
                        numGhostSpawnLocations[3]++; // so we can average the position later
                        if (currentTileChar == 'C')
                        {
                            ghostSpawnOrientations[3] = SpawnOrientation.North;
                        }
                        else
                        {
                            ghostSpawnOrientations[3] = SpawnOrientation.South;
                        }
                    }
                    else if (currentTileChar == '.')
                    {
                        // add a small dot
                        dotLocations.Add(new Vector2(x, y));
                        currentTileChar = ' '; // make it a floor
                    }
                    else if (currentTileChar == '*')
                    {
                        // add a big dot
                        bigDotLocations.Add(new Vector2(x, y));
                        currentTileChar = ' '; // make it a floor
                    } else if (currentTileChar == '&')
                    {
                        // get it? it's twisty like a cherry
                        numCherrySpawnLocations++;
                        levelHasCherry = true;
                        cherryLocation += new Vector2(x, y);
                        currentTileChar = ' ';
                    }

                    // then use the char mapping to the index to the tile
                    int currTileIndex = tileKeys.FindIndex(u => u == currentTileChar);
                    if (currTileIndex >= 0 && currTileIndex < tiles.Count)
                    {
                        levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[currTileIndex]);
                    } else
                    {
                        levelTilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                        Debug.LogError("CurrTileIndex not found");
                    }
                }
            }
        }
        
        // now average the spawn locations to get them located correctly:
        pacmanSpawnLocation /= numPacmanSpawnLocations;
        cherryLocation /= numCherrySpawnLocations;
        for (int i = 0; i < ghostSpawnLocations.Length; i++)
        {
            if (isGhostInLevel[i])
                ghostSpawnLocations[i] /= numGhostSpawnLocations[i];
            else
                ghostSpawnLocations[i] = -Vector2.one; // put it off the map so that the level knows not to spawn it in
        }

        // set the camera orthographic size to encompas the width and height of the level
        float ratio = levelCamera.aspect;
        // Debug.Log("Aspect ratio: " + ratio);
        // the UIScreenCoveragePercent expands the camera even further so we can have UI on the sides, this ensures that it won't overlap with the level
        levelCamera.orthographicSize = Mathf.Max(levelHeight / 2f / (1 - UIScreenCoveragePercent.y), levelWidth / 2f / ratio / (1 - UIScreenCoveragePercent.x));

        // then set the camera position, focusing on the leve, offset by the yoffset to account for the UI
        levelCamera.transform.position = new Vector3(levelWidth / 2f, levelHeight / 2f - UIYOffset * levelCamera.orthographicSize * 2, -10);
    }

    void CreateRandomIsWallList(int width = 20, int height = 20)
    {
        List<List<bool>> output = new List<List<bool>>();
        List<List<char>> newCharArray = new List<List<char>>();
        for (int y = 0; y < height; y++)
        {
            List<bool> currentY = new List<bool>();
            List<char> currentCharRow = new List<char>();
            for (int x = 0; x < width; x++)
            {
                if (Random.value < .5f)
                {
                    currentY.Add(false);
                    currentCharRow.Add('#');
                }
                else
                {
                    currentY.Add(true);
                    currentCharRow.Add(' ');
                }
            }
            output.Add(currentY);
            newCharArray.Add(currentCharRow);
        }
        newCharArray[Mathf.FloorToInt(height / 2)][Mathf.FloorToInt(width / 2)] = 'S'; // set the start position for pacman. Don't bother with the ghosts for this one I guess...
        SetMap(output, newCharArray);
    }

    private void CreateSquareMap(int width = 20, int height = 20)
    {
        List<List<bool>> output = new List<List<bool>>();
        List<List<char>> newCharArray = new List<List<char>>();
        for (int y = 0; y < height; y++)
        {
            List<bool> currentY = new List<bool>();
            List<char> currentCharRow = new List<char>();
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height-1) {
                    currentY.Add(false);
                    currentCharRow.Add('#');
                } else
                {
                    if (x == 0 || x == width -1)
                    {
                        currentY.Add(false);
                        currentCharRow.Add('#');
                    } else
                    {
                        currentY.Add(true);
                        currentCharRow.Add(' ');
                    }
                }
            }
            output.Add(currentY);
            newCharArray.Add(currentCharRow);
        }
        newCharArray[Mathf.FloorToInt(height / 2)][Mathf.FloorToInt(width / 2)] = 'S'; // set the start position for pacman. Don't bother with the ghosts for this one I guess...
        SetMap(output, newCharArray);
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
        ReloadLevel(true);
    }

    public void AddPoints(int n, int numDots = 0)
    {
        // add points to this current game of pacman
        Points += n;
        numDotsEaten += numDots;
        if (Points > highScore)
        {
            HighScore = Points;
            PlayerPrefs.SetInt("HighScoreLevel"+levelLoaded, highScore);
        }
        if (Points >= cherryPointsSpawnLevel && !hasSpawnedCherry && levelHasCherry)
        {
            hasSpawnedCherry = true;
            cherryGameObject.SetActive(true);
        }
    }

    public void PacmanDied()
    {
        numLives--;
        if (numLives >= 0)
        {
            ReloadLevel();
        } else
        {
            DisplayEndGameText();
        }
    }

    private void DisplayEndGameText(bool win = false)
    {
        Time.timeScale = 0;
        scoreDisplayParent.SetActive(true);
        scoreDisplayText.text = "Score:\n" + points.ToString("00000");
        winTextParent.SetActive(win);
    }

    public void ReloadLevel(bool resetPoints = false)
    {
        countdownParent.SetActive(true);
        playLevel = false; // so that the countdown starts
        countDownTime = 3;
        scoreDisplayParent.SetActive(false);
        Time.timeScale = 1;
        // start the countdown
        if (resetPoints)
        {
            numDotsEaten = 0;
            // this destroys all the gameobjects created and resets the level
            for (int i = 0; i < levelGameObjects.Count; i++)
            {
                Destroy(levelGameObjects[i]);
            }
            levelGameObjects = new List<GameObject>();

            Points = 0;
            ghostKills = 0;
            levelTime = 0;
            numLives = 2;
            hasSpawnedCherry = false;
        }

        // reset the ghosts and pacman
        for (int i = 0; i < resetableGameObjects.Count; i++)
        {
            Destroy(resetableGameObjects[i]);
        }
        resetableGameObjects = new List<GameObject>();

        currentGhostKills = 0;


        // now spawn new ones and add them to the levelgameobjects list:
        GameObject pacman = Instantiate(pacmanPrefab);
        resetableGameObjects.Add(pacman);
        // set the pacman transform and orientation etc:
        pacman.GetComponent<PacmanMovement>().SetLevelManager(this, pacmanSpawnLocation, pacmanSpawnOrientation);

        if (ghostSpawnLocations[0].x >= 0)
        {
            // only spawn it if the x >= 0, otherwise it's off the map because it doesn't have a spawning position
            GameObject inky = Instantiate(inkyPrefab);
            resetableGameObjects.Add(inky);
            inky.GetComponent<GhostMovement>().SetLevelManager(this, ghostSpawnLocations[0], ghostSpawnOrientations[0]);
            Debug.Log("inky added");
        }
        if (ghostSpawnLocations[1].x >= 0)
        {
            // only spawn it if the x >= 0, otherwise it's off the map because it doesn't have a spawning position
            GameObject blinky = Instantiate(blinkyPrefab);
            resetableGameObjects.Add(blinky);
            blinky.GetComponent<GhostMovement>().SetLevelManager(this, ghostSpawnLocations[1], ghostSpawnOrientations[1]);
            Debug.Log("blinky added");
        }
        if (ghostSpawnLocations[2].x >= 0)
        {
            // only spawn it if the x >= 0, otherwise it's off the map because it doesn't have a spawning position
            GameObject pinky = Instantiate(pinkyPrefab);
            resetableGameObjects.Add(pinky);
            pinky.GetComponent<GhostMovement>().SetLevelManager(this, ghostSpawnLocations[2], ghostSpawnOrientations[2]);
            Debug.Log("pinky added");
        }
        if (ghostSpawnLocations[3].x >= 0)
        {
            // only spawn it if the x >= 0, otherwise it's off the map because it doesn't have a spawning position
            GameObject clyde = Instantiate(clydePrefab);
            resetableGameObjects.Add(clyde);
            clyde.GetComponent<GhostMovement>().SetLevelManager(this, ghostSpawnLocations[3], ghostSpawnOrientations[3]);
            Debug.Log("clyde added");
        }
        if (resetPoints)
        {
            for (int i = 0; i < dotLocations.Count; i++)
            {
                GameObject dot = Instantiate(dotPrefab);
                levelGameObjects.Add(dot);
                dot.transform.position = dotLocations[i];
            }
            for (int i = 0; i < bigDotLocations.Count; i++)
            {
                GameObject bigDot = Instantiate(bigDotPrefab);
                levelGameObjects.Add(bigDot);
                bigDot.transform.position = bigDotLocations[i];
            }
            if (levelHasCherry)
            {
                GameObject cherry = Instantiate(cherryPrefab);
                levelGameObjects.Add(cherry);
                cherry.SetActive(false);
                cherryGameObject = cherry;
                cherry.transform.position = cherryLocation;
            }
        }
    }

    public void NextLevel()
    {
        // swap the level to load
        levelLoaded++;
        levelLoaded %= defaultLevelFilenames.Length;
        HighScore = PlayerPrefs.GetInt("HighScoreLevel" + levelLoaded);
        //levelLoader.ReadFile(Application.dataPath + defaultLevelFilenames[levelLoaded]);
        levelLoader.ReadAsset(levelTextAssets[levelLoaded]);
        LoadFromLevelLoader();
    }
	
	// Update is called once per frame
	void Update () {
        /*
		if (Input.GetKeyDown(KeyCode.O))
        {
            CreateRandomIsWallList();
            SetTilemap(true);
            ReloadLevel(true);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            CreateSquareMap();
            SetTilemap(true);
            ReloadLevel(true);
        }
        else */
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel(true);
        }

        if (countDownTime <= 0 && !playLevel)
        {
            playLevel = true;
            countdownParent.SetActive(false);
        } else
        {
            countDownTime -= Time.deltaTime;
            if (countDownTime <= 1)
            {
                countdownText.text = "GO!";
            }
            else
            {
                countdownText.text = countDownTime.ToString("0");
            }
        }

        if (numDotsEaten >= dotLocations.Count + bigDotLocations.Count)
        {
            // you win!
            DisplayEndGameText(true);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            // refresh the game and reset the highscore
            HighScore = 0;
            PlayerPrefs.DeleteAll();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            NextLevel();
        }
        /*
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            AddPoints(1, 10);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            HighScore += 1;
        }*/
	}

    public enum SpawnOrientation // facing that direction
    {
        North, East, South, West
    }
}
