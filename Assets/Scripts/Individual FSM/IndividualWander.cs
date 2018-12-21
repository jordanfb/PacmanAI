using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move around aimlessly
public class IndividualWander : IndividualState {
    [SerializeField]
    private int chance = 6;
    private float time = 1;
    string followState = "WanderState"; // this should go to attacking probably

    // When active
    override public void Active() {
        GetComponent<GhostMovement>().Wander();
        if (time < 0) {
            if (Random.Range(0, chance) == 0) {
                system.Transition(followState);
            }
            time++;
        }
        time -= Time.deltaTime;
    }
}


/*
 * I need to sleep. What I've done is made these individual states. I need to disable the group FSM states from firing special events when events happen (like eating big dots)
 * and instead wire those to these when it's time. I also need to implement the death behavior of going back to the base and implement the scared behavior and the attacking behavior
 * which I'll just use a heuristic for.
 * 
 * 
 */