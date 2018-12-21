using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : CharacterMovement {
    [SerializeField]
    private GameObject[] eyes;
    private bool collidedWithGhost = false; // this is for turning around when the ghosts collide
    private bool isDead = false;
    private bool almighty = true;

    [Header("Animation Stuff")]
    private Color originalGhostColor;
    direction currentDirection;
    public Color deadGhostColor;
    public SpriteRenderer ghostBottom;
    public SpriteRenderer ghostTop;
    public GameObject deadEyes;    

    private void Start()
    {
        SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
        originalGhostColor = ghostBottom.color;

        //animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (levelManager.playLevel && !almighty)
        {
            if (levelManager.bigDotTimer < 3)
            {
                // flash!
                if ((int)(levelManager.bigDotTimer * 8) % 2 == 0)
                {
                    // flash off
                    ghostBottom.color = deadGhostColor;
                    ghostTop.color = deadGhostColor;
                }
                else
                {
                    // flash normal
                    ghostBottom.color = originalGhostColor;
                    ghostTop.color = originalGhostColor;
                }
            }
        }
    }

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
        currentDirection = facing;
        for (int i = 0; i < eyes.Length; i++) {
            eyes[i].SetActive((int)facing == i && (almighty || isDead));
        }
        if (!almighty && !isDead)
        {
            // then turn on the scared eyes!
            deadEyes.SetActive(true);
        } else
        {
            deadEyes.SetActive(false);
        }
    }

    public void SetAlmighty(bool Almighty) {
        almighty = Almighty;
        if (!Almighty) {
            speed = 0.05f;
            ghostBottom.color = deadGhostColor;
            ghostTop.color = deadGhostColor;
        } else if (!isDead) {
            speed = 0.1f;
            ghostBottom.color = originalGhostColor;
            ghostTop.color = originalGhostColor;
        } else
        {
            speed = 0.1f; // if we aren't scared we're regular speed
        }
        SetDirection(currentDirection);
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
            ghostBottom.color = new Color(0, 0, 0, 0); // deadGhostColor;
            ghostTop.color = new Color(0, 0, 0, 0);
        } else
        {
            ghostBottom.color = originalGhostColor;
            ghostTop.color = originalGhostColor;
        }
        SetDirection(currentDirection);
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
                // enable the death state of the individual one if individual FSMs are enabled
                levelManager.GhostDied(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ghost")) {
            collidedWithGhost = false;
        }
    }
}
