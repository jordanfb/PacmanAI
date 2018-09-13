using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : CharacterMovement {

    [SerializeField]
    private GameObject eyesUp;
    [SerializeField]
    private GameObject eyesDown;
    [SerializeField]
    private GameObject eyesLeft;
    [SerializeField]
    private GameObject eyesRight;


    private bool collidedWithGhost = false; // this is for turning around when the ghosts collide

	// Use this for initialization
	void Start () {
	}

    /*private void FixedUpdate()
    {
        //update movement
        Vector3 nextDest = setDestination(facing);
        if (CheckCanMoveNextTile(destination))
        {
            if (currentLocation != destination)
            {
                Vector3 p = Vector3.MoveTowards(transform.position, destination, speed);
                transform.position = p;
            }
        }
        else //change direction
        {
            int dir = Random.Range(0, 1);
            if (facing == direction.Up || facing == direction.Down)
            {
                if (dir == 1)
                    facing = direction.Left;
                else
                    facing = direction.Right;
            }
            else //if we're going left or right
            {
                if (dir == 1)
                    facing = direction.Up;
                else
                    facing = direction.Down;
            }

            //reset destination and move
            setDestination(facing);
            if (currentLocation != destination)
            {
                Vector3 p = Vector3.MoveTowards(transform.position, destination, speed);
                transform.position = p;
            }
        }
    }*/

    private void FixedUpdate()
    {
        if (!levelManager.playLevel)
        {
            return; // don't move if the level isn't playing
        }
        Move();
        if (IsAtDecisionPoint())
        {
            // then decide!
            List<Vector2> choices = new List<Vector2>();
            bool[] validChoices = GhostValidDirections();
            for (int i = 0; i < validChoices.Length; i++)
            {
                if (validChoices[i])
                {
                    choices.Add(allDirections[i]);
                }
            }
            if (choices.Count == 0)
            {
                SetGoalDirection(RandomOrthogonalDirection());
            } else
            {
                SetGoalDirection(choices[Random.Range(0, choices.Count)]);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        //change eyes facing
        switch (facing)
        {
            case direction.Down:
                eyesUp.SetActive(false);
                eyesDown.SetActive(true);
                eyesLeft.SetActive(false);
                eyesRight.SetActive(false);
                break;
            case direction.Left:
                eyesUp.SetActive(false);
                eyesDown.SetActive(false);
                eyesLeft.SetActive(true);
                eyesRight.SetActive(false);
                break;
            case direction.Up:
                eyesUp.SetActive(true);
                eyesDown.SetActive(false);
                eyesLeft.SetActive(false);
                eyesRight.SetActive(false);
                break;
            case direction.Right:
                eyesUp.SetActive(false);
                eyesDown.SetActive(false);
                eyesLeft.SetActive(false);
                eyesRight.SetActive(true);
                break;
            default:
                Debug.Log("BROKE eyes switch case");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("warp"))
        {
            //handle warp stuff
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("ghost"))
        {
            Debug.Log(collidedWithGhost);
            if (!collidedWithGhost)
            {
                // turn around
                SetGoalDirection(-movementDirection);
            }
            collidedWithGhost = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ghost"))
        {
            collidedWithGhost = false;
        }
    }
}
