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
    public FSMState currentState;

    // Level 0- Wander Behavior
    // Level 1- Chase Behavior
    // Level 2- Flank then Chase Behavior
    // Level 3- Random elements added.
    // Level 4- Original Pacman Behavior

    // Called by the levelmanager on reload
    public void OnReload() {
        switch (level) {
            case 1:
                Transition(1);
                break;
            case 2:
                Transition(2);
                break;
            case 3:
                Transition(2);
                break;
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
