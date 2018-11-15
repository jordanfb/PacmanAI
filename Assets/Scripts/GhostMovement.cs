using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : CharacterMovement {
    [SerializeField]
    private GameObject[] eyes;
    private bool collidedWithGhost = false; // this is for turning around when the ghosts collide
    private bool isDead = false;
    private bool almighty = true;

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

    public void SetAlmighty(bool Almighty) {
        almighty = Almighty;
        if (!Almighty) {
            speed = 0.05f;
        } else {
            speed = 0.1f;
        }
    }

    // Is ghost F?
    public bool Ded() {
        return isDead;
    }

    // Yes, the ghost is ded. DED.
    public void DedGhost(bool isded) {
        isDead = isded;
        GetComponent<BoxCollider2D>().enabled = !isded;
        if (isded) {
            speed = 0.2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("warp")) {
            //handle warp stuff
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("ghost")) {
            if (!collidedWithGhost) {
                // turn around
                SetGoalDirection((currDirection + 2) % 4);
            }
            collidedWithGhost = true;
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("pacman")) {
            // If almighty, don't die
            if (almighty) {
                levelManager.PacmanDied();
            } else {
                DedGhost(true);
                SetAlmighty(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ghost")) {
            collidedWithGhost = false;
        }
    }
}
