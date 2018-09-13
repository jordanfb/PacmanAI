using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMovement : CharacterMovement {

    private Vector2 lastInput = new Vector2(0, 0);
    private float Up = 0;
    private float Right = 0;
    private direction nextFacing;

    private void Update()
    {
        Up = Input.GetAxis("Vertical");
        Right = Input.GetAxis("Horizontal");
        //get directional input for one pac-y boi
        //Vertical axis has priority over horizontal
        if (Up == 1)
            nextFacing = direction.Up;
        else if (Up == -1)
            nextFacing = direction.Down;
        else if (Right == 1)
            nextFacing = direction.Right;
        else if (Right == -1)
            nextFacing = direction.Left;
        //otherwise no change
        Debug.Log(facing);
    }

    private void FixedUpdate()
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
                GetComponent<Rigidbody2D>().MovePosition(p);
            }
        //if that space doesn't work, try contiuing forward
        } else if (CheckCanMoveNextTile(destination))
        {
            if (currentLocation != destination)
            {
                Vector3 p = Vector3.MoveTowards(transform.position, destination, speed);
                GetComponent<Rigidbody2D>().MovePosition(p);
            }
        }
        else //if we can't turn and forward doesn't work, stop
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //dot handling
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("dot"))
        {
            levelManager.AddPoints(10);
            collision.collider.gameObject.SetActive(false);
        } else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("dot"))
        {
            //lose a life
        }
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("warp"))
        {
            //handle warp stuff
        }
    }
}
