using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    protected GameObject levelManangerObj;
    public float speed = 1f;
    protected LevelManager levelManager;
    protected Vector3 destination;
    protected Vector3 currentLocation;
    public direction facing;


    private void Awake()
    {
        //try and find a better way to do this than .Find later
        levelManangerObj = GameObject.Find("LevelManager");
        levelManager = levelManangerObj.GetComponent<LevelManager>();
        facing = direction.Right;
        destination = currentLocation = transform.position.Round(0);
    }

    //see if we can move to the next tile
    protected bool CheckCanMoveNextTile(Vector3 toCheck)
    {
        return levelManager.GetIsTileWalkable((int)toCheck.x, (int)toCheck.y);
    }

    //returns the most recent destination before the function call
    protected Vector3 setDestination(direction thisFacing)
    {
        currentLocation = transform.position.Round(0);
        Vector3 toMoveTo = currentLocation;

        switch (thisFacing)
        {
            case direction.Up:
                toMoveTo = currentLocation + new Vector3(0, 1, 0);
                break;
            case direction.Right:
                toMoveTo = currentLocation + new Vector3(1, 0, 0);
                break;
            case direction.Down:
                toMoveTo = currentLocation + new Vector3(0, -1, 0);
                break;
            case direction.Left:
                toMoveTo = currentLocation + new Vector3(-1, 0, 0);
                break;
            default:
                Debug.Log("BROKE direction switch case");
                break;
        }
        return toMoveTo;
    }

    private void FixedUpdate()
    {
        //MAKE SURE in child function you do the following:
        /*
         * Assign rotation/eyes
         * determine direction facing, be it from input or otherwise
         * CheckCanMoveTile and resulting behavior
         * probably more things?
         */
    }

    public enum direction
    {
        Up, Down, Left, Right
    }
}
