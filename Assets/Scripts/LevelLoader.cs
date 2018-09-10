using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour {

    public string startingFileAfterDatapath = "/Levels/level1-1.txt";
    public List<char> floorTiles;
    public List<char> wallTiles;



    public List<List<bool>> pacMov = new List<List<bool>>();
    public List<List<char>> tilecharArray = new List<List<char>>();
    public int levelWidth = -1;
    public int levelHeight = -1;

    void Awake () {
        if (FileExists(Application.dataPath + startingFileAfterDatapath))
        {
            ReadFile(Application.dataPath + startingFileAfterDatapath);
        } else
        {
            Debug.LogError("Default starting file doesn't exist");
        }
    }

    public string[] DirectoryFiles(string path)
    {
        return Directory.GetFiles(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool ReadFile(string path) {
        // reads a file and returns whether or not it was successfull.
        // it delays replacing the old level until after we have passed all our checks

        List<List<bool>> newPacMov = new List<List<bool>>();
        List<List<char>> newTilecharArray = new List<List<char>>();
        int newLevelWidth = -1;
        int newLevelHeight = -1;
        bool hasPacmanSpawn = false;

        if (!FileExists(path))
        {
            return false; // errored because the file doesn't exist
        }

        StreamReader reader = new StreamReader(path);
        List<bool> tempList;
        List<char> charRow;

        string line;
        while ((line = reader.ReadLine()) != null) {
            tempList = new List<bool>();
            charRow = new List<char>();
            for (int i = 0; i < line.Length; i++)
            {
                char curr = line[i];
                charRow.Add(curr);

                if (curr == 'S' || curr == 's')
                {
                    // pacman spawn
                    /*
                     * I commented this out because we want pacman to spawn between two tiles, so it makes life easier if we can average the position of pacman between to starting positions...
                     * if (hasPacmanSpawn)
                    {
                        Debug.LogError("Tried to load a level with two pacman spawns");
                        return false; // can't have two pacman spawns
                    }*/
                    hasPacmanSpawn = true;
                }

                if (floorTiles.Contains(curr))
                {
                    tempList.Add(true);
                }
                else if (wallTiles.Contains(curr))
                {
                    tempList.Add(false);
                }
                else
                {
                    Debug.Log("ERROR: FOUND A CHAR THAT ISN'T LISTED AS A FLOOR OR WALL TILE: " + curr);
                }
            }
            newPacMov.Add(tempList);
            newTilecharArray.Add(charRow);
            if (newLevelWidth == -1)
            {
                newLevelWidth = charRow.Count;
            } else if (newLevelWidth != charRow.Count)
            {
                reader.Close();
                reader.Dispose();
                return false; // if not all rows are the same width error out
            }
        }
        reader.Close();
        reader.Dispose();

        newLevelHeight = newPacMov.Count;
        if (newLevelHeight == 0)
        {
            return false; // it errored
        }
        if (!hasPacmanSpawn)
        {
            Debug.LogError("Tried to load a level with no pacman spawn");
            return false;
        }


        // now that we know we have a correctly formed level (probably...) we replace the old one with the new one
        // reverse them so that they're the correct orientation
        newPacMov.Reverse();
        newTilecharArray.Reverse();

        pacMov = newPacMov;
        tilecharArray = newTilecharArray;
        levelWidth = newLevelWidth;
        levelHeight = newLevelHeight;
        return true;
    }
}
