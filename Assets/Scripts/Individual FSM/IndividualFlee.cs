using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move around aimlessly
public class IndividualFlee : IndividualState
{
    [SerializeField]
    private string stateToFollow = "FleeState"; // after they all change direction what state should they go to?

    // When active
    override public void Enter()
    {
        GetComponent<GhostMovement>().FlipDirection(); // then keep wandering just walk the other way
        system.Transition(stateToFollow);
    }
}
