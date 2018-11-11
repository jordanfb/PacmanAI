using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSystem : MonoBehaviour {
    [SerializeField]
    private FSMState[] states;
    private FSMState currentState;

	// Use this for initialization
	void Start () {
        currentState = states[0];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        currentState.Active();
	}

    public void Transition(int stateID) {
        currentState = states[stateID];
    }
}
