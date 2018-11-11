using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSystem : MonoBehaviour {
    // Necessary gameobjects
    [HideInInspector]
    public List<GameObject> Ghosts;
    [HideInInspector]
    public GameObject Pacman;
    private LevelManager levelManager;

    // State changes
    [SerializeField]
    private FSMState[] states;
    private FSMState currentState;

    // Use this for initialization
    void Start () {
        levelManager = GetComponent<LevelManager>();
        currentState = states[0];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (levelManager.playLevel) {
            currentState.Active();
        }
    }

    // Move to another state
    public void Transition(int stateID) {
        currentState = states[stateID];
    }
}
