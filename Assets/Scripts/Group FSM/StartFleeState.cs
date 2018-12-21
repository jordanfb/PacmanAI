using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move around aimlessly
public class StartFleeState : FSMState
{
    [SerializeField]
    private int stateToFollow = 0; // after they all change direction what state should they go to?

    // When active
    override public void Active()
    {
        for (int i = 0; i < system.Ghosts.Count; i++)
        {
            system.Ghosts[i].GetComponent<GhostMovement>().FlipDirection();
        }
        system.Transition(stateToFollow);
    }
}
