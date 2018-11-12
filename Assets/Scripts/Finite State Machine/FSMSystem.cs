using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSystem : MonoBehaviour {
    // Settings
    public int level = 0;
    
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

    // Level 0- Wander Behavior
    // Level 1- Chase Behavior

    // Called by the levelmanager on reload
    public void OnReload() {
        if (level == 1) {
            Transition(2);
        }
    }

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
