using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelLoading : MonoBehaviour {

    public List<List<bool>> pacMov = new List<List<bool>>();
    public float t;

    // Use this for initialization
    void Start () {
        ReadFile("Assets/Resources/tileset.txt");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReadFile(string path) {
        StreamReader reader = new StreamReader(path);
        List<bool> tempList = new List<bool>();
        pacMov.Add(tempList);

        while (!reader.EndOfStream) {
            if (reader.Peek() == '\n') {
                tempList = new List<bool>();
            } else if (reader.Peek() == ' ') {
                tempList.Add(true);
            } else if (reader.Peek() == '#') {
                tempList.Add(false);
            }
            reader.Read();
        }
        reader.Close();
        reader.Dispose();
    }
}
