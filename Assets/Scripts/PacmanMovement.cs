using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMovement : CharacterMovement {
    private float Up = 0;
    private float Right = 0;
    private bool startedMoving = false;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        Up = Input.GetAxis("Vertical");
        Right = Input.GetAxis("Horizontal");
        Vector2 inputDir = new Vector2(Right, Up);
        if (inputDir.SqrMagnitude() > 0 && (inputDir.x == 0 || inputDir.y == 0))
        {
            if (inputDir.y > 0) {
                SetGoalDirection(0);
            } else if (inputDir.y < 0) {
                SetGoalDirection(2);
            }
            if (inputDir.x > 0) {
                SetGoalDirection(1);
            } else if (inputDir.x < 0) {
                SetGoalDirection(3);
            }
            startedMoving = true;
        }

        animator.SetFloat("speed", (IsMoving() ? 1 : 0));
    }

    // Set the direction
    override protected void SetDirection(direction facing) {
        //change direction facing
        switch (facing) {
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
        if (startedMoving && levelManager.playLevel)
            Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //dot handling
        if (collision.gameObject.layer == LayerMask.NameToLayer("dot"))
        {
            if (collision.gameObject.CompareTag("dot"))
            {
                levelManager.AddPoints(1, 1);
            }
            else
            {
                // collided with a big dot
                if (levelManager.bigDotsSpecial)
                {
                    levelManager.AddPoints(5, 1); // give 5 times the points like in regular pacman I guess, even though cherries are 100...
                }
                else
                {
                    levelManager.AddPoints(1, 1); // for now just do the same as regular dots
                }
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("fruit"))
        {
            if (collision.gameObject.CompareTag("cherry"))
            {
                levelManager.AddPoints(100);
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("ghost"))
        {
            //lose a life
            levelManager.PacmanDied();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("warp"))
        {
            //handle warp stuff
        }
    }
}
