﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    protected GameObject levelManangerObj;
    public float speed = 1f;
    protected LevelManager levelManager;
    protected Vector3 currentLocation;
    public direction facing;

    protected Vector2 goalDirection = Vector2.zero;
    // this is the direction that the character is trying to move towards. The child classes set this.
    // goal direction is a goal, it's not actual direction.
    protected Vector2 movementDirection = Vector2.zero; // this is the actual direction that the character is moving in
    protected Vector2 destination;
    // this is a valid position which is where you are moving towards. Whenever you are at an integer position you set this

    private bool atDecisionPoint = false;

    public void SetLevelManager(LevelManager lm, Vector3 pos, LevelManager.SpawnOrientation orientation)
    {
        levelManager = lm;
        transform.position = pos;
        destination = pos;
        currentLocation = pos;
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

    protected bool IsMoving()
    {
        // this returns true when the character is moving. Possibly used for animation?
        return (Vector2)transform.position != destination;
    }

    protected bool[] ValidDirections()
    {
        // it returns an array of what directions it can turn at this point.
        // ghosts can't turn backwards but pacman can, but this function doesn't differentiate
        bool[] dirs = { false, false, false, false };
        return dirs;
    }

    protected bool[] GhostValidDirections()
    {
        return ValidDirections();
    }

    public void SetGoalDirection(Vector2 dir)
    {
        goalDirection = dir;
    }

    public bool IsAtDecisionPoint()
    {
        return atDecisionPoint;
    }

    protected void Move()
    {
        //MAKE SURE in child function you do the following:
        /*
         * Assign rotation/eyes
         * determine direction facing, be it from input or otherwise
         * CheckCanMoveTile and resulting behavior
         * probably more things?
         */
        atDecisionPoint = false;

        Vector2 currentPos = transform.position;
        transform.position = Vector2.MoveTowards(currentPos, destination, speed);
        if ((Vector2)transform.position == destination)
        {
            atDecisionPoint = true;
            // come up with a new destination
            Vector2Int goalDestination = new Vector2Int((int)(transform.position.x + goalDirection.x), (int)(transform.position.y + goalDirection.y));
            if (levelManager.GetIsTileWalkable(goalDestination.x, goalDestination.y))
            {
                destination = goalDestination;
                movementDirection = goalDirection;
            }
            else
            {
                // otherwise try moving in the same direction you were moving in
                Vector2Int currentDestination = new Vector2Int((int)(transform.position.x + movementDirection.x), (int)(transform.position.y + movementDirection.y));
                if (levelManager.GetIsTileWalkable(currentDestination.x, currentDestination.y))
                {
                    destination = currentDestination;
                }
                else
                {
                    // if you can't move anywhere you're in a dead end or need to make a choice
                    // Just do nothing, the code will return that you're stuck since your destination is the same as your position
                }
            }
        }
    }

    public enum direction
    {
        Up, Down, Left, Right
    }
}
