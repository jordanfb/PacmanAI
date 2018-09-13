using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMovement : CharacterMovement {

    private Vector2 lastInput = new Vector2(0, 0);
    private float Up = 0;
    private float Right = 0;
    private direction nextFacing;
    private bool startedMoving = false;

    private void Update()
    {
        Up = Input.GetAxis("Vertical");
        Right = Input.GetAxis("Horizontal");
        Vector2 inputDir = new Vector2(Right, Up);
        if (inputDir.SqrMagnitude() > 0 && (inputDir.x == 0 || inputDir.y == 0))
        {
            SetGoalDirection(inputDir.normalized);
            startedMoving = true;
        }


        //change direction facing
        switch (facing)
        {
            case direction.Down:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case direction.Left:
                transform.eulerAngles = new Vector3(0, 0, -180);
                break;
            case direction.Up:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case direction.Right:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            default:
                Debug.Log("BROKE rotation switch case");
                break;
        }
    }

    private void FixedUpdate()
    {
        if (startedMoving)
            Move();
    }

    /*private void FixedUpdate()
    {
        //change direction facing
        switch (facing)
        {
            case direction.Down:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case direction.Left:
                transform.eulerAngles = new Vector3(0, 0, -180);
                break;
            case direction.Up:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case direction.Right:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            default:
                Debug.Log("BROKE rotation switch case");
                break;
        }

        //update movement
        Vector3 nextDest = setDestination(nextFacing);
        //first check if the next space is a viable option
        if (CheckCanMoveNextTile(nextDest)) //if that is a viable space to move to, change destination and go
        {
            destination = nextDest;
            if (currentLocation != destination)
            {
                Vector3 p = Vector3.MoveTowards(transform.position, destination, speed);
                transform.position = p;
            }
        //if that space doesn't work, try contiuing forward
        } else if (CheckCanMoveNextTile(destination))
        {
            if (currentLocation != destination)
            {
                Vector3 p = Vector3.MoveTowards(transform.position, destination, speed);
                transform.position = p;
            }
        }
        else //if we can't turn and forward doesn't work, stop
        {
            //no change
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("tell me something nice");
        //dot handling
        if(collision.gameObject.layer == LayerMask.NameToLayer("dot"))
        {
            levelManager.AddPoints(10);
            collision.gameObject.SetActive(false);
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("dot"))
        {
            //lose a life
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("warp"))
        {
            //handle warp stuff
        }
    }
}
