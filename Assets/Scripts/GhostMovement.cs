using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : CharacterMovement {
    [SerializeField]
    private GameObject[] eyes;
    private bool collidedWithGhost = false; // this is for turning around when the ghosts collide

    private void FixedUpdate() {
        if (levelManager.playLevel) {
            Move();
        }
    }

    // Move in a random direction
    public void Wander() {
        if (atDecisionPoint) {
            List<int> choices = ValidDirections(false);
            if (choices.Count == 0) {
                SetGoalDirection(Random.Range(0, 4));
            } else {
                SetGoalDirection(choices[Random.Range(0, choices.Count)]);
            }
        }
    }

    // Set the direction
    override protected void SetDirection(direction facing) {
        for (int i = 0; i < eyes.Length; i++) {
            eyes[i].SetActive((int)facing == i);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("warp")) {
            //handle warp stuff
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("ghost")) {
            if (!collidedWithGhost) {
                // turn around
                SetGoalDirection((currDirection + 2) % 4);
            }
            collidedWithGhost = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ghost")) {
            collidedWithGhost = false;
        }
    }*/
}
