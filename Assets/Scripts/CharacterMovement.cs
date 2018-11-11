using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    public float speed = 1f;
    protected LevelManager levelManager;

    // Direction to face
    protected enum direction {
        Up, Right, Down, Left
    }
    // Directions are represented as ints to represent the values in allDirections
    // north east south west
    protected Vector2[] allDirections = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    // The queued input, or the direction the character would move if possible.
    protected int goalDirection = 0;
    // The direction the character is currently moving on
    protected int currDirection = 0;

    // this is a valid position which is where you are moving towards. Whenever you are at an integer position you set this
    protected Vector2 destination;
    public bool atDecisionPoint = false;

    public void SetLevelManager(LevelManager lm, Vector3 pos, LevelManager.SpawnOrientation orientation) {
        levelManager = lm;
        transform.position = pos;
        destination = pos;
        goalDirection = (int)orientation;
    }

    //see if we can move to the next tile
    protected bool CheckCanMoveNextTile(Vector3 toCheck) {
        return levelManager.GetIsTileWalkable((int)toCheck.x, (int)toCheck.y);
    }

    // this returns true when the character is moving. Possibly used for animation?
    protected bool IsMoving() {
        return (Vector2)transform.position != destination;
    }

    // it returns an array of what directions it can turn at this point.
    // north east south west
    protected List<int> ValidDirections() {
        List<int> dirs = new List<int>();
        for (int i = 0; i < 4; i++) {
            if (CheckCanMoveNextTile(allDirections[i] + (Vector2)transform.position)) {
                dirs.Add(i);
            }
        }
        return dirs;
    }

    public void SetGoalDirection(int dir) {
        goalDirection = dir;
    }

    protected void Move() {
        //MAKE SURE in child function you do the following:
        /*
         * Assign rotation/eyes
         * determine direction facing, be it from input or otherwise
         * CheckCanMoveTile and resulting behavior
         * probably more things?
         */

        if (atDecisionPoint) {
            // come up with a new destination
            Vector2Int goalDestination = new Vector2Int((int)(transform.position.x + allDirections[goalDirection].x), (int)(transform.position.y + allDirections[goalDirection].y));
            if (levelManager.GetIsTileWalkable(goalDestination.x, goalDestination.y)) {
                destination = goalDestination;
                currDirection = goalDirection;
            } else {
                // otherwise try moving in the same direction you were moving in
                Vector2Int currentDestination = new Vector2Int((int)(transform.position.x + allDirections[currDirection].x), (int)(transform.position.y + allDirections[currDirection].y));
                if (levelManager.GetIsTileWalkable(currentDestination.x, currentDestination.y)) {
                    destination = currentDestination;
                }
                // if you can't move anywhere you're in a dead end or need to make a choice
                // Just do nothing, the code will return that you're stuck since your destination is the same as your position
            }
        } else if (goalDirection == (currDirection + 2) % 4) {
            // this is for pacman when he changes direction
            // if it's a valid choice then sure he can go there
            Vector2Int goalDestination = new Vector2Int((int)(transform.position.x + allDirections[goalDirection].x), (int)(transform.position.y + allDirections[goalDirection].y));
            if (levelManager.GetIsTileWalkable(goalDestination.x, goalDestination.y)) {
                destination = goalDestination;
                currDirection = goalDirection;
            }
        }

        // set the direction for art purposes
        SetDirection((direction)currDirection);

        // moved the movement down here so that ghosts get a chance to choose where to go
        atDecisionPoint = destination == (Vector2)transform.position;
        transform.position = Vector2.MoveTowards(transform.position, destination, speed);
    }

    // Base function to override based on ghost or pacman
    virtual protected void SetDirection(direction facing) { }
}
