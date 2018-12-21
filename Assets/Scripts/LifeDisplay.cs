using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDisplay : MonoBehaviour {

    [SerializeField]
    private LevelManager levelManager;
    [SerializeField]
    private GameObject pacManLifeSprite;
    [SerializeField]
    private float offset = 1;

    private List<GameObject> spawnedPacmanSprites = new List<GameObject>();
    private int numEnabled = 0;
	
	// Update is called once per frame
	void Update () {
        if (spawnedPacmanSprites.Count < levelManager.numLives - 1)
        {
            // spawn more
            GameObject pl = Instantiate(pacManLifeSprite, transform.position + offset * Vector3.right * spawnedPacmanSprites.Count, Quaternion.identity, transform);
            spawnedPacmanSprites.Add(pl);
        }
        else if (numEnabled != levelManager.numLives)
        {
            // then update the num enabled
            for (int i = 0; i < spawnedPacmanSprites.Count; i++)
            {
                if (i < levelManager.numLives-1) // always display one fewer lives than you have so that when you're alive at 0 lives you have nothing showing this is probably -1 because we store lives 1-3 I think I guess? fencepost errors man.
                {
                    // then enable it
                    spawnedPacmanSprites[i].SetActive(true);
                } else
                {
                    // disable it
                    spawnedPacmanSprites[i].SetActive(false);
                }
            }
        }
    }
}
