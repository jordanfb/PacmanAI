using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move around aimlessly
public class WanderState : FSMState {
    [SerializeField]
    private int chance = 6;
    private float time = 1;

    // When active
    override public void Active() {
        for (int i = 0; i < system.Ghosts.Count; i++) {
            system.Ghosts[i].GetComponent<GhostMovement>().Wander();
        }
        if (time < 0) {
            if (Random.Range(0, chance) == 0) {
                system.Transition(1);
            }
            time++;
        }
        time -= Time.deltaTime;
    }
}
