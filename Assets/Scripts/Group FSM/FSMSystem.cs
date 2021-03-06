﻿using System.Collections;
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

    [Tooltip("This is just so we can see what it is it doesn't set anything")]
    public int currentStateIndex = 0;

    // State changes
    [SerializeField]
    private FSMState[] states;
    private FSMState currentState;

    [Space]
    public bool enableFSMControl = true; // this is so we can swap between version 1 and 2 in the assignment

    // Level 0- Wander Behavior
    // Level 1- Chase Behavior

    // Called by the levelmanager on reload
    public void OnReload() {
        if (level == 1) {
            Transition(1);
        }
    }

    // Use this for initialization
    void Start () {
        levelManager = GetComponent<LevelManager>();
        currentState = states[0];
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
        currentStateIndex = stateID;
    }
}
