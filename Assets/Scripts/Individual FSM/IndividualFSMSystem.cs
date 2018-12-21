using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualFSMSystem : MonoBehaviour {
    public string startingState;

    // Settings
    public int level = 0;
    
    // Necessary gameobjects
    [HideInInspector]
    public List<GameObject> Ghosts;
    [HideInInspector]
    public GameObject Pacman;
    private LevelManager levelManager;

    //[Tooltip("This is just so we can see what it is it doesn't set anything")]
    //public int currentStateIndex = 0;

    // State changes
    [SerializeField]
    private IndividualState[] states;
    private Dictionary<string, IndividualState> stateDictionary = new Dictionary<string, IndividualState>(); // by state name
    private IndividualState currentState;

    [Space]
    public bool enableFSMControl = true; // this is so we can swap between version 1 and 2 in the assignment

    // Level 0- Wander Behavior
    // Level 1- Chase Behavior

    // Called by the levelmanager on reload
    public void OnReload(List<List<bool>> graph, int levelwidth, int levelheight, Vector2[] spawns, int ghostIndexAssignment) {
        if (level == 1) {
            Transition("WanderState");
        }
        foreach (IndividualState s in states)
        {
            if (s.stateName == "DeathState")
            {
                // then reset the death state for pathing purposes
                ((IndividualDeath)s).OnReload(graph, levelwidth, levelheight, spawns, ghostIndexAssignment);
            }
        }
    }

    // Use this for initialization
    void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        for (int i = 0; i < states.Length; i++)
        {
            // load the states into the dictionary
            stateDictionary.Add(states[i].StateName, states[i]);
        }
        currentState = stateDictionary[startingState];
        currentState.Enter();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (levelManager.playLevel && enableFSMControl) {
            currentState.Active();
        }
    }

    // Move to another state
    public void Transition(int stateID) {
        currentState = states[stateID];
        currentState.Enter();
    }

    public void Transition(string stateName) {
        currentState = stateDictionary[stateName];
        currentState.Enter();
    }
}
